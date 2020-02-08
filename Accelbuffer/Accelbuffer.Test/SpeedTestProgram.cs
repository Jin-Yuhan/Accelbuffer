using Accelbuffer.Memory;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Collections.Generic;
using System.IO;
using PSerializer = ProtoBuf.Serializer;

namespace Accelbuffer.Test
{
    public static class SpeedTestProgram
    {
        public static void ProgramMain()
        {
            Serializer.InitializeForType<SpeedTest>();
            Serializer.InitializeForType<SpeedTest.Nested>();
            PSerializer.PrepareSerializer<SpeedTest>();
            PSerializer.PrepareSerializer<SpeedTest.Nested>();
            BenchmarkRunner.Run<TestClass>();
        }

        [AllStatisticsColumn, MemoryDiagnoser, SimpleJob(RuntimeMoniker.Net472)]
        public class TestClass
        {
            public readonly SpeedTest m_Test;
            private readonly NativeBuffer m_Buffer;
            private readonly MemoryStream m_Ms;

            public TestClass()
            {
                m_Test = new SpeedTest
                {
                    FixedIntValue = 111,
                    StrValue = "Hello world",
                    ArrayValue = new SpeedTest.Nested[] { new SpeedTest.Nested { BooleanValue = false, FloatValue = 12.3f }, new SpeedTest.Nested { BooleanValue = false, FloatValue = 12.3f } },
                    DictValue = new Dictionary<string, char>() { ["A"] = 'A', ["B"] = 'B', ["C"] = 'C' }
                };

                m_Buffer = Serializer.Serialize<SpeedTest>(m_Test, Encoding.ASCII, Endian.LittleEndian);
                m_Ms = new MemoryStream();
                PSerializer.Serialize<SpeedTest>(m_Ms, m_Test);
            }

            [GlobalCleanup]
            private void Free()
            {
                m_Buffer.Dispose();
                m_Ms.Dispose();
                MemoryAllocator.Shared.Trim(true);
            }

            [Benchmark]
            public void AccelbufferSerialize()
            {
                Serializer.Serialize<SpeedTest>(m_Test, Encoding.Unicode, Endian.LittleEndian).Dispose();
            }

            [Benchmark]
            public void AccelbufferDeserialize()
            {
                Serializer.Deserialize<SpeedTest>(m_Buffer, 0, m_Buffer.Length);
            }

            [Benchmark]
            public void ProtobufSerialize()
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PSerializer.Serialize<SpeedTest>(ms, m_Test);
                }
            }

            [Benchmark]
            public void ProtobufDeserialize()
            {
                m_Ms.Seek(0, SeekOrigin.Begin);
                PSerializer.Deserialize<SpeedTest>(m_Ms);
            }
        }
    }
}

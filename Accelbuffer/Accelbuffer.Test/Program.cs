using Accelbuffer.Memory;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using PSerializer = ProtoBuf.Serializer;

namespace Accelbuffer.Test
{
    static class Program
    {
        private static void Main()
        {
            Serializer.InitializeForType<Test>();
            Serializer.InitializeForType<Test1>();
            PSerializer.PrepareSerializer<Test>();
            PSerializer.PrepareSerializer<Test1>();
            BenchmarkRunner.Run<TestClass>();
            Console.ReadKey();
        }
    }

    [AllStatisticsColumn, MemoryDiagnoser]
    public class TestClass
    {
        public readonly Test m_Test;
        private readonly NativeBuffer m_Buffer;
        private readonly MemoryStream m_Ms;

        public TestClass()
        {
            m_Test = new Test
            {
                StringValue = "Hello World",
                Int32Value = -667851,
                ListValue = new List<Test1>()
                {
                    new Test1
                    {
                        FloatValue = 1248796f,
                        DictValue = new Dictionary<string, int>(10)
                        {
                            ["A"] = 1,
                            ["B"] = 2,
                            ["C"] = 3,
                            ["D"] = 4,
                            ["E"] = 5,
                        }
                    },
                    new Test1
                    {
                        FloatValue = 1248796f,
                        DictValue = new Dictionary<string, int>(10)
                        {
                            ["A"] = 1,
                            ["B"] = 2,
                            ["C"] = 3,
                            ["D"] = 4,
                            ["E"] = 5,
                        }
                    },
                    new Test1
                    {
                        FloatValue = 1248796f,
                        DictValue = new Dictionary<string, int>(10)
                        {
                            ["A"] = 1,
                            ["B"] = 2,
                            ["C"] = 3,
                            ["D"] = 4,
                            ["E"] = 5,
                        }
                    },
                    new Test1
                    {
                        FloatValue = 1248796f,
                        DictValue = new Dictionary<string, int>(10)
                        {
                            ["A"] = 1,
                            ["B"] = 2,
                            ["C"] = 3,
                            ["D"] = 4,
                            ["E"] = 5,
                        }
                    },
                    new Test1
                    {
                        FloatValue = 1248796f,
                        DictValue = new Dictionary<string, int>(10)
                        {
                            ["A"] = 1,
                            ["B"] = 2,
                            ["C"] = 3,
                            ["D"] = 4,
                            ["E"] = 5,
                        }
                    }
                }
            };

            m_Buffer = Serializer.Serialize<Test>(m_Test, Encoding.ASCII, Endian.LittleEndian);
            m_Ms = new MemoryStream();
            PSerializer.Serialize<Test>(m_Ms, m_Test);
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
            Serializer.Serialize<Test>(m_Test, Encoding.Unicode, Endian.LittleEndian).Dispose();
        }

        [Benchmark]
        public void AccelbufferDeserialize()
        {
            Serializer.Deserialize<Test>(m_Buffer, 0, m_Buffer.Length);
        }

        [Benchmark]
        public void ProtobufSerialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PSerializer.Serialize<Test>(ms, m_Test);
            }
        }

        [Benchmark]
        public void ProtobufDeserialize()
        {
            m_Ms.Seek(0, SeekOrigin.Begin);
            PSerializer.Deserialize<Test>(m_Ms);
        }
    }
}

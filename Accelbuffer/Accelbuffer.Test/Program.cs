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
            Serializer.PrepareSerializer<Test>();
            Serializer.PrepareSerializer<Test1>();

            PSerializer.PrepareSerializer<Test>();
            PSerializer.PrepareSerializer<Test1>();

            BenchmarkRunner.Run<TestClass>();
            Console.ReadKey();
        }
    }

    [AllStatisticsColumn, MemoryDiagnoser]
    public class TestClass
    {
        private static readonly Test s_Test;

        static TestClass()
        {
            s_Test = new Test
            {
                StringValue = "hello world!",
                Int32Value = -667851,
                ListValue = new List<Test1>()
                {
                    new Test1
                    {
                        FloatValue = 1248796f,
                        DictValue = new Dictionary<string, int>()
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
                        DictValue = new Dictionary<string, int>()
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
                        DictValue = new Dictionary<string, int>()
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
                        DictValue = new Dictionary<string, int>()
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
                        DictValue = new Dictionary<string, int>()
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
        }

        [Benchmark]
        public void AccelbufferSerializeAndDeserialize()
        {
            byte[] bytes = Serializer.Serialize<Test>(s_Test);
            Serializer.Deserialize<Test>(bytes, 0, bytes.Length);
        }

        [Benchmark]
        public void ProtobufDeserializeAndDeserialize()
        {
            MemoryStream ms = new MemoryStream();
            PSerializer.Serialize<Test>(ms, s_Test);
            ms.Seek(0, SeekOrigin.Begin);
            PSerializer.Deserialize<Test>(ms);
            ms.Dispose();
        }
    }
}

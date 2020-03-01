using Accelbuffer.Injection;
using System;
using System.Collections.Generic;

namespace Accelbuffer.Test
{
    partial struct FieldTypeTest : IEquatable<FieldTypeTest>
    {
        partial void OnBeforeSerialization()
        {
            Console.WriteLine("OnBeforeSerialization");
        }

        partial void OnAfterDeserialization()
        {
            Console.WriteLine("OnAfterDeserialization");
        }

        public bool Equals(FieldTypeTest other)
        {
            return Field0 == other.Field0 &&
            Field1 == other.Field1 &&
            Field2 == other.Field2 &&
            Field3 == other.Field3 &&
            Field4 == other.Field4 &&
            Field5 == other.Field5 &&
            Field6 == other.Field6 &&
            Field7 == other.Field7 &&
            Field8 == other.Field8 &&
            Field9 == other.Field9 &&
            Field10 == other.Field10 &&
            Field11 == other.Field11 &&
            Field12 == other.Field12 &&
            Field13 == other.Field13 &&
            Field14 == other.Field14 &&
            Field15 == other.Field15 &&
            Field16 == other.Field16 &&
            Field17 == other.Field17 &&
            Field18.Length == other.Field18.Length &&
            Field19.Count == other.Field19.Count &&
            Field20.Count == other.Field20.Count &&
            Field21.Key == other.Field21.Key &&
            Field21.Value == other.Field21.Value &&
            Field22 == other.Field22 &&
            Field23 == other.Field23 &&
            Field24 == other.Field24 &&
            Field25 == other.Field25 &&
            Field26 == other.Field26 &&
            Field27 == other.Field27 &&
            Field28 == other.Field28;
        }

        public bool Equals(FieldTypeTestRuntime other)
        {
            return Field0 == other.Field0 &&
            Field1 == other.Field1 &&
            Field2 == other.Field2 &&
            Field3 == other.Field3 &&
            Field4 == other.Field4 &&
            Field5 == other.Field5 &&
            Field6 == other.Field6 &&
            Field7 == other.Field7 &&
            Field8 == other.Field8 &&
            Field9 == other.Field9 &&
            Field10 == other.Field10 &&
            Field11 == other.Field11 &&
            Field12 == other.Field12 &&
            Field13 == other.Field13 &&
            Field14 == other.Field14 &&
            Field15 == other.Field15 &&
            Field16 == other.Field16 &&
            Field17 == other.Field17 &&
            Field18.Length == other.Field18.Length &&
            Field19.Count == other.Field19.Count &&
            Field20.Count == other.Field20.Count &&
            Field21.Key == other.Field21.Key &&
            Field21.Value == other.Field21.Value &&
            Field22 == other.Field22 &&
            Field23 == other.Field23 &&
            Field24 == other.Field24 &&
            Field25 == other.Field25 &&
            Field26 == other.Field26 &&
            Field27 == other.Field27 &&
            Field28 == other.Field28;
        }
    }

    partial struct FieldTypeTestRuntime : IEquatable<FieldTypeTestRuntime>
    {
        [OnBeforeSerialization]
        public void OnBeforeSerialization()
        {
            Console.WriteLine("OnBeforeSerialization");
        }

        [OnAfterDeserialization]
        public void OnAfterDeserialization()
        {
            Console.WriteLine("OnAfterDeserialization");
        }

        public bool Equals(FieldTypeTestRuntime other)
        {
            return Field0 == other.Field0 &&
            Field1 == other.Field1 &&
            Field2 == other.Field2 &&
            Field3 == other.Field3 &&
            Field4 == other.Field4 &&
            Field5 == other.Field5 &&
            Field6 == other.Field6 &&
            Field7 == other.Field7 &&
            Field8 == other.Field8 &&
            Field9 == other.Field9 &&
            Field10 == other.Field10 &&
            Field11 == other.Field11 &&
            Field12 == other.Field12 &&
            Field13 == other.Field13 &&
            Field14 == other.Field14 &&
            Field15 == other.Field15 &&
            Field16 == other.Field16 &&
            Field17 == other.Field17 &&
            Field18.Length == other.Field18.Length &&
            Field19.Count == other.Field19.Count &&
            Field20.Count == other.Field20.Count &&
            Field21.Key == other.Field21.Key &&
            Field21.Value == other.Field21.Value &&
            Field22 == other.Field22 &&
            Field23 == other.Field23 &&
            Field24 == other.Field24 &&
            Field25 == other.Field25 &&
            Field26 == other.Field26 &&
            Field27 == other.Field27 &&
            Field28 == other.Field28;
        }
    }

    internal static class FieldTypeTestProgram
    {
        public static void ProgramMain()
        {
            Serializer.AddCallback((ref FieldTypeTest obj) => Console.WriteLine("Before"), (ref FieldTypeTest obj) => Console.WriteLine("After"));
            Serializer.AddCallback((ref FieldTypeTestRuntime obj) => Console.WriteLine("Before_R"), (ref FieldTypeTestRuntime obj) => Console.WriteLine("After_R"));

            Console.WriteLine("Gen");

            Guid guid = Guid.NewGuid();
            TimeSpan time = DateTime.Now.TimeOfDay;
            DateTime date = DateTime.Now;
            DateTimeOffset offset = DateTimeOffset.Now;

            FieldTypeTest test = new FieldTypeTest
            {
                Field0 = 88,
                Field1 = 88,
                Field2 = 888,
                Field3 = 888,
                Field4 = 8888,
                Field5 = 8888,
                Field6 = 88888,
                Field7 = 88888,
                Field8 = true,
                Field9 = -111.1f,
                Field10 = -111.1,
                Field11 = -111.1M,
                Field12 = (IntPtr)88888,
                Field13 = (UIntPtr)88888,
                Field14 = -66666,
                Field15 = 66666,
                Field16 = 'C',
                Field17 = "C#",
                Field18 = new VInt[] { -66666, -66666, -66666, -66666, -66666, -66666 },
                Field19 = new Dictionary<string, float>() { ["A"] = 10.5f, ["B"] = 10.5f, ["C"] = -10.5f },
                Field20 = new List<bool>() { true, true, true },
                Field21 = new KeyValuePair<bool, bool>(true, false),
                Field22 = null,
                Field23 = typeof(FieldTypeTest),
                Field24 = guid,
                Field25 = time,
                Field26 = date,
                Field27 = offset,
                Field28 = TestEnum.Value3
            };

            var buffer0 = test.WriteToBuffer(Encoding.ASCII, Endian.LittleEndian);
            var result0 = Serializer.Deserialize<FieldTypeTest>(buffer0, 0, buffer0.Length);
            buffer0.Dispose();


            var buffer1 = test.WriteToBuffer(Encoding.ASCII, Endian.BigEndian);
            var result1 = Serializer.Deserialize<FieldTypeTest>(buffer1, 0, buffer1.Length);
            buffer1.Dispose();

            var buffer2 = test.WriteToBuffer(Encoding.Unicode, Endian.LittleEndian);
            var result2 = Serializer.Deserialize<FieldTypeTest>(buffer2, 0, buffer2.Length);
            buffer2.Dispose();

            var buffer3 = test.WriteToBuffer(Encoding.Unicode, Endian.BigEndian);
            var result3 = Serializer.Deserialize<FieldTypeTest>(buffer3, 0, buffer3.Length);
            buffer3.Dispose();

            var buffer4 = test.WriteToBuffer(Encoding.UTF8, Endian.LittleEndian);
            var result4 = Serializer.Deserialize<FieldTypeTest>(buffer4, 0, buffer4.Length);
            buffer4.Dispose();

            var buffer5 = test.WriteToBuffer(Encoding.UTF8, Endian.BigEndian);
            var result5 = Serializer.Deserialize<FieldTypeTest>(buffer5, 0, buffer5.Length);
            buffer5.Dispose();

            Console.WriteLine(test.Equals(result0));
            Console.WriteLine(test.Equals(result1));
            Console.WriteLine(test.Equals(result2));
            Console.WriteLine(test.Equals(result3));
            Console.WriteLine(test.Equals(result4));
            Console.WriteLine(test.Equals(result5));

            Console.WriteLine();
            Console.WriteLine("Runtime");

            FieldTypeTestRuntime test1 = new FieldTypeTestRuntime
            {
                Field0 = 88,
                Field1 = 88,
                Field2 = 888,
                Field3 = 888,
                Field4 = 8888,
                Field5 = 8888,
                Field6 = 88888,
                Field7 = 88888,
                Field8 = true,
                Field9 = -111.1f,
                Field10 = -111.1,
                Field11 = -111.1M,
                Field12 = (IntPtr)88888,
                Field13 = (UIntPtr)88888,
                Field14 = -66666,
                Field15 = 66666,
                Field16 = 'C',
                Field17 = "C#",
                Field18 = new VInt[] { -66666, -66666, -66666, -66666, -66666, -66666 },
                Field19 = new Dictionary<string, float>() { ["A"] = 10.5f, ["B"] = 10.5f, ["C"] = -10.5f },
                Field20 = new List<bool>() { true, true, true },
                Field21 = new KeyValuePair<bool, bool>(true, false),
                Field22 = null,
                Field23 = typeof(FieldTypeTest),
                Field24 = guid,
                Field25 = time,
                Field26 = date,
                Field27 = offset,
                Field28 = TestEnum.Value3,
                ClassData = "C# String -> Object",
                StructData = 123
            };
            
            var buffer10 = Serializer.Serialize(test1, Encoding.ASCII, Endian.LittleEndian);
            var result10 = Serializer.Deserialize<FieldTypeTestRuntime>(buffer10, 0, buffer10.Length);
            buffer10.Dispose();


            var buffer11 = Serializer.Serialize(test1, Encoding.ASCII, Endian.BigEndian);
            var result11 = Serializer.Deserialize<FieldTypeTestRuntime>(buffer11, 0, buffer11.Length);
            buffer11.Dispose();

            var buffer12 = Serializer.Serialize(test1, Encoding.Unicode, Endian.LittleEndian);
            var result12 = Serializer.Deserialize<FieldTypeTestRuntime>(buffer12, 0, buffer12.Length);
            buffer12.Dispose();

            var buffer13 = Serializer.Serialize(test1, Encoding.Unicode, Endian.BigEndian);
            var result13 = Serializer.Deserialize<FieldTypeTestRuntime>(buffer13, 0, buffer13.Length);
            buffer13.Dispose();

            var buffer14 = Serializer.Serialize(test1, Encoding.UTF8, Endian.LittleEndian);
            var result14 = Serializer.Deserialize<FieldTypeTestRuntime>(buffer14, 0, buffer14.Length);
            buffer14.Dispose();

            var buffer15 = Serializer.Serialize(test1, Encoding.UTF8, Endian.BigEndian);
            var result15 = Serializer.Deserialize<FieldTypeTestRuntime>(buffer15, 0, buffer15.Length);
            buffer15.Dispose();

            Console.WriteLine(test1.Equals(result10));
            Console.WriteLine(test1.Equals(result11));
            Console.WriteLine(test1.Equals(result12));
            Console.WriteLine(test1.Equals(result13));
            Console.WriteLine(test1.Equals(result14));
            Console.WriteLine(test1.Equals(result15));

            Console.WriteLine();

            Console.WriteLine(test.Equals(result10));
            Console.WriteLine(test.Equals(result11));
            Console.WriteLine(test.Equals(result12));
            Console.WriteLine(test.Equals(result13));
            Console.WriteLine(test.Equals(result14));
            Console.WriteLine(test.Equals(result15));
        }
    }
}

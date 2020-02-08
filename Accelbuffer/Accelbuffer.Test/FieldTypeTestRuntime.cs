namespace Accelbuffer.Test
{
    using Accelbuffer.Injection;
    using Accelbuffer.Memory;
    using System;
    using System.Collections.Generic;
    using boolean = System.Boolean;
    using float128 = System.Decimal;
    using float32 = System.Single;
    using float64 = System.Double;
    using int16 = System.Int16;
    using int32 = System.Int32;
    using int64 = System.Int64;
    using int8 = System.SByte;
    using intptr = System.IntPtr;
    using uint16 = System.UInt16;
    using uint32 = System.UInt32;
    using uint64 = System.UInt64;
    using uint8 = System.Byte;
    using uintptr = System.UIntPtr;
    using vint = Accelbuffer.VInt;
    using vuint = Accelbuffer.VUInt;


    [MemorySize(256)]
    public partial struct FieldTypeTestRuntime
    {
        
        [NeverNull()]
        [FieldIndex(1)]
        public int8 Field0;
        
        [NeverNull()]
        [FieldIndex(2)]
        public uint8 Field1;
        
        [NeverNull()]
        [FieldIndex(3)]
        public int16 Field2;
        
        [NeverNull()]
        [FieldIndex(4)]
        public uint16 Field3;
        
        [NeverNull()]
        [FieldIndex(5)]
        public int32 Field4;
        
        [NeverNull()]
        [FieldIndex(6)]
        public uint32 Field5;
        
        [NeverNull()]
        [FieldIndex(7)]
        public int64 Field6;
        
        [NeverNull()]
        [FieldIndex(8)]
        public uint64 Field7;
        
        [NeverNull()]
        [FieldIndex(9)]
        public boolean Field8;
        
        [NeverNull()]
        [FieldIndex(10)]
        public float32 Field9;
        
        [NeverNull()]
        [FieldIndex(11)]
        public float64 Field10;
        
        [NeverNull()]
        [FieldIndex(12)]
        public float128 Field11;
        
        [NeverNull()]
        [FieldIndex(13)]
        public intptr Field12;
        
        [NeverNull()]
        [FieldIndex(14)]
        public uintptr Field13;
        
        [NeverNull()]
        [FieldIndex(15)]
        public vint Field14;
        
        [NeverNull()]
        [FieldIndex(16)]
        public vuint Field15;
        
        [NeverNull()]
        [FieldIndex(17)]
        public char Field16;
        
        [FieldIndex(18)]
        public string Field17;
        
        [FieldIndex(19)]
        public vint[] Field18;
        
        [FieldIndex(20)]
        public Dictionary<string,float32> Field19;
        
        [FieldIndex(21)]
        public List<boolean> Field20;
        
        [NeverNull()]
        [FieldIndex(22)]
        public KeyValuePair<boolean,boolean> Field21;
        
        [FieldIndex(23)]
        public vint? Field22;
        
        [FieldIndex(24)]
        public Type Field23;
        
        [NeverNull()]
        [FieldIndex(25)]
        public Guid Field24;
        
        [NeverNull()]
        [FieldIndex(26)]
        public TimeSpan Field25;
        
        [NeverNull()]
        [FieldIndex(27)]
        public DateTime Field26;
        
        [NeverNull()]
        [FieldIndex(28)]
        public DateTimeOffset Field27;
    }
}

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
        [SerialIndex(1)]
        public int8 Field0;
        
        [NeverNull()]
        [SerialIndex(2)]
        public uint8 Field1;
        
        [NeverNull()]
        [SerialIndex(3)]
        public int16 Field2;
        
        [NeverNull()]
        [SerialIndex(4)]
        public uint16 Field3;
        
        [NeverNull()]
        [SerialIndex(5)]
        public int32 Field4;
        
        [NeverNull()]
        [SerialIndex(6)]
        public uint32 Field5;
        
        [NeverNull()]
        [SerialIndex(7)]
        public int64 Field6;
        
        [NeverNull()]
        [SerialIndex(8)]
        public uint64 Field7;
        
        [NeverNull()]
        [SerialIndex(9)]
        public boolean Field8;
        
        [NeverNull()]
        [SerialIndex(10)]
        public float32 Field9;
        
        [NeverNull()]
        [SerialIndex(11)]
        public float64 Field10;
        
        [NeverNull()]
        [SerialIndex(12)]
        public float128 Field11;
        
        [NeverNull()]
        [SerialIndex(13)]
        public intptr Field12;
        
        [NeverNull()]
        [SerialIndex(14)]
        public uintptr Field13;
        
        [NeverNull()]
        [SerialIndex(15)]
        public vint Field14;
        
        [NeverNull()]
        [SerialIndex(16)]
        public vuint Field15;
        
        [NeverNull()]
        [SerialIndex(17)]
        public char Field16;
        
        [SerialIndex(18)]
        public string Field17;
        
        [SerialIndex(19)]
        public vint[] Field18;
        
        [SerialIndex(20)]
        public Dictionary<string,float32> Field19;
        
        [SerialIndex(21)]
        public List<boolean> Field20;
        
        [NeverNull()]
        [SerialIndex(22)]
        public KeyValuePair<boolean,boolean> Field21;
        
        [SerialIndex(23)]
        public vint? Field22;
        
        [SerialIndex(24)]
        public Type Field23;
        
        [NeverNull()]
        [SerialIndex(25)]
        public Guid Field24;
        
        [NeverNull()]
        [SerialIndex(26)]
        public TimeSpan Field25;
        
        [NeverNull()]
        [SerialIndex(27)]
        public DateTime Field26;
        
        [NeverNull()]
        [SerialIndex(28)]
        public DateTimeOffset Field27;

        [NeverNull()]
        [SerialIndex(29)]
        public TestEnum Field28;

        [SerialIndex(30)]
        [FacadeType(typeof(string))]
        public object ClassData;//extra

        [SerialIndex(31)]
        [FacadeType(typeof(object))]
        public int StructData;//extra
    }
}

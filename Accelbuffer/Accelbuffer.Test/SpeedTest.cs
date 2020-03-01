namespace Accelbuffer.Test
{
    using Accelbuffer;
    using Accelbuffer.Injection;
    using Accelbuffer.Memory;
    using ProtoBuf;
    using System.Collections.Generic;
    using boolean = System.Boolean;
    using float32 = System.Single;
    using int32 = System.Int32;

    [MemorySize(256)]
    [ProtoContract]
    [SerializeBy(typeof(SpeedTestSerializer))]
    public partial struct SpeedTest
    {
        [ProtoMember(1)]
        [SerialIndex(1), NeverNull]
        public int32 FixedIntValue;

        [ProtoMember(2)]
        [SerialIndex(2)]
        public string StrValue;

        [ProtoMember(3)]
        [SerialIndex(3)]
        public Nested[] ArrayValue;

        [ProtoMember(4)]
        [SerialIndex(4)]
        public Dictionary<string, char> DictValue;

        public sealed class SpeedTestSerializer : ITypeSerializer<SpeedTest>
        {
            public void Serialize(SpeedTest obj, ref AccelWriter writer)
            {
                writer.WriteValue(1, obj.FixedIntValue);

                if ((obj.StrValue != null))
                {
                    writer.WriteValue(2, obj.StrValue);
                }

                if ((obj.ArrayValue != null))
                {
                    writer.WriteValue(3, obj.ArrayValue);
                }

                if ((obj.DictValue != null))
                {
                    writer.WriteValue(4, obj.DictValue);
                }
            }

            public SpeedTest Deserialize(ref AccelReader reader)
            {
                SpeedTest result = new SpeedTest();

                while (reader.HasNext(out int index))
                {
                    switch (index)
                    {
                        case 1:
                            result.FixedIntValue = reader.ReadInt32();
                            break;
                        case 2:
                            result.StrValue = reader.ReadString();
                            break;
                        case 3:
                            result.ArrayValue = reader.ReadGeneric<Nested[]>();
                            break;
                        case 4:
                            result.DictValue = reader.ReadGeneric<Dictionary<string, char>>();
                            break;
                        default:
                            reader.SkipNext();
                            break;
                    }
                }

                return result;
            }
        }

        [MemorySize(8)]
        [ProtoContract]
        [SerializeBy(typeof(NestedSerializer))]
        public partial struct Nested
        { 
            [ProtoMember(1)]
            [SerialIndex(1), NeverNull]
            public boolean BooleanValue;

            [ProtoMember(2)]
            [SerialIndex(2), NeverNull]
            public float32 FloatValue;

            public sealed class NestedSerializer : object, ITypeSerializer<Nested>
            {
                public void Serialize(Nested obj, ref AccelWriter writer)
                {
                    writer.WriteValue(1, obj.BooleanValue);
                    writer.WriteValue(2, obj.FloatValue);
                }

                public Nested Deserialize(ref AccelReader reader)
                {
                    Nested result = new Nested();

                    while (reader.HasNext(out int index))
                    {
                        switch (index)
                        {
                            case 1:
                                result.BooleanValue = reader.ReadBoolean();
                                break;
                            case 2:
                                result.FloatValue = reader.ReadFloat32();
                                break;
                            default:
                                reader.SkipNext();
                                break;
                        }
                    }

                    return result;
                }
            }
        }
    }
}

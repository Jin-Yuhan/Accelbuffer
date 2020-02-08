using Accelbuffer.Memory;
using System;

namespace Accelbuffer.Injection
{
    internal unsafe sealed class OtherTypeSerializer :
        IMemorySizeForType<Guid>,
        IMemorySizeForType<TimeSpan>,
        IMemorySizeForType<DateTime>,
        IMemorySizeForType<DateTimeOffset>,
        ITypeSerializer<Guid>,
        ITypeSerializer<TimeSpan>,
        ITypeSerializer<DateTime>,
        ITypeSerializer<DateTimeOffset>
    {
        int IMemorySizeForType<Guid>.ApproximateMemorySize => 16;

        int IMemorySizeForType<TimeSpan>.ApproximateMemorySize => 8;

        int IMemorySizeForType<DateTime>.ApproximateMemorySize => 8;

        int IMemorySizeForType<DateTimeOffset>.ApproximateMemorySize => 12;

        Guid ITypeSerializer<Guid>.Deserialize(ref AccelReader reader)
        {
            if (!reader.HasNext())
            {
                return default;
            }

            decimal value = reader.ReadFloat128();
            return *(Guid*)&value;
        }

        TimeSpan ITypeSerializer<TimeSpan>.Deserialize(ref AccelReader reader)
        {
            if (!reader.HasNext())
            {
                return default;
            }
            long ticks = reader.ReadInt64();
            return new TimeSpan(ticks);
        }

        DateTime ITypeSerializer<DateTime>.Deserialize(ref AccelReader reader)
        {
            if (!reader.HasNext())
            {
                return default;
            }

            ulong value = reader.ReadUInt64();
            return *(DateTime*)&value;
        }

        DateTimeOffset ITypeSerializer<DateTimeOffset>.Deserialize(ref AccelReader reader)
        {
            DateTimeOffset result = new DateTimeOffset();
            ulong* dateTimePtr = (ulong*)&result;

            if (!reader.HasNext())
            {
                return result;
            }

            *dateTimePtr = reader.ReadUInt64();

            if (!reader.HasNext())
            {
                return result;
            }

            *(int*)(dateTimePtr + 1) = reader.ReadInt32();

            return result;
        }

        void ITypeSerializer<Guid>.Serialize(Guid obj, ref AccelWriter writer)
        {
            decimal value = *(decimal*)&obj;
            writer.WriteValue(1, value);
        }

        void ITypeSerializer<TimeSpan>.Serialize(TimeSpan obj, ref AccelWriter writer)
        {
            writer.WriteValue(1, obj.Ticks);
        }

        void ITypeSerializer<DateTime>.Serialize(DateTime obj, ref AccelWriter writer)
        {
            ulong data = *(ulong*)&obj;
            writer.WriteValue(1, data);
        }

        void ITypeSerializer<DateTimeOffset>.Serialize(DateTimeOffset obj, ref AccelWriter writer)
        {
            ulong* dateTimePtr = (ulong*)&obj;
            ulong data = *dateTimePtr;
            int offset = *(int*)(dateTimePtr + 1);
            writer.WriteValue(1, data);
            writer.WriteValue(2, offset);
        }
    }
}

using Accelbuffer.Compiling.Declarations;
using Accelbuffer.Memory;

namespace Accelbuffer.Compiling
{
    internal sealed class DeclarationArraySerializer : ITypeSerializer<DeclarationArray>, IMemorySizeForType<DeclarationArray>
    {
        int IMemorySizeForType<DeclarationArray>.ApproximateMemorySize => 561;

        private enum DeclarationType : byte
        {
            Field = 1,
            Package = 2,
            Struct = 3,
            UsingAs = 4,
            Using = 5
        }

        DeclarationArray ITypeSerializer<DeclarationArray>.Deserialize(ref AccelReader reader)
        {
            if (!reader.HasNext(out int id))
            {
                return default;
            }
            System.Console.WriteLine(id);
            int len = (int)reader.ReadVariantUInt();
            IDeclaration[] declarations = new IDeclaration[len];

            for (int i = 0; i < len; i++)
            {
                if (!reader.HasNext(out int index))
                {
                    declarations[i] = null;
                    break;
                }
                System.Console.WriteLine(index);
                DeclarationType type = (DeclarationType)reader.ReadUInt8();

                if (!reader.HasNext())
                {
                    declarations[i] = null;
                    break;
                }

                switch (type)
                {
                    case DeclarationType.Field:
                        declarations[i] = reader.ReadGeneric<FieldDeclaration>();
                        break;
                    case DeclarationType.Package:
                        declarations[i] = reader.ReadGeneric<PackageDeclaration>();
                        break;
                    case DeclarationType.Struct:
                        declarations[i] = reader.ReadGeneric<StructDeclaration>();
                        break;
                    case DeclarationType.UsingAs:
                        declarations[i] = reader.ReadGeneric<UsingAsDeclaration>();
                        break;
                    case DeclarationType.Using:
                        declarations[i] = reader.ReadGeneric<UsingDeclaration>();
                        break;
                    default:
                        reader.SkipNext();
                        break;
                }
            }

            return new DeclarationArray { Declarations = declarations };
        }

        void ITypeSerializer<DeclarationArray>.Serialize(DeclarationArray obj, ref AccelWriter writer)
        {
            if (obj.Declarations == null)
            {
                return;
            }

            writer.WriteValue(1, (VUInt)obj.Declarations.Length);

            for (int i = 0; i < obj.Declarations.Length; i++)
            {
                IDeclaration declaration = obj.Declarations[i];

                switch (declaration)
                {
                    case FieldDeclaration field:
                        writer.WriteValue(2, (byte)DeclarationType.Field);
                        writer.WriteValue<FieldDeclaration>(3, field);
                        break;
                    case PackageDeclaration package:
                        writer.WriteValue(2, (byte)DeclarationType.Package);
                        writer.WriteValue<PackageDeclaration>(3, package);
                        break;
                    case StructDeclaration @struct:
                        writer.WriteValue(2, (byte)DeclarationType.Struct);
                        writer.WriteValue<StructDeclaration>(3, @struct);
                        break;
                    case UsingAsDeclaration usingAs:
                        writer.WriteValue(2, (byte)DeclarationType.UsingAs);
                        writer.WriteValue<UsingAsDeclaration>(3, usingAs);
                        break;
                    case UsingDeclaration @using:
                        writer.WriteValue(2, (byte)DeclarationType.Using);
                        writer.WriteValue<UsingDeclaration>(3, @using);
                        break;
                }
            }
        }
    }
}

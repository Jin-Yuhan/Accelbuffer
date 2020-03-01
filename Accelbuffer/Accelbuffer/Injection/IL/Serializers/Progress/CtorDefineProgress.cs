using Accelbuffer.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Injection.IL.Serializers.Progress
{
    internal sealed class CtorDefineProgress : SerializerGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, IEnumerable<AccelFieldInfo> fields, int fieldCount, bool hasContinuousSerialIndexes, MethodInfo beforeMethod, MethodInfo afterMethod)
        {
            builder.DefineDefaultConstructor(MethodAttributes.Public);
        }
    }
}

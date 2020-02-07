using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Injection
{
    internal sealed class CtorDefineProgress : SerializerGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, MethodInfo beforeMethod, MethodInfo afterMethod)
        {
            builder.DefineDefaultConstructor(MethodAttributes.Public);
        }
    }
}

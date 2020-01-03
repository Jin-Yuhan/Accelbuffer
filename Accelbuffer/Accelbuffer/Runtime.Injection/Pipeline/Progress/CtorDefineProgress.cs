using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Runtime.Injection
{
    internal sealed class CtorDefineProgress : ProxyGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, List<MethodData> methods)
        {
            builder.DefineDefaultConstructor(MethodAttributes.Public);
        }
    }
}

using System;
using UnityEngine;

namespace Accelbuffer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class VIntRangeAttribute : PropertyAttribute
    {
        public int Min { get; }

        public int Max { get; }

        public VIntRangeAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}

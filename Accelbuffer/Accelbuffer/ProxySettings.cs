using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelbuffer
{
    internal readonly struct ProxySettings
    {
        public readonly Type ProxyType;
        public readonly long InitialBufferSize;
        public readonly bool StrictMode;

        public ProxySettings(Type proxyType, long initialBufferSize, bool strictMode)
        {
            ProxyType = proxyType;
            InitialBufferSize = initialBufferSize;
            StrictMode = strictMode;
        }
    }
}

using System.Collections.Generic;

namespace Juno
{
    internal struct TypeInjectInfo
    {
        public TypeInjectInfo( List<MethodInjectInfo> methodInfo )
        {
            MethodInfo = methodInfo;
        }

        public List<MethodInjectInfo> MethodInfo
        {
            get;
            private set;
        }
    }
}
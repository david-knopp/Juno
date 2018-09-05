using System.Collections.Generic;
using System.Reflection;

namespace Juno
{
    internal struct MethodInjectInfo
    {
        public MethodInjectInfo( MethodInfo methodInfo, List<ParamInjectInfo> paramInfo )
        {
            MethodInfo = methodInfo;
            ParamInfo = paramInfo;
        }

        public MethodInfo MethodInfo
        {
            get;
            private set;
        }

        public List<ParamInjectInfo> ParamInfo
        {
            get;
            private set;
        }
    }
}
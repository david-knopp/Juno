using System;
using System.Collections.Generic;

namespace Juno
{
    internal struct TypeInjectInfo
    {
        public TypeInjectInfo( List<MethodInjectInfo> methodInfo,
                               List<FieldInjectInfo> fieldInfo )
        {
            MethodInfo = methodInfo;
            FieldInfo = fieldInfo;
        }

        public List<MethodInjectInfo> MethodInfo
        {
            get;
            private set;
        }

        public List<FieldInjectInfo> FieldInfo
        {
            get;
            private set;
        }
    }
}
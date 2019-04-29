using System;
using System.Reflection;

namespace Juno
{
    internal struct FieldInjectInfo
    {
        public FieldInjectInfo( Type type, FieldInfo fieldInfo, int? id, bool isOptional )
        {
            Type = type;
            FieldInfo = fieldInfo;
            ID = id;
            IsOptional = isOptional;
        }

        public FieldInjectInfo( Type type, FieldInfo fieldInfo, bool isOptional )
        {
            Type = type;
            FieldInfo = fieldInfo;
            ID = null;
            IsOptional = isOptional;
        }

        public Type Type
        {
            get;
            private set;
        }

        public int? ID
        {
            get;
            private set;
        }

        public bool IsAnonymous
        {
            get
            {
                return ID.HasValue == false;
            }
        }

        public FieldInfo FieldInfo
        {

            get;
            private set;
        }

        public bool IsOptional
        {
            get;
            private set;
        }
    }
}

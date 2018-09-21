using System;

namespace Juno
{
    internal struct ParamInjectInfo
    {
        public ParamInjectInfo( Type type, int? id, bool isOptional )
        {
            Type = type;
            ID = id;
            IsOptional = isOptional;
        }

        public ParamInjectInfo( Type type, bool isOptional )
        {
            Type = type;
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
            get { return ID.HasValue; }
        }

        public bool IsOptional
        {
            get;
            private set;
        }
    }
}
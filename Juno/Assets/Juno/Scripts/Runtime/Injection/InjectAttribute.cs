using System;
using UnityEngine.Scripting;

namespace Juno
{
    [AttributeUsage( validOn: AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Field, 
                     AllowMultiple = false )]
    public sealed class InjectAttribute : PreserveAttribute
    {
        public int ID
        {
            get
            {
                return m_id;
            }

            set
            {
                HasID = true;
                m_id = value;
            }
        }

        public bool HasID
        {
            get;
            private set;
        }

        public bool IsOptional
        {
            get;
            set;
        }

        private int m_id;
    }
}
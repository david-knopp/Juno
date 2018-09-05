using System;
using UnityEngine.Scripting;

namespace Juno
{
    [AttributeUsage( validOn: AttributeTargets.Method | AttributeTargets.Parameter, 
                     AllowMultiple = false )]
    public sealed class InjectAttribute : PreserveAttribute
    {
        public int? Id
        {
            get;
            set;
        }

        public bool IsOptional
        {
            get;
            set;
        }
    }
}
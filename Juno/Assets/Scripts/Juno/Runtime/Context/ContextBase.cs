using System.Collections.Generic;
using UnityEngine;

namespace Juno
{
    public abstract class ContextBase : MonoBehaviour
    {
        #region public
        public DIContainer Container
        {
            get;
            private set;
        }
        #endregion // public

        #region protected
        #endregion // protected

        #region private
        [SerializeField] private List<MonoBehaviourInstaller> m_monoBehaviourInstallers;
        [SerializeField] private List<ScriptableObjectInstaller> m_scriptableObjectInstallers;
        #endregion // private
    }
}
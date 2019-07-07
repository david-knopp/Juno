using System;
using System.Collections.Generic;
using UnityEngine;

namespace Juno
{
    public abstract class ContextBase : MonoBehaviour
    {
        #region public
        public virtual DIContainer GetContainer()
        {
            return Container;
        }
        #endregion public

        #region protected
        protected DIContainer Container
        {
            get;
            private set;
        }

        protected virtual void OnPreInject()
        {
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Tick()
        {
        }

        protected virtual void LateTick()
        {
        }

        protected virtual void Dispose()
        {
        }

        protected void InstallBindings<T>( List<T> installers ) where T : IInstaller
        {
            if ( installers != null )
            {
                for ( int i = 0; i < installers.Count; i++ )
                {
                    IInstaller installer = installers[i];
                    if ( installer != null )
                    {
                        installer.InstallBindings( Container );
                    }
                    else
                    {
                        throw new System.NullReferenceException( string.Format( "Null installer reference found in context '{0}'", name ) );
                    }
                } 
            }
        }
        #endregion protected

        #region private
        [SerializeField] private List<MonoBehaviourInstaller> m_monoBehaviourInstallers;
        [SerializeField] private List<ScriptableObjectInstaller> m_scriptableObjectInstallers;
        [Inject( IsOptional = true )] private List<ITickable> m_tickables;
        [Inject( IsOptional = true )] private List<ILateTickable> m_lateTickables;

        private void Awake()
        {
            Container = new DIContainer();
            Container.Bind( Container );

            // run installers
            InstallBindings( m_monoBehaviourInstallers );
            InstallBindings( m_scriptableObjectInstallers );

            OnAwake();
        }

        private void Start()
        {
            // inject
            OnPreInject();
            Container.FlushInjectQueue();

            // run initialize interfaces
            List<IInitializable> initializables;
            if ( Container.TryGetAll( out initializables ) )
            {
                foreach ( var initializable in initializables )
                {
                    initializable.Initialize();
                }
            }

            Initialize();
        }

        private void OnDestroy()
        {
            // run disposable interfaces
            List<IDisposable> disposables;
            if ( Container.TryGetAll( out disposables ) )
            {
                foreach ( var disposable in disposables )
                {
                    disposable.Dispose();
                }
            }

            Dispose();
        }

        private void Update()
        {
            if ( m_tickables != null )
            {
                foreach ( var tickable in m_tickables )
                {
                    tickable.Tick();
                }
            }

            Tick();
        }

        private void LateUpdate()
        {
            if ( m_lateTickables != null )
            {
                foreach ( var tickable in m_lateTickables )
                {
                    tickable.LateTick();
                }
            }

            LateTick();
        }
        #endregion private
    }
}
﻿using UnityEngine;

namespace Juno
{
    public sealed class Factory
    {
        #region public
        public T Create<T>()
        {
            T obj = System.Activator.CreateInstance<T>();
            m_container.Inject( obj );

            return obj;
        }

        public T Create<T>( params object[] args )
        {
            System.Type type = typeof( T );
            T obj = ( T )System.Activator.CreateInstance( type, args );
            m_container.Inject( obj );

            return obj;
        }

        #region GameObject
        public GameObject Clone( GameObject original )
        {
            GameObject obj = Object.Instantiate( original );
            InjectGameObject( obj );

            return obj;
        }

        public GameObject Clone( GameObject original, Transform parent )
        {
            GameObject obj = Object.Instantiate( original, parent );
            InjectGameObject( obj );

            return obj;
        }

        public GameObject Clone( GameObject original, Transform parent, bool worldPositionStays )
        {
            GameObject obj = Object.Instantiate( original, parent, worldPositionStays );
            InjectGameObject( obj );

            return obj;
        }

        public GameObject Clone( GameObject original, Vector3 position, Quaternion rotation )
        {
            GameObject obj = Object.Instantiate( original, position, rotation );
            InjectGameObject( obj );

            return obj;
        }

        public GameObject Clone( GameObject original, Vector3 position, Quaternion rotation, Transform parent )
        {
            GameObject obj = Object.Instantiate( original, position, rotation, parent );
            InjectGameObject( obj );

            return obj;
        }
        #endregion GameObject

        #region MonoBehaviour
        public T Clone<T>( T original ) where T : MonoBehaviour
        {
            T obj = Object.Instantiate( original );
            m_container.Inject( obj.gameObject );

            return obj;
        }

        public T Clone<T>( T original, Transform parent ) where T : MonoBehaviour
        {
            T obj = Object.Instantiate( original, parent );
            m_container.Inject( obj.gameObject );

            return obj;
        }

        public T Clone<T>( T original, Transform parent, bool worldPositionStays ) where T : MonoBehaviour
        {
            T obj = Object.Instantiate( original, parent, worldPositionStays );
            m_container.Inject( obj.gameObject );

            return obj;
        }

        public T Clone<T>( T original, Vector3 position, Quaternion rotation ) where T : MonoBehaviour
        {
            T obj = Object.Instantiate( original, position, rotation );
            m_container.Inject( obj.gameObject );

            return obj;
        }

        public T Clone<T>( T original, Vector3 position, Quaternion rotation, Transform parent ) where T : MonoBehaviour
        {
            T obj = Object.Instantiate( original, position, rotation, parent );
            m_container.Inject( obj.gameObject );

            return obj;
        }
        #endregion MonoBehaviour
        #endregion public

        #region private
        private DIContainer m_container;

        [Inject]
        private void Inject( DIContainer container )
        {
            m_container = container;
        }

        private void InjectGameObject( GameObject obj )
        {
            var injectables = InjectUtils.GetInjectableMonoBehaviours( obj );
            foreach ( MonoBehaviour injectable in injectables )
            {
                m_container.QueueForInject( injectable );
            }

            m_container.FlushInjectQueue();
        }
        #endregion private
    }
}
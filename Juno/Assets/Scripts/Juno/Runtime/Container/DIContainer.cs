using System;
using System.Collections.Generic;

namespace Juno
{
    public class DIContainer
    {
        #region public
        public int AnonymousID
        {
            get;
            set;
        }

        public DIContainer( int anonymousID = int.MinValue )
        {
            m_namedBindings = new Dictionary<Type, Dictionary<int, object>>();
            m_anonymousBindings = new Dictionary<Type, List<object>>();
            m_injectQueue = new HashSet<object>();
            AnonymousID = anonymousID;
        }

        #region binding
        #region anonymous
        public void Bind<T>()
        {
            Type type = typeof( T );
            object instance = Activator.CreateInstance( type );
            Bind( type, instance );
        }

        public void Bind<T>( T instance )
        {
            Bind( typeof( T ), instance );
        }

        public void Bind( Type type, object instance )
        {
            var bindings = GetAnonymousBindings( type );
            bindings.Add( instance );
            QueueForInject( instance );
        }
        #endregion // anonymous

        #region named
        public void Bind<T>( int id )
        {
            Type type = typeof( T );
            object instance = Activator.CreateInstance( type );
            Bind( type, instance, id );
        }

        public void Bind<T>( T instance, int id )
        {
            Bind( typeof( T ), instance, id );
        }

        public void Bind( Type type, object instance, int id )
        {
            var typeBindings = GetNamedTypeBindings( type );
            typeBindings.Add( id, instance );
            QueueForInject( instance );
        }
        #endregion // named
        #endregion // binding

        #region injection
        public void Inject( object obj )
        {

        }

        public void FlushInjectQueue()
        {
            foreach ( object obj in m_injectQueue )
            {
                Inject( obj );
            }
            m_injectQueue.Clear();
        }

        public void QueueForInject( object obj )
        {
            m_injectQueue.Add( obj );
        }
        #endregion // injection
        #endregion // public

        #region private
        private Dictionary<Type, Dictionary<int, object>> m_namedBindings;
        private Dictionary<Type, List<object>> m_anonymousBindings;
        private HashSet<object> m_injectQueue;

        private List<object> GetAnonymousBindings( Type type )
        {
            List<object> bindings;

            if ( m_anonymousBindings.TryGetValue( type, out bindings ) == false )
            {
                bindings = new List<object>();
                m_anonymousBindings.Add( type, bindings );
            }

            return bindings;
        }

        private Dictionary<int, object> GetNamedTypeBindings( Type type )
        {
            Dictionary<int, object> typeBindings;
            if ( m_namedBindings.TryGetValue( type, out typeBindings ) == false )
            {
                typeBindings = new Dictionary<int, object>();
                m_namedBindings.Add( type, typeBindings );
            }

            return typeBindings;
        }
        #endregion // private
    }
}
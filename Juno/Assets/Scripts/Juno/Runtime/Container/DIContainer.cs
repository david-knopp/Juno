using System;
using System.Collections.Generic;
using System.Linq;

namespace Juno
{
    public sealed class DIContainer
    {
        #region public
        public const int c_defaultID = int.MinValue;

        public DIContainer()
        {
            m_bindings = new Dictionary<Type, Dictionary<int, object>>();
            m_injectQueue = new HashSet<object>();
        }
        
        #region binding
        public void Bind<T>( int id = c_defaultID )
        {
            Type type = typeof( T );
            object instance = Activator.CreateInstance( type );
            Bind( type, instance, id );
        }

        public void Bind<T>( T instance, int id = c_defaultID )
        {
            Bind( typeof( T ), instance, id );
        }

        public void Bind( Type type, object instance, int id = c_defaultID )
        {
            var typeBindings = GetBindingsForType( type );
            typeBindings.Add( id, instance );
            QueueForInject( instance );
        }

        public void Unbind<T>( int id = c_defaultID )
        {
            var bindings = GetBindingsForType( typeof( T ) );
            bindings.Remove( id );
        }

        public bool TryGet<T>( out T instance, int id = c_defaultID )
        {
            object objInstance;
            if ( TryGet( typeof( T ), out objInstance, id ) )
            {
                instance = ( T )objInstance;
                return true;
            }
            else
            {
                instance = default( T );
                return false;
            }
        }

        public bool TryGet( Type type, out object instance, int id = c_defaultID )
        {
            var bindings = GetBindingsForType( type );
            return bindings.TryGetValue( id, out instance );
        }

        public T Get<T>( int id = c_defaultID )
        {
            T instance;
            if ( TryGet( out instance, id ) )
            {
                return instance;
            }

            throw new KeyNotFoundException( string.Format( "No bindings exist for type '{0}' with id '{1}'", typeof( T ).FullName, id ) );
        }

        public List<T> GetAll<T>()
        {
            Dictionary<int, object> namedBindings = GetBindingsForType( typeof( T ) );
            return namedBindings.Values.Cast<T>().ToList();
        }
        #endregion // binding

        #region injection
        public void Inject( object obj )
        {
            if ( obj != null )
            {
                Type objType = obj.GetType();
                TypeInjectInfo typeInfo = TypeAnalyzer.GetTypeInjectInfo( objType );

                // inject methods
                foreach ( MethodInjectInfo methodInjectInfo in typeInfo.MethodInfo )
                {
                    // get desired arguments to inject
                    List<object> methodArguments = new List<object>();

                    foreach ( ParamInjectInfo paramInjectInfo in methodInjectInfo.ParamInfo )
                    {
                        object instance;
                        int id = ( paramInjectInfo.ID.HasValue == false )? c_defaultID : paramInjectInfo.ID.Value;

                        if ( TryGet( paramInjectInfo.Type, out instance, id ) )
                        {
                            methodArguments.Add( instance );
                        }
                        else
                        {
                            if ( paramInjectInfo.IsOptional == false )
                            {
                                throw new InvalidOperationException( string.Format( "No binding of type '{0}' with ID '{1}' exists for attempted injection '{2}'",
                                                                                    paramInjectInfo.Type,
                                                                                    id,
                                                                                    methodInjectInfo.MethodInfo.Name ) );
                            }

                            methodArguments.Add( null );
                        }
                    }

                    methodInjectInfo.MethodInfo.Invoke( obj, methodArguments.ToArray() );
                }
            }
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
        private Dictionary<Type, Dictionary<int, object>> m_bindings;
        private HashSet<object> m_injectQueue;

        private Dictionary<int, object> GetBindingsForType( Type type )
        {
            Dictionary<int, object> typeBindings;
            if ( m_bindings.TryGetValue( type, out typeBindings ) == false )
            {
                typeBindings = new Dictionary<int, object>();
                m_bindings.Add( type, typeBindings );
            }

            return typeBindings;
        }
        #endregion // private
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Juno
{
    public sealed class DIContainer
    {
        #region public
        public DIContainer()
        {
            m_namedBindings = new Dictionary<Type, Dictionary<int?, object>>();
            m_injectQueue = new HashSet<object>();
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
            var bindings = GetBindingsForType( type );
            bindings.Add( null, instance );
            QueueForInject( instance );
        }

        public void Unbind<T>()
        {
            var bindings = GetBindingsForType( typeof( T ) );
            bindings.Remove( null );
        }

        public bool TryGet<T>( out T instance )
        {
            object objInstance;
            if ( TryGet( typeof( T ), out objInstance ) )
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

        public bool TryGet( Type type, out object instance )
        {
            Dictionary<int?, object> bindings = GetBindingsForType( type );
            return bindings.TryGetValue( null, out instance );
        }

        public T Get<T>()
        {
            T instance;
            if ( TryGet( out instance ) )
            {
                return instance;
            }

            throw new InvalidOperationException( string.Format( "No anonymous bindings exist for type '{0}'", typeof( T ).FullName ) );
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
            var typeBindings = GetBindingsForType( type );
            typeBindings.Add( id, instance );
            QueueForInject( instance );
        }

        public void Unbind<T>( int id )
        {
            var bindings = GetBindingsForType( typeof( T ) );
            bindings.Remove( id );
        }

        public bool TryGet<T>( int id, out T instance )
        {
            object objInstance;
            if ( TryGet( typeof( T ), id, out objInstance ) )
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

        public bool TryGet( Type type, int id, out object instance )
        {
            var bindings = GetBindingsForType( type );
            return bindings.TryGetValue( id, out instance );
        }

        public T Get<T>( int id )
        {
            T instance;
            if ( TryGet( id, out instance ) )
            {
                return instance;
            }

            throw new KeyNotFoundException( string.Format( "No bindings exist for type '{0}' with id '{1}'", typeof( T ).FullName, id ) );
        }
        #endregion // named

        public List<T> GetAll<T>()
        {
            Dictionary<int?, object> namedBindings = GetBindingsForType( typeof( T ) );
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
                        if ( TryGet( paramInjectInfo.Type, paramInjectInfo.ID.Value, out instance ) )
                        {
                            methodArguments.Add( instance );
                        }
                        else
                        {
                            if ( paramInjectInfo.IsOptional == false )
                            {
                                throw new InvalidOperationException( string.Format( "No binding of type '{0}' with ID '{1}' exists for attempted injection '{2}'",
                                                                                    paramInjectInfo.Type,
                                                                                    paramInjectInfo.ID.Value,
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
        private Dictionary<Type, Dictionary<int?, object>> m_namedBindings;
        private HashSet<object> m_injectQueue;

        private Dictionary<int?, object> GetBindingsForType( Type type )
        {
            Dictionary<int?, object> typeBindings;
            if ( m_namedBindings.TryGetValue( type, out typeBindings ) == false )
            {
                typeBindings = new Dictionary<int?, object>();
                m_namedBindings.Add( type, typeBindings );
            }

            return typeBindings;
        }
        #endregion // private
    }
}
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
            m_namedBindings = new Dictionary<Type, Dictionary<int, object>>();
            m_anonymousBindings = new Dictionary<Type, List<object>>();
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
            var bindings = GetAnonymousBindings( type );
            bindings.Add( instance );
            QueueForInject( instance );
        }

        public void Unbind<T>()
        {
            var bindings = GetAnonymousBindings( typeof( T ) );
            bindings.Clear();
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
            var bindings = GetAnonymousBindings( type );
            if ( bindings.Count > 0 )
            {
                instance = bindings[0];
                return true;
            }
            else
            {
                instance = default( object );
                return false;
            }
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

        public bool TryGet<T>( out List<T> instances )
        {
            List<object> bindings;

            if ( m_anonymousBindings.TryGetValue( typeof( T ), out bindings ) )
            {
                instances = bindings.Cast<T>().ToList();
                return true;
            }

            instances = default( List<T> );
            return false;
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

        public void Unbind<T>( int id )
        {
            var bindings = GetNamedTypeBindings( typeof( T ) );
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
            var bindings = GetNamedTypeBindings( type );
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
            Dictionary<int, object> namedBindings = GetNamedTypeBindings( typeof( T ) );
            List<T> bindings = namedBindings.Values.Cast<T>().ToList();

            List<object> anonymousBindings;
            if ( m_anonymousBindings.TryGetValue( typeof( T ), out anonymousBindings ) )
            {
                bindings.AddRange( anonymousBindings.Cast<T>() );
            }
            
            return bindings;
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
                        // anonymous params
                        if ( paramInjectInfo.IsAnonymous )
                        {
                            object instance;
                            if ( TryGet( paramInjectInfo.Type, out instance ) )
                            {
                                methodArguments.Add( instance );
                            }
                            else
                            {
                                // no binding exists, error if not optional
                                if ( paramInjectInfo.IsOptional == false )
                                {
                                    // TODO(dak): is throwing the correct choice here? If Flushing the inject queue, all subsequent objects won't get injected
                                    throw new InvalidOperationException( string.Format( "No binding of type '{0}' exists for attempted injection '{1}'", 
                                                                                        paramInjectInfo.Type, 
                                                                                        methodInjectInfo.MethodInfo.Name ) );
                                }

                                methodArguments.Add( null );
                            }
                        }
                        // named params
                        else
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
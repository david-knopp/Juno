using System;
using System.Collections;
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
            m_bindings = new Dictionary<Type, Dictionary<int, List<object>>>();
            m_injectQueue = new HashSet<object>();
        }

        public DIContainer Parent
        {
            get;
            set;
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
            Dictionary<int, List<object>> typeBindings;
            if ( m_bindings.TryGetValue( type, out typeBindings ) == false )
            {
                typeBindings = new Dictionary<int, List<object>>();
                m_bindings.Add( type, typeBindings );
            }

            List<object> bindings;
            if ( typeBindings.TryGetValue( id, out bindings ) == false )
            {
                bindings = new List<object>();
                typeBindings.Add( id, bindings );
            }

            bindings.Add( instance );

            QueueForInject( instance );
        }

        public void Unbind<T>( int id = c_defaultID )
        {            
            Dictionary<int, List<object>> typeBindings;
            if ( m_bindings.TryGetValue( typeof( T ), out typeBindings ) )
            {
                typeBindings.Remove( id );
            }
        }

        public bool TryGet<T>( out T instance, int id = c_defaultID )
        {
            object objInstance;
            if ( TryGet( typeof( T ), out objInstance, id ) )
            {
                instance = ( T )objInstance;
                return true;
            }

            instance = default( T );
            return false;
        }

        public bool TryGet( Type type, out object instance, int id = c_defaultID )
        {
            Dictionary<int, List<object>> typeBindings;
            if ( m_bindings.TryGetValue( type, out typeBindings ) )
            {
                List<object> bindings;
                if ( typeBindings.TryGetValue( id, out bindings ) &&
                     bindings.Count > 0 )
                {
                    instance = bindings.First();
                    return true;
                }
            }

            // try parent
            if ( Parent != null )
            {
                return Parent.TryGet( type, out instance, id );
            }

            instance = null;
            return false;
        }

        public T Get<T>( int id = c_defaultID )
        {            
            T instance;
            if ( TryGet<T>( out instance, id ) )
            {
                return instance;
            }

            throw new KeyNotFoundException( $"No bindings exist for type '{typeof( T ).FullName}' with id '{id}'" );
        }

        public bool TryGetAll<T>( out List<T> instances )
        {
            List<object> objects;
            if ( TryGetAll( out objects ) )
            {
                instances = objects.Cast<T>().ToList();
                return true;
            }

            instances = null;
            return false;
        }

        public bool TryGetAll( Type type, out List<object> instances )
        {
            Dictionary<int, List<object>> typeBindings;
            if ( m_bindings.TryGetValue( type, out typeBindings ) )
            {
                instances = typeBindings.Values.SelectMany( x => x ).ToList();
                return true;
            }

            instances = null;
            return false;
        }

        public bool TryGetAll( Type type, out IList instances )
        {
            Dictionary<int, List<object>> typeBindings;
            if ( m_bindings.TryGetValue( type, out typeBindings ) )
            {
                var genericType = typeof( List<> ).MakeGenericType( type );
                instances = ( IList )Activator.CreateInstance( genericType );

                foreach ( var bindings in typeBindings )
                {
                    foreach ( var instance in bindings.Value )
                    {
                        instances.Add( instance );
                    }
                }

                return true;
            }

            instances = null;
            return false;
        }

        public List<T> GetAll<T>()
        {
            var bindings = GetAll( typeof( T ) );
            return bindings.Cast<T>().ToList();
        }

        public List<object> GetAll( Type type )
        {
            List<object> bindings;
            if ( TryGetAll( type, out bindings ) )
            {
                return bindings;
            }
            
            throw new KeyNotFoundException( $"No bindings exist for type '{type.FullName}'" );
        }
        #endregion binding

        #region injection
        public void Inject( object obj )
        {
            if ( obj != null )
            {
                // get info
                Type objType = obj.GetType();
                TypeInjectInfo typeInfo = TypeAnalyzer.GetTypeInjectInfo( objType );

                // inject
                InjectMethods( obj, typeInfo );
                InjectFields( obj, typeInfo );
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
        #endregion injection
        #endregion public

        #region private
        private Dictionary<Type, Dictionary<int, List<object>>> m_bindings;
        private HashSet<object> m_injectQueue;

        private void InjectMethods( object obj, in TypeInjectInfo typeInfo )
        {
            foreach ( MethodInjectInfo methodInjectInfo in typeInfo.MethodInfo )
            {
                // get desired arguments to inject
                List<object> methodArguments = new List<object>();

                foreach ( ParamInjectInfo paramInjectInfo in methodInjectInfo.ParamInfo )
                {
                    object instance;
                    var paramType = paramInjectInfo.Type;
                    int id = paramInjectInfo.IsAnonymous ? c_defaultID : paramInjectInfo.ID.Value;

                    // get instance to inject
                    if ( TryGet( paramType, out instance, id ) == false )
                    {
                        IList instanceList;
                        Type genericTypeArg = paramType.GenericTypeArguments.First();
                        
                        // check if requested type is a generic list, and try to inject all bindings of type
                        if ( IsCandidateForGetAll( paramType ) &&
                             TryGetAll( paramType.GenericTypeArguments.First(), out instanceList ) )
                        {
                            instance = instanceList;
                        }
                        // check if parameter is optional
                        else if ( paramInjectInfo.IsOptional == false )
                        {
                            throw new InvalidOperationException( $"No binding of type '{paramType}' with ID '{id}' exists for attempted method injection '{methodInjectInfo.MethodInfo.Name}'" );
                        }
                    }

                    methodArguments.Add( instance );
                }

                methodInjectInfo.MethodInfo.Invoke( obj, methodArguments.ToArray() );
            }
        }

        private void InjectFields( object obj, in TypeInjectInfo typeInfo )
        {
            foreach ( FieldInjectInfo fieldInjectInfo in typeInfo.FieldInfo )
            {
                object instance;
                int id = fieldInjectInfo.IsAnonymous ? c_defaultID : fieldInjectInfo.ID.Value;
                Type fieldType = fieldInjectInfo.Type;

                if ( TryGet( fieldType, out instance, id ) == false )
                {
                    IList instanceList;
                    Type genericTypeArg = fieldType.GenericTypeArguments.First();

                    if ( IsCandidateForGetAll( fieldType ) &&
                         TryGetAll( genericTypeArg, out instanceList ) )
                    {
                        instance = instanceList;
                    }
                    else if ( fieldInjectInfo.IsOptional == false )
                    {
                        throw new InvalidOperationException( $"No binding of type '{fieldType}' with ID '{id}' exists for attempted member injection '{fieldInjectInfo.FieldInfo.Name}'" );
                    }
                }

                fieldInjectInfo.FieldInfo.SetValue( obj, instance );
            }
        }

        private static bool IsCandidateForGetAll( Type type )
        {
            return type.IsGenericType &&
                    ( type.GetGenericTypeDefinition() == typeof( List<> ) ||
                      type.GetGenericTypeDefinition() == typeof( IList<> ) ||
                      type.GetGenericTypeDefinition() == typeof( IReadOnlyList<> ) ||
                      type.GetGenericTypeDefinition() == typeof( IEnumerable<> ) );
        }
        #endregion private
    }
}
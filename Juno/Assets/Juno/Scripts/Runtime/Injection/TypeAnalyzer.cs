using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Juno
{
    internal static class TypeAnalyzer
    {
        public static TypeInjectInfo GetTypeInjectInfo( Type type )
        {
            TypeInjectInfo info;

            if ( s_cachedTypeInfo.TryGetValue( type, out info ) == false )
            {
                info = new TypeInjectInfo( GetMethodInjectInfo( type ),
                                           GetFieldInjectInfo( type ) );
                s_cachedTypeInfo.Add( type, info );
            }

            return info;
        }

        private static Dictionary<Type, TypeInjectInfo> s_cachedTypeInfo = new Dictionary<Type, TypeInjectInfo>();

        private static List<MethodInjectInfo> GetMethodInjectInfo( Type type )
        {
            List<MethodInfo> methodInfos = GetHierarchyMethodInfo( type );
            List<MethodInjectInfo> methodInjectInfos = new List<MethodInjectInfo>();

            // TODO: order methods by class hierarchy?

            for ( int i = 0; i < methodInfos.Count; i++ )
            {
                MethodInfo methodInfo = methodInfos[i];
                object[] injectAttributes = methodInfo.GetCustomAttributes( attributeType: typeof( InjectAttribute ), 
                                                                            inherit: false );
                if ( injectAttributes.Length > 0 )
                {
                    methodInjectInfos.Add( new MethodInjectInfo( methodInfo,
                                                                 GetParamInjectInfo( methodInfo.GetParameters() ) ) );
                }
            }

            return methodInjectInfos;
        }

        private static List<ParamInjectInfo> GetParamInjectInfo( ParameterInfo[] paramsInfo )
        {
            List<ParamInjectInfo> paramInjectInfo = new List<ParamInjectInfo>();

            for ( int i = 0; i < paramsInfo.Length; i++ )
            {
                ParameterInfo paramInfo = paramsInfo[i];
                ParamInjectInfo injectableParamInfo;

                // extract inject attribute data
                object[] injectAttributes = paramInfo.GetCustomAttributes( attributeType: typeof( InjectAttribute ), 
                                                                           inherit: false );
                if ( injectAttributes.Length > 0 )
                {
                    InjectAttribute injectAttribute = ( InjectAttribute )injectAttributes[0];
                    injectableParamInfo = new ParamInjectInfo( type: paramInfo.ParameterType, 
                                                               id: injectAttribute.ID, 
                                                               isOptional: injectAttribute.IsOptional );
                }
                // no inject attribute available
                else
                {
                    injectableParamInfo = new ParamInjectInfo( type: paramInfo.ParameterType, 
                                                               isOptional: false );
                }

                paramInjectInfo.Add( injectableParamInfo );
            }

            return paramInjectInfo;
        }

        private static List<MethodInfo> GetHierarchyMethodInfo( Type type )
        {
            List<MethodInfo> info = new List<MethodInfo>();

            for ( ; type != null && type != typeof( object ); type = type.BaseType )
            {
                info.AddRange( type.GetMethods( BindingFlags.Public |
                                                BindingFlags.NonPublic |
                                                BindingFlags.Instance |
                                                BindingFlags.DeclaredOnly ) );
            }

            return info;
        }

        private static List<FieldInjectInfo> GetFieldInjectInfo( Type type )
        {
            List<FieldInfo> fieldInfos = GetHierarchyFieldInfo( type );
            List<FieldInjectInfo> fieldInjectInfos = new List<FieldInjectInfo>();

            foreach ( var fieldInfo in fieldInfos )
            {
                object[] injectAttributes = fieldInfo.GetCustomAttributes( attributeType: typeof( InjectAttribute ),
                                                                           inherit: false );

                if ( injectAttributes.Length > 0 )
                {
                    InjectAttribute injectAttribute = ( InjectAttribute )injectAttributes[0];

                    if ( injectAttribute.HasID )
                    {
                        fieldInjectInfos.Add( new FieldInjectInfo( type: fieldInfo.FieldType,
                                                                   fieldInfo: fieldInfo,
                                                                   id: injectAttribute.ID,
                                                                   isOptional: injectAttribute.IsOptional ) );
                    }
                    else
                    {
                        fieldInjectInfos.Add( new FieldInjectInfo( type: fieldInfo.FieldType,
                                                                   fieldInfo: fieldInfo,
                                                                   isOptional: injectAttribute.IsOptional ) );
                    }
                }
            }

            return fieldInjectInfos;
        }

        private static List<FieldInfo> GetHierarchyFieldInfo( Type type )
        {
            List<FieldInfo> info = new List<FieldInfo>();

            for ( ; type != null && type != typeof( object ); type = type.BaseType )
            {
                info.AddRange( type.GetFields( BindingFlags.Public |
                                               BindingFlags.NonPublic |
                                               BindingFlags.Instance |
                                               BindingFlags.DeclaredOnly ).Where( x => x.IsDefined( typeof( InjectAttribute ) ) ) );
            }

            return info;
        }
    }
}
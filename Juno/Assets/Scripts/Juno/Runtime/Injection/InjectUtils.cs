using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Juno
{
    public static class InjectUtils
    {
        public static void GetInjectableMonoBehavioursNonAlloc( GameObject gameObject, ref List<MonoBehaviour> injectableBehaviors )
        {
            Transform rootTransform = gameObject.transform;
            MonoBehaviour[] childBehaviors = gameObject.GetComponentsInChildren<MonoBehaviour>( true );
            List<Transform> contextTransforms = childBehaviors.OfType<ContextBase>().Select( x => x.transform ).ToList();

            foreach ( var behavior in childBehaviors )
            {
                if ( IsInjectable( behavior, rootTransform ) )
                {
                    injectableBehaviors.Add( behavior );
                }
            }
        }

        public static List<MonoBehaviour> GetInjectableMonoBehaviours( GameObject gameObject )
        {
            List<MonoBehaviour> behaviors = new List<MonoBehaviour>();
            GetInjectableMonoBehavioursNonAlloc( gameObject, ref behaviors );

            return behaviors;
        }

        public static List<MonoBehaviour> GetInjectableMonoBehaviours( Scene scene )
        {
            List<MonoBehaviour> behaviors = new List<MonoBehaviour>();

            if ( scene.isLoaded )
            {
                GameObject[] rootObjects = scene.GetRootGameObjects();
                for ( int i = 0; i < rootObjects.Length; i++ )
                {
                    GameObject rootObject = rootObjects[i];
                    if ( rootObject.GetComponent<GameObjectContext>() == null )
                    {
                        GetInjectableMonoBehavioursNonAlloc( rootObject, ref behaviors );
                    }
                }
            }
            else
            {
                throw new InvalidOperationException( string.Format( "Unable to get Injectable MonoBehaviours from scene '{0}' as it's not yet loaded", scene.name ) );
            }

            return behaviors;
        }

        public static bool IsInjectable( MonoBehaviour behavior, Transform rootTransform )
        {
            if ( behavior != null )
            {
                // check if child of GameObjectContext
                Transform transform = behavior.transform;
                while ( transform != null && 
                        transform != rootTransform )
                {
                    if ( transform.GetComponent<GameObjectContext>() != null )
                    {
                        return false;
                    }

                    transform = transform.parent;
                }

                return true;
            }

            return false;
        }
    }
}
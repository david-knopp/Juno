using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Juno
{
    public static class InjectUtils
    {
        public static List<MonoBehaviour> GetInjectableMonoBehaviours( GameObject gameObject )
        {
            Transform rootTransform = gameObject.transform;
            MonoBehaviour[] childBehaviors = gameObject.GetComponentsInChildren<MonoBehaviour>( true );
            List<Transform> contextTransforms = childBehaviors.OfType<ContextBase>().Select( x => x.transform ).ToList();
            List<MonoBehaviour> behaviors = new List<MonoBehaviour>();

            for ( int i = 0; i < childBehaviors.Length; i++ )
            {
                MonoBehaviour behavior = childBehaviors[i];

                // ensure this isn't a missing component reference
                if ( behavior != null )
                {
                    // check if child of GameObjectContext
                    for ( Transform transform = behavior.transform; transform != null; transform = transform.parent )
                    {
                        if ( transform == rootTransform )
                        {
                            break;
                        }
                        else if ( transform.GetComponent<GameObjectContext>() != null )
                        {
                            continue;
                        }
                    }

                    behaviors.Add( behavior );
                }
            }

            return behaviors;
        }

        public static List<MonoBehaviour> GetInjectableMonoBehaviours( UnityEngine.SceneManagement.Scene scene )
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
                        behaviors.AddRange( GetInjectableMonoBehaviours( rootObject ) );
                    }
                }
            }

            return behaviors;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Juno
{
    public sealed class GameObjectContext : ContextBase
    {
        #region public
        public override DIContainer GetContainer()
        {
            ConnectParent();
            return base.GetContainer();
        }
        #endregion public

        #region protected
        protected override void OnPreInject()
        {
            base.OnPreInject();
            ConnectParent();
            QueueChildrenAndSelfForInject();
        }
        #endregion protected

        #region private
        private void QueueChildrenAndSelfForInject()
        {
            List<MonoBehaviour> behaviors = InjectUtils.GetInjectableMonoBehaviours( gameObject );
            foreach ( var behavior in behaviors )
            {
                Container.QueueForInject( behavior );
            }
        }

        private void ConnectParent()
        {
            // set parent
            if ( Container.Parent == null )
            {
                // context from object hierarchy
                GameObjectContext goContext = null;
                if ( transform.parent != null )
                {
                    goContext = transform.parent.GetComponentInParent<GameObjectContext>();
                }

                if ( goContext != null )
                {
                    Container.Parent = goContext.GetContainer();
                }
                else
                {
                    // scene's context if one exists
                    SceneContext sceneContext;
                    if ( SceneContext.TryGetSceneContext( gameObject.scene.path, out sceneContext ) )
                    {
                        Container.Parent = sceneContext.GetContainer();
                    }
                    // fallback to project's context
                    else
                    {
                        Container.Parent = ProjectContext.Instance.GetContainer();
                    }
                }
            }
        }
        #endregion private
    }
}
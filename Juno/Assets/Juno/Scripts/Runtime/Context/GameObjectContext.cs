using System.Collections.Generic;
using UnityEngine;

namespace Juno
{
    public class GameObjectContext : ContextBase
    {
        #region protected
        protected override void OnPreInject()
        {
            base.OnPreInject();

            // set context parent
            if ( Container.Parent == null )
            {
                // use Scene's context if one exists
                SceneContext sceneContext;
                if ( SceneContext.TryGetSceneContext( gameObject.scene.path, out sceneContext ) )
                {
                    Container.Parent = sceneContext.Container;
                }
                // fallback to project's context
                else
                {
                    Container.Parent = ProjectContext.Instance.Container;
                }
            }

            QueueChildrenAndSelfForInject();
        }
        #endregion // protected

        #region private
        private void QueueChildrenAndSelfForInject()
        {
            List<MonoBehaviour> behaviors = InjectUtils.GetInjectableMonoBehaviours( gameObject );
            foreach ( var behavior in behaviors )
            {
                Container.QueueForInject( behavior );
            }
        }
        #endregion // private
    }
}
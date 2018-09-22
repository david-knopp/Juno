using System.Collections.Generic;
using UnityEngine;

namespace Juno
{
    public sealed class SceneContext : ContextBase
    {
        #region public
        static SceneContext()
        {
            s_sceneContexts = new Dictionary<string, SceneContext>();
        }

        public static bool TryGetSceneContext( string scenePath, out SceneContext sceneContext )
        {
            return s_sceneContexts.TryGetValue( scenePath, out sceneContext );
        }
        #endregion // public

        #region protected
        protected override void OnAwake()
        {
            base.OnAwake();
            Container.Parent = ProjectContext.Instance.GetContainer();
        }

        protected override void OnPreInject()
        {
            base.OnPreInject();
            QueueSceneObjectsForInject();
        }
        #endregion // protected

        #region private
        private static Dictionary<string, SceneContext> s_sceneContexts;

        private void OnEnable()
        {
            s_sceneContexts.Add( gameObject.scene.path, this );
        }

        private void OnDisable()
        {
            s_sceneContexts.Remove( gameObject.scene.path );
        }

        private void QueueSceneObjectsForInject()
        {
            List<MonoBehaviour> behaviors = InjectUtils.GetInjectableMonoBehaviours( gameObject.scene );
            foreach ( var behavior in behaviors )
            {
                Container.QueueForInject( behavior );
            }
        }
        #endregion // private
    }
}
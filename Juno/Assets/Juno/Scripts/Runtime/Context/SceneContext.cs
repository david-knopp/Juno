using System.Collections.Generic;
using UnityEngine;

namespace Juno
{
    public class SceneContext : ContextBase
    {
        #region protected
        protected override void OnAwake()
        {
            base.OnAwake();
            Container.Parent = ProjectContext.Instance.Container;
        }

        protected override void OnPreInject()
        {
            base.OnPreInject();
            QueueSceneObjectsForInject();
        }
        #endregion // protected

        #region private
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
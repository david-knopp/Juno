using System.Collections.Generic;
using UnityEngine;

namespace Juno
{
    public class GameObjectContext : ContextBase
    {
        #region protected
        protected override void OnAwake()
        {
            base.OnAwake();

            // TODO: Try to parent to scene first
            Container.Parent = ProjectContext.Instance.Container;
        }

        protected override void OnPreInject()
        {
            base.OnPreInject();
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
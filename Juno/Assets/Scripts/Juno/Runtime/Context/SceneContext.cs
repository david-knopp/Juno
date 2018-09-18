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

        }
        #endregion // protected

        #region private
        private void QueueSceneObjectsForInject()
        {

        }
        #endregion // private
    }
}
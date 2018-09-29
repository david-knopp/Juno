namespace Juno.Test
{
    public class Test_FactoryInstaller : MonoBehaviourInstaller
    {
        public override void InstallBindings( DIContainer container )
        {
            container.Bind<Factory>();
        }
    }
}
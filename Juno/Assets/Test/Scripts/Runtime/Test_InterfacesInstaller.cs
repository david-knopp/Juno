namespace Juno
{
    public sealed class Test_InterfacesInstaller : MonoBehaviourInstaller
    {
        public override void InstallBindings( DIContainer container )
        {
            container.BindWithInterfaces<Test_Interfaces>();
        }
    }
}

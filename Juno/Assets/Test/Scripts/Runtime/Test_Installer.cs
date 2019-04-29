using UnityEngine;

namespace Juno.Test
{
    public sealed class Test_Installer : MonoBehaviourInstaller
    {
        public override void InstallBindings( DIContainer container )
        {
            container.Bind( m_value );
            container.Bind( m_value2, 2 );
        }


        [SerializeField] private string m_value;
        [SerializeField] private string m_value2;
    }
}
using UnityEngine;

namespace Juno
{
    public abstract class MonoBehaviourInstaller : MonoBehaviour, IInstaller
    {
        public abstract void InstallBindings( DIContainer container );
    }
}
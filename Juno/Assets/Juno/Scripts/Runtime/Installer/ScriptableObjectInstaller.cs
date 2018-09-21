using UnityEngine;

namespace Juno
{
    public abstract class ScriptableObjectInstaller : ScriptableObject, IInstaller
    {
        public abstract void InstallBindings( DIContainer container );
    }
}
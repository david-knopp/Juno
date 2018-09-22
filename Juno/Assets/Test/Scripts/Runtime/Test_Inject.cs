using UnityEngine;

namespace Juno.Test
{
    public sealed class Test_Inject : MonoBehaviour
    {
        [Inject]
        private void OnInject( string str )
        {
            Debug.LogFormat( "{0}.OnInject: '{1}'", name, str );
        }
    }
}
using UnityEngine;

namespace Juno.Test
{
    public sealed class Test_Inject : MonoBehaviour
    {
        [Inject]
        private void OnInject( string str )
        {
            Debug.LogFormat( "OnInject: '{0}'", str );
        }
    }
}
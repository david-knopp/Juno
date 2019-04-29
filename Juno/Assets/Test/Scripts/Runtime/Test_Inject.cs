using UnityEngine;

namespace Juno.Test
{
    public class Test_Inject : MonoBehaviour
    {
        [Inject]
        private void OnInject( string str )
        {
            Debug.LogFormat( "{0}.OnInject(Base): '{1}'", name, str );
        }
    }
}
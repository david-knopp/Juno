using UnityEngine;

namespace Juno.Test
{
    public class Test_Inject : MonoBehaviour
    {
        [Inject]
        private void Inject()
        {
            Debug.Log( "Inject" );
        }
    }
}
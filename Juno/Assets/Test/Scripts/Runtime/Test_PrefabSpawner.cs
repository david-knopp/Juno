using UnityEngine;

namespace Juno.Test
{
    public class Test_PrefabSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject m_prefab;

        private Factory m_factory;

        [Inject]
        private void Inject( Factory factory )
        {
            m_factory = factory;
        }

        private void Update()
        {
            if ( Input.GetKeyDown( KeyCode.Space ) )
            {
                m_factory.Clone( m_prefab, transform );   
            }
        }
    }
}
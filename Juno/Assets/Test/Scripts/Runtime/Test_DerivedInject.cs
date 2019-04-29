using UnityEngine;

namespace Juno.Test
{
    public sealed class Test_DerivedInject : Test_Inject
    {
        [Inject]
        private void OnInject( string str )
        {
            Debug.LogFormat( "{0}.OnInject(Derived): '{1}'", name, str );
        }

        [Inject]
        private string m_value;

        [Inject( ID = 2 )]
        private string m_value2;

        private void Start()
        {
            Debug.Log( $"m_value: {m_value}, m_value2: {m_value2}" );
        }
    }
}

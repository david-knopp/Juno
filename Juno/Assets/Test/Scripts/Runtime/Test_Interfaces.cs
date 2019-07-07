using Juno;
using System;
using UnityEngine;

namespace Juno
{
    public sealed class Test_Interfaces : IInitializable, IDisposable, ITickable, ILateTickable
    {
        public void Initialize()
        {
            Debug.Log( "Initialize" );
        }

        public void Dispose()
        {
            Debug.Log( "Dispose" );
        }

        public void Tick()
        {
            Debug.Log( "Tick" );
        }

        public void LateTick()
        {
            Debug.Log( "LateTick" );
        }
    }
}

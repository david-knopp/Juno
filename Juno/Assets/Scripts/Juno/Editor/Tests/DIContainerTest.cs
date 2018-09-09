using UnityEngine.TestTools;
using NUnit.Framework;
using System;

namespace Juno.Test
{
    public sealed class DIContainerTest
    {
        private class TestClass
        {
            public string m_id;
        }

        #region Bind
        [Test]
        public void Bind_Instance_Anonymous()
        {
            TestClass testObj = new TestClass() { m_id = "Test" };
            DIContainer container = new DIContainer();

            container.Bind( testObj );
        }

        [Test]
        public void Bind_Instance_WithID( [Range( -10000, 10000, 1000 )] int id )
        {
            TestClass testObj = new TestClass() { m_id = "Test" };
            DIContainer container = new DIContainer();

            container.Bind( testObj );
        }

        [Test]
        public void Bind_Anonymous()
        {
            DIContainer container = new DIContainer();
            container.Bind<TestClass>();
        }

        [Test]
        public void Bind_WithID( [Range( -10000, 10000, 1000 )] int id )
        {
            DIContainer container = new DIContainer();
            container.Bind<TestClass>( id );
        }

        [Test]
        public void Bind_Duplicate_Anonymous()
        {
            DIContainer container = new DIContainer();
            container.Bind<TestClass>();

            Assert.Throws<ArgumentException>( () => container.Bind<TestClass>() );
        }

        [Test]
        public void Bind_Duplicate_WithID( [Range( -10000, 10000, 1000 )] int id )
        {
            DIContainer container = new DIContainer();
            container.Bind<TestClass>( id );

            Assert.Throws<ArgumentException>( () => container.Bind<TestClass>( id ) );
        }

        [Test]
        public void Bind_Multiple_WithID( [Range( -10000, 10000, 1000 )] int id )
        {
            DIContainer container = new DIContainer();
            container.Bind<TestClass>( id );
            container.Bind<TestClass>( id + 1 );
        }
        #endregion // Bind

        #region Get
        [Test]
        public void Get_Anonymous()
        {
            TestClass testObj = new TestClass() { m_id = "Test" };
            DIContainer container = new DIContainer();

            container.Bind( testObj );
            Assert.AreEqual( testObj, container.Get<TestClass>() );
        }

        [Test]
        public void Get_WithID( [Range( -10000, 10000, 1000 )] int id )
        {
            TestClass testObj = new TestClass() { m_id = "Test" };
            DIContainer container = new DIContainer();

            container.Bind( testObj, id );
            Assert.AreEqual( testObj, container.Get<TestClass>( id ) );
        }
        #endregion // Get
    }
}

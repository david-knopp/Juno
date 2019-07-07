using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Juno.Test
{
    public sealed class DIContainerTest
    {
        private class TestClass
        {
            public string m_id;

            [Inject]
            private void Inject()
            {
            }
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
            container.Bind<TestClass>();
        }

        [Test]
        public void Bind_Duplicate_WithID( [Range( -10000, 10000, 1000 )] int id )
        {
            DIContainer container = new DIContainer();
            container.Bind<TestClass>( id );
            container.Bind<TestClass>( id );
        }

        [Test]
        public void Bind_Multiple_WithID( [Range( -10000, 10000, 1000 )] int id )
        {
            DIContainer container = new DIContainer();
            container.Bind<TestClass>( id );
            container.Bind<TestClass>( id + 1 );
        }
        #endregion Bind

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

        [Test]
        public void Get_Multiple_Anonymous()
        {
            TestClass testObj1 = new TestClass() { m_id = "Test1" };
            TestClass testObj2 = new TestClass() { m_id = "Test2" };
            DIContainer container = new DIContainer();

            container.Bind( testObj1 );
            container.Bind( testObj2 );

            var bindings = container.GetAll<TestClass>();
            Assert.IsTrue( bindings.Count == 2 );
        }

        [Test]
        public void Get_Multiple_WithID( [Range( -10000, 10000, 1000 )] int id )
        {
            TestClass testObj1 = new TestClass() { m_id = "Test1" };
            TestClass testObj2 = new TestClass() { m_id = "Test2" };
            DIContainer container = new DIContainer();

            container.Bind( testObj1, id );
            container.Bind( testObj2, id );

            var bindings = container.GetAll<TestClass>();
            Assert.IsTrue( bindings.Count == 2 );
        }
        
        [Test]
        public void Get_Multiple_WithVaryingIDs( [Range( -10000, 10000, 1000 )] int initialID )
        {
            TestClass testObj1 = new TestClass() { m_id = "Test1" };
            TestClass testObj2 = new TestClass() { m_id = "Test2" };
            DIContainer container = new DIContainer();

            container.Bind( testObj1, initialID++ );
            container.Bind( testObj2, initialID );

            var bindings = container.GetAll<TestClass>();
            Assert.IsTrue( bindings.Count == 2 );
        }
        #endregion Get

        #region Injection
        private class FieldInjectTestClass
        {
            [Inject( ID = 1 )] public string Value1;
            [Inject( ID = 2 )] public string Value2;
        }

        private class MethodInjectTestClass
        {
            public string Value1
            {
                get;
                private set;
            }

            public string Value2
            {
                get;
                private set;
            }

            [Inject]
            private void Inject( [Inject( ID = 1 )] string value1,
                                 [Inject( ID = 2 )] string value2 )
            {
                Value1 = value1;
                Value2 = value2;
            }
        }

        private class FieldInjectTestClassGeneric<T>
        {
            [Inject] public T Value;
        }

        private class MethodInjectTestClassGeneric<T>
        {
            public T Value
            {
                get;
                private set;
            }

            [Inject] 
            private void Inject( T value )
            {
                Value = value;
            }
        }

        [Test]
        public void Inject_Method_SingleViaBind( [Values( "TestID" )] string id )
        {
            DIContainer container = new DIContainer();
            var testClass = new MethodInjectTestClassGeneric<string>();

            container.Bind( id );
            container.Bind( testClass );

            container.FlushInjectQueue();

            Assert.AreEqual( testClass.Value, id );
        }

        [Test]
        public void Inject_Field_SingleViaBind( [Values( "TestID" )] string id )
        {
            DIContainer container = new DIContainer();
            var testClass = new FieldInjectTestClassGeneric<string>();

            container.Bind( id );
            container.Bind( testClass );

            container.FlushInjectQueue();

            Assert.AreEqual( testClass.Value, id );
        }

        [Test]
        public void Inject_Method_SingleViaManualInject( [Values( "TestID" )] string id )
        {
            DIContainer container = new DIContainer();
            var testClass = new MethodInjectTestClassGeneric<string>();

            container.Bind( id );
            container.Inject( testClass );

            Assert.AreEqual( testClass.Value, id );
        }

        [Test]
        public void Inject_Field_SingleViaManualInject( [Values( "TestID" )] string id )
        {
            DIContainer container = new DIContainer();
            var testClass = new FieldInjectTestClassGeneric<string>();

            container.Bind( id );
            container.Inject( testClass );

            Assert.AreEqual( testClass.Value, id );
        }

        [Test]
        public void Inject_Method_SingleViaInjectQueue( [Values( "TestID" )] string id )
        {
            DIContainer container = new DIContainer();
            var testClass = new MethodInjectTestClassGeneric<string>();

            container.Bind( id );
            container.QueueForInject( testClass );
            container.FlushInjectQueue();

            Assert.AreEqual( testClass.Value, id );
        }

        [Test]
        public void Inject_Field_SingleViaInjectQueue( [Values( "TestID" )] string id )
        {
            DIContainer container = new DIContainer();
            var testClass = new FieldInjectTestClassGeneric<string>();

            container.Bind( id );
            container.QueueForInject( testClass );
            container.FlushInjectQueue();

            Assert.AreEqual( testClass.Value, id );
        }

        [Test]
        public void Inject_Method_MultipleViaBind( [Values( "Value1" )] string value1,
                                                   [Values( "Value2" )] string value2 )
        {
            DIContainer container = new DIContainer();
            MethodInjectTestClass testClass = new MethodInjectTestClass();

            container.Bind( value1, 1 );
            container.Bind( value2, 2 );
            container.Bind( testClass );

            container.FlushInjectQueue();

            Assert.AreEqual( testClass.Value1, value1 );
            Assert.AreEqual( testClass.Value2, value2 );
        }

        [Test]
        public void Inject_Field_MultipleViaBind( [Values( "Value1" )] string value1,
                                                  [Values( "Value2" )] string value2 )
        {
            DIContainer container = new DIContainer();
            var testClass = new FieldInjectTestClass();

            container.Bind( value1, 1 );
            container.Bind( value2, 2 );
            container.Bind( testClass );

            container.FlushInjectQueue();

            Assert.AreEqual( testClass.Value1, value1 );
            Assert.AreEqual( testClass.Value2, value2 );
        }

        [Test]
        public void Inject_Method_MultipleViaManualInject( [Values( "Value1" )] string value1,
                                                           [Values( "Value2" )] string value2 )
        {
            DIContainer container = new DIContainer();
            MethodInjectTestClass testClass = new MethodInjectTestClass();

            container.Bind( value1, 1 );
            container.Bind( value2, 2 );

            container.Inject( testClass );

            Assert.AreEqual( testClass.Value1, value1 );
            Assert.AreEqual( testClass.Value2, value2 );
        }

        [Test]
        public void Inject_Field_MultipleViaManualInject( [Values( "Value1" )] string value1,
                                                          [Values( "Value2" )] string value2 )
        {
            DIContainer container = new DIContainer();
            var testClass = new FieldInjectTestClass();

            container.Bind( value1, 1 );
            container.Bind( value2, 2 );

            container.Inject( testClass );

            Assert.AreEqual( testClass.Value1, value1 );
            Assert.AreEqual( testClass.Value2, value2 );
        }

        [Test]
        public void Inject_Method_MultipleViaInjectQueue( [Values( "Value1" )] string value1,
                                                          [Values( "Value2" )] string value2 )
        {
            DIContainer container = new DIContainer();
            MethodInjectTestClass testClass = new MethodInjectTestClass();

            container.Bind( value1, 1 );
            container.Bind( value2, 2 );
            container.QueueForInject( testClass );

            container.FlushInjectQueue();

            Assert.AreEqual( testClass.Value1, value1 );
            Assert.AreEqual( testClass.Value2, value2 );
        }

        [Test, TestCase( 0, 1, 1, 2, 3, 5, 8 )]
        public void Inject_Field_All_List( params int[] values )
        {
            DIContainer container = new DIContainer();
            FieldInjectTestClassGeneric<List<int>> testClass = new FieldInjectTestClassGeneric<List<int>>();

            foreach ( var value in values )
            {
                container.Bind( value );
            }
            
            container.Inject( testClass );


            Assert.AreEqual( testClass.Value.Count, values.Length );

            testClass.Value.Sort();
            for ( int i = 0; i < values.Length; i++ )
            {
                Assert.AreEqual( testClass.Value[i], values[i] );
            }
        }

        [Test, TestCase( 0, 1, 1, 2, 3, 5, 8 )]
        public void Inject_Field_All_IList( params int[] values )
        {
            DIContainer container = new DIContainer();
            FieldInjectTestClassGeneric<IList<int>> testClass = new FieldInjectTestClassGeneric<IList<int>>();

            foreach ( var value in values )
            {
                container.Bind( value );
            }
            
            container.Inject( testClass );

            Assert.AreEqual( testClass.Value.Count, values.Length );

            for ( int i = 0; i < values.Length; i++ )
            {
                Assert.AreEqual( testClass.Value[i], values[i] );
            }
        }

        [Test, TestCase( 0, 1, 1, 2, 3, 5, 8 )]
        public void Inject_Field_All_Enumerable( params int[] values )
        {
            DIContainer container = new DIContainer();
            FieldInjectTestClassGeneric<IEnumerable<int>> testClass = new FieldInjectTestClassGeneric<IEnumerable<int>>();

            foreach ( var value in values )
            {
                container.Bind( value );
            }
            
            container.Inject( testClass );

            Assert.AreEqual( testClass.Value.Count(), values.Length );
        }

        [Test, TestCase( 0, 1, 1, 2, 3, 5, 8 )]
        public void Inject_Field_All_IReadOnlyList( params int[] values )
        {
            DIContainer container = new DIContainer();
            FieldInjectTestClassGeneric<IReadOnlyList<int>> testClass = new FieldInjectTestClassGeneric<IReadOnlyList<int>>();

            foreach ( var value in values )
            {
                container.Bind( value );
            }
            
            container.Inject( testClass );


            Assert.AreEqual( testClass.Value.Count(), values.Length );

            for ( int i = 0; i < values.Length; i++ )
            {
                Assert.AreEqual( testClass.Value[i], values[i] );
            }
        }

        [Test, TestCase( 0, 1, 1, 2, 3, 5, 8 )]
        public void Inject_Method_All_List( params int[] values )
        {
            DIContainer container = new DIContainer();
            MethodInjectTestClassGeneric<List<int>> testClass = new MethodInjectTestClassGeneric<List<int>>();

            foreach ( var value in values )
            {
                container.Bind( value );
            }
            
            container.Inject( testClass );

            Assert.AreEqual( testClass.Value.Count, values.Length );

            for ( int i = 0; i < values.Length; i++ )
            {
                Assert.AreEqual( testClass.Value[i], values[i] );
            }
        }
        #endregion Injection
    }
}

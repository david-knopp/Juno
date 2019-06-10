using System;
using System.Linq;
using UnityEngine;

namespace Juno
{
    public sealed class ProjectContext : ContextBase
    {
        #region public
        public static bool HasInstance
        {
            get { return m_instance != null; }
        }

        public static ProjectContext Instance
        {
            get
            {
                if ( HasInstance == false &&
                     m_isApplicationQuitting == false)
                {                    
                    // search for scene instance
                    m_instance = FindObjectOfType<ProjectContext>();
                    if ( !m_instance )
                    {
                        // instantiate from prefab
                        ProjectContext prefab = Resources.LoadAll<ProjectContext>( string.Empty ).FirstOrDefault();
                        if ( prefab )
                        {
                            m_instance = Instantiate( prefab );
                        }
                        else
                        {
                            Debug.LogWarning( "Couldn't find a ProjectContext prefab in resources, creating a default instance instead" );

                            // create new
                            GameObject obj = new GameObject( "ProjectContext" );
                            m_instance = obj.AddComponent<ProjectContext>();
                        }
                    }
                }

                return m_instance;
            }
        }
        #endregion public

        #region protected
        protected override void OnAwake()
        {
            base.OnAwake();

            if ( HasInstance == false )
            {
                m_instance = this;
            }

            if ( ReferenceEquals( m_instance, this ) == false )
            {
                throw new NotSupportedException( "Multiple ProjectContext instances detected - use of multiple instances is not supported" );
            }

            DontDestroyOnLoad( gameObject );
        }
        #endregion protected

        #region private
        private const string c_resourceName = "ProjectContext";
        private static ProjectContext m_instance;
        private static bool m_isApplicationQuitting;

        private void OnApplicationQuit()
        {
            // catch if app is quitting to prevent allocation on exit
            m_isApplicationQuitting = true;
        }
        #endregion private
    }
}
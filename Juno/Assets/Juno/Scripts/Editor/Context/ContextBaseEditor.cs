using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Juno
{
    [CustomEditor( typeof( ContextBase ), true )]
    public class ContextBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            m_monoInstallerList.DoLayoutList();
            m_soInstallerList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private ReorderableList m_monoInstallerList;
        private ReorderableList m_soInstallerList;

        private void OnEnable()
        {
            m_monoInstallerList = CreateList( "m_monoBehaviourInstallers", "MonoBehaviour Installers" );
            m_soInstallerList = CreateList( "m_scriptableObjectInstallers", "ScriptableObject Installers" );
        }

        private ReorderableList CreateList( string propertyPath, string displayName )
        {
            SerializedProperty prop = serializedObject.FindProperty( propertyPath );
            ReorderableList list = new ReorderableList( serializedObject, prop );

            list.drawHeaderCallback = ( Rect rect ) =>
            {
                GUI.Label( rect, displayName );
            };
            list.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField( rect, prop.GetArrayElementAtIndex( index ), new GUIContent(), true );
                EditorGUI.indentLevel--;
            };

            list.elementHeightCallback = ( int index ) =>
            {
                return EditorGUI.GetPropertyHeight( prop.GetArrayElementAtIndex( index ), true );
            };


            return list;
        }
    }
}

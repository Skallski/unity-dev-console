using DevConsole.Window;
using UnityEditor;

namespace DevConsole.Editor
{
    [CustomEditor(typeof(DevConsoleController), true)]
    public class DevConsoleEditor : UnityEditor.Editor
    {
        private DevConsoleController _devConsoleController;

        private SerializedProperty _consoleOpenKeycode;
        private SerializedProperty _content;

        private void OnEnable()
        {
            _devConsoleController = target as DevConsoleController;

            _consoleOpenKeycode = serializedObject.FindProperty("_consoleOpenKeycode");
            _content = serializedObject.FindProperty("_content");
        }

        public override void OnInspectorGUI()
        {
            if (_devConsoleController == null)
            {
                return;
            }
            
            serializedObject.Update();
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.PropertyField(_consoleOpenKeycode);
            EditorGUILayout.PropertyField(_content);

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
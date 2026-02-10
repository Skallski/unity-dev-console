using System;
using DevConsole.Window.Flex;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Object = UnityEngine.Object;

namespace DevConsole.Editor.Settings
{
    public static class DevConsoleSettings
    {
        private const string PROJECT_SETTINGS_PATH = "Project/Dev Console";

        [MenuItem("Window/Dev Console/Settings")]
        public static void OpenSettings()
        {
            SettingsService.OpenProjectSettings(PROJECT_SETTINGS_PATH);
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var repositionHandler = Object.FindObjectOfType<DevConsoleWindowRepositionHandler>(true);
            var resizeHandler = Object.FindObjectOfType<DevConsoleWindowResizeHandler>(true);
            
            return new SettingsProvider(PROJECT_SETTINGS_PATH, SettingsScope.Project)
            {
                label = "Dev Console",
                guiHandler = _ =>
                {
                    EditorGUILayout.Space();
                    DrawHandlerToggles(repositionHandler, "_allowReposition", "_resetPositionOnOpen");
                    DrawHandlerToggles(resizeHandler, "_allowResize", "_resetSizeOnOpen");
                },
                keywords = new System.Collections.Generic.HashSet<string>
                {
                    "Allow Reposition", "Reset Position On Open", "Allow Resize", "Reset Size On Open"
                }
            };
            
            void DrawHandlerToggles(Object target, string field1, string field2)
            {
                if (target == null)
                {
                    return;
                }

                Type type = target.GetType();
                FieldInfo f1 = type.GetField(field1, BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo f2 = type.GetField(field2, BindingFlags.Instance | BindingFlags.NonPublic);
                if (f1 == null || f2 == null)
                {
                    return;
                }

                bool v1 = (bool) f1.GetValue(target);
                bool v2 = (bool) f2.GetValue(target);

                EditorGUI.BeginChangeCheck();

                DrawOffsetToggle(ObjectNames.NicifyVariableName(field1), ref v1);
                DrawOffsetToggle(ObjectNames.NicifyVariableName(field2), ref v2);

                if (EditorGUI.EndChangeCheck())
                {
                    f1.SetValue(target, v1);
                    f2.SetValue(target, v2);
                    EditorUtility.SetDirty(target);
                }
            }

            void DrawOffsetToggle(string label, ref bool val)
            {
                const float offset = 8f;
                
                Rect rect = EditorGUILayout.GetControlRect();
                rect.x += offset;
                rect.width -= offset;
                val = EditorGUI.ToggleLeft(rect, label, val);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevConsole.CommandHandling.Commands.DevConsoleVariable
{
    public static class DevConsoleVariableRepository
    {
        private class DevConsoleFieldData
        {
            internal readonly FieldInfo FieldInfo;
            internal readonly Object Instance;
            internal readonly string VariableName;
            internal readonly Type FieldType;

            internal DevConsoleFieldData(FieldInfo fieldInfo)
            {
                FieldInfo = fieldInfo;
                Instance = GetInstanceOfField(fieldInfo);
                VariableName = fieldInfo?.GetCustomAttribute<DevConsoleVariableAttribute>()?.VariableName ?? string.Empty;
                FieldType = fieldInfo?.FieldType;
            }

            /// <summary>
            /// Returns instance of seeking FieldInfo
            /// </summary>
            /// <param name="info"></param>
            /// <returns></returns>
            private static Object GetInstanceOfField(MemberInfo info)
            {
                Type declaringType = info.DeclaringType;
                if (declaringType == null)
                {
                    return null;
                }

                if (typeof(MonoBehaviour).IsAssignableFrom(declaringType)) // for variable stored inside MonoBehaviour
                {
                    return Object.FindObjectOfType(declaringType);
                }
                // else if (typeof(ScriptableObject).IsAssignableFrom(declaringType)) // for variable stored inside ScriptableObject
                // {
                //     CreateAssetMenuAttribute createAssetMenu = declaringType.GetCustomAttribute<CreateAssetMenuAttribute>();
                //     if (createAssetMenu != null)
                //     {
                //         return Resources.Load(createAssetMenu.menuName);
                //     }
                // }

                return null;
            }
        }

        private static readonly List<DevConsoleFieldData> DevConsoleFieldsCache = new List<DevConsoleFieldData>();
        
        private static IEnumerable<DevConsoleFieldData> GetAllVariables()
        {
            if (DevConsoleFieldsCache.Count == 0)
            {
                IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
                List<DevConsoleFieldData> fields = new List<DevConsoleFieldData>();

                foreach (var assembly in assemblies)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject)))
                        {
                            IEnumerable<FieldInfo> typeFields = type
                                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where(field => field.IsDefined(typeof(DevConsoleVariableAttribute), true));

                            fields.AddRange(typeFields.Select(fieldInfo => new DevConsoleFieldData(fieldInfo)));
                        }
                    }
                }

                DevConsoleFieldsCache.AddRange(fields);
            }

            return DevConsoleFieldsCache;
        }

        private static DevConsoleFieldData GetVariableByName(string variableName)
        {
            IEnumerable<DevConsoleFieldData> allFields = GetAllVariables();
            return allFields.FirstOrDefault(field => field.VariableName.Equals(variableName));
        }
        
        internal static bool TryGetValue(string variableName, out object value, out string response)
        {
            value = null;
            response = string.Empty;

            DevConsoleFieldData data = GetVariableByName(variableName);
            if (data == null)
            {
                response = "not found!";
                return false;
            }

            if (data.Instance == null)
            {
                response = "has no instance!";
                return false;
            }

            if (data.FieldInfo == null)
            {
                response = "has no value!";
                return false;
            }

            value = data.FieldInfo.GetValue(data.Instance);
            return true;
        }
        
        internal static bool TrySetValue(string variableName, object value, out string response)
        {
            response = string.Empty;

            DevConsoleFieldData data = GetVariableByName(variableName);
            if (data == null)
            {
                response = "not found!";
                return false;
            }

            if (data.Instance == null)
            {
                response = "has no instance!";
                return false;
            }

            if (data.FieldInfo == null)
            {
                response = "has no value!";
                return false;
            }

            FieldInfo fieldInfo = data.FieldInfo;
            if (fieldInfo.IsLiteral || fieldInfo.IsInitOnly)
            {
                response = "has unsupported type to modify value";
                return false;
            }

            object parsedValue = data.FieldType switch
            {
                Type t when t == typeof(int) => Convert.ToInt32(value),
                Type t when t == typeof(float) => Convert.ToSingle(value),
                Type t when t == typeof(string) => Convert.ToString(value),
                _ => null
            };

            if (parsedValue == null)
            {
                response = "has unsupported type to modify value";
                return false;
            }

            fieldInfo.SetValue(data.Instance, parsedValue);
            return true;
        }
        
        internal static (string, Type)[] GetAllFields()
        {
            IEnumerable<DevConsoleFieldData> allFields = GetAllVariables();
            return allFields
                .Where(data => data != null)
                .Select(data => (data.VariableName, data.FieldType))
                .ToArray();
        }
    }
}
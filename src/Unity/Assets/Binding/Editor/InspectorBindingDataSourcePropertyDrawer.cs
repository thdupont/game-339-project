using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using Binding.Runtime;
using System.Linq;
using Observable.Runtime;

namespace Binding.Editor
{
    [CustomPropertyDrawer(typeof(InspectorBindingDataSource))]
    public class InspectorBindingDataSourcePropertyDrawer : PropertyDrawer
    {
        private int _cachedSelectedTypeIndex = -1;
        private string[] _cachedPropertyNameList = Array.Empty<string>();
        private string[] GetPropertyNameList(int selectedTypeIndex)
        {
            var viewModelTypeList = ViewModelTypeList;

            if (selectedTypeIndex != _cachedSelectedTypeIndex && selectedTypeIndex < viewModelTypeList.Count)
            {
                var selectedType = viewModelTypeList[selectedTypeIndex];
                var memberList = GetDataBindingProperties(selectedType);
                _cachedPropertyNameList = memberList.Select(it => it.Name).ToArray();
                _cachedSelectedTypeIndex = selectedTypeIndex;
            }

            return _cachedPropertyNameList;
        }

        private static Dictionary<Type, List<MemberInfo>> _dataBindingPropertyCache;
        private static List<MemberInfo> GetDataBindingProperties(Type viewModelType)
        {
            _dataBindingPropertyCache ??= new Dictionary<Type, List<MemberInfo>>();

            if (_dataBindingPropertyCache.TryGetValue(viewModelType, out var list))
            {
                return list;
            }

            var propertyList = viewModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fieldList = viewModelType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var memberSet = new HashSet<MemberInfo>(propertyList.Length + fieldList.Length);

            foreach (var candidate in propertyList)
            {
                if (IsEligibleProperty(candidate))
                {
                    memberSet.Add(candidate);
                }
            }

            foreach (var candidate in fieldList)
            {
                if (IsEligibleField(candidate))
                {
                    memberSet.Add(candidate);
                }
            }

            list = memberSet.OrderBy(it => it.Name).ToList();
            _dataBindingPropertyCache[viewModelType] = list;
            return list;

            bool IsEligibleProperty(PropertyInfo memberInfo)
            {
                return memberInfo.GetCustomAttribute<BindingSourceAttribute>() != null
                    || memberInfo.PropertyType.GetCustomAttribute<BindingSourceAttribute>() != null
                    || (memberInfo.PropertyType.IsGenericType && memberInfo.PropertyType.GetGenericTypeDefinition() == typeof(Property<>));
            }

            bool IsEligibleField(FieldInfo memberInfo)
            {
                return memberInfo.GetCustomAttribute<BindingSourceAttribute>() != null
                    || memberInfo.FieldType.GetCustomAttribute<BindingSourceAttribute>() != null
                    || (memberInfo.FieldType.IsGenericType && memberInfo.FieldType.GetGenericTypeDefinition() == typeof(Property<>));
            }
        }

        private static List<Type> _viewModelTypeList;
        private static List<Type> ViewModelTypeList
        {
            get
            {
                if (_viewModelTypeList != null)
                {
                    return _viewModelTypeList;
                }

                _viewModelTypeList = new List<Type>();

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetCustomAttribute<BindingSourceAssemblyAttribute>() == null)
                    {
                        continue;
                    }

                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsNotPublic || (!type.IsInterface && type.IsAbstract))
                        {
                            continue;
                        }

                        if (typeof(IViewModel).IsAssignableFrom(type))
                        {
                            _viewModelTypeList.Add(type);
                        }
                    }
                }

                _viewModelTypeList.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
                return _viewModelTypeList;
            }
        }

        private static string[] _viewModelTypeNameList;
        private static string[] ViewModelTypeNameList
        {
            get
            {
                if (_viewModelTypeNameList != null)
                {
                    return _viewModelTypeNameList;
                }

                _viewModelTypeNameList = ViewModelTypeList.Select(it => it.Name).ToArray();
                return _viewModelTypeNameList;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var selectedTypeIndex = DoTypeSelector(property);
            DoPropertySelector(property, selectedTypeIndex);
            EditorGUI.EndProperty();
        }

        private int DoTypeSelector(SerializedProperty property)
        {
            var viewModelTypeList = ViewModelTypeList;

            var assemblyQualifiedNameProperty =
                property.FindPropertyRelative(nameof(InspectorBindingDataSource._typeAssemblyQualifiedName));

            var selectedType = Type.GetType(assemblyQualifiedNameProperty.stringValue);
            var selectedIndex = viewModelTypeList.IndexOf(selectedType);
            int newSelectedTypeIndex = EditorGUILayout.Popup("View Model", selectedIndex, ViewModelTypeNameList);

            if (newSelectedTypeIndex != selectedIndex)
            {
                assemblyQualifiedNameProperty.stringValue = viewModelTypeList[newSelectedTypeIndex].AssemblyQualifiedName;
            }

            return newSelectedTypeIndex;
        }

        private void DoPropertySelector(SerializedProperty property, int selectedTypeIndex)
        {
            var propertyNameProperty =
                property.FindPropertyRelative(nameof(InspectorBindingDataSource._memberName));

            var selectedName = propertyNameProperty.stringValue;
            var propertyNameList = GetPropertyNameList(selectedTypeIndex);
            var selectedNameIndex = Array.IndexOf(propertyNameList, selectedName);

            var newSelectedNameIndex = EditorGUILayout.Popup("Property Name", selectedNameIndex, propertyNameList);

            if (newSelectedNameIndex != selectedNameIndex)
            {
                propertyNameProperty.stringValue = propertyNameList[newSelectedNameIndex];
            }
        }
    }
}
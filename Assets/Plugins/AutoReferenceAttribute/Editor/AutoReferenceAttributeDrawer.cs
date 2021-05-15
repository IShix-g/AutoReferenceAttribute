
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AutoReference
{
    [CustomPropertyDrawer(typeof(AutoReferenceAttribute))]
    public sealed class AutoReferenceAttributeDrawer : PropertyDrawer
    {
        const string ExcludeNameSpace = "UnityEngine";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            if (Application.isPlaying
                || property.propertyType != SerializedPropertyType.ObjectReference
                || IsExcludeNameSpace(fieldInfo))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var attr = attribute as AutoReferenceAttribute;
            var childOnly = attr != null && attr.ChildOnly;
            var parentStg = attr != null && !string.IsNullOrEmpty(attr.Parent) ? attr.Parent : default;
            var targetObject = property.serializedObject.targetObject;
            var isChangeValue = false;

            if (fieldInfo.FieldType.IsArray)
            {
                EditorGUI.PropertyField(position, property, label);
                
                var type = fieldInfo.FieldType.GetElementType();
                var values = fieldInfo.GetValue(targetObject) as Array;
                
                if (childOnly)
                {
                    var parent = GetParent(property, parentStg);
                    var objs = parent.GetComponentsInChildren(type).ToArray();
                    if (objs.Length > 0 && values?.Length != objs.Length)
                    {
                        var filledArray = Array.CreateInstance(type, objs.Length);
                        Array.Copy(objs, filledArray, objs.Length);
                        fieldInfo.SetValue(targetObject, filledArray);
                        isChangeValue = true;
                    }
                    else if(objs.Length == 0 && values?.Length != 0)
                    {
                        fieldInfo.SetValue(targetObject, default);
                        isChangeValue = true;
                    }
                }
                else
                {
                    var objs = Object.FindObjectsOfType(type);
                    if (objs.Length > 0 && values?.Length != objs.Length)
                    {
                        fieldInfo.SetValue(targetObject, objs);
                        isChangeValue = true;
                    }
                    else if(objs.Length == 0 && values?.Length != 0)
                    {
                        fieldInfo.SetValue(targetObject, default);
                        isChangeValue = true;
                    }
                }
            }
            else if (fieldInfo.FieldType.IsGenericType
                     && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                EditorGUI.PropertyField(position, property, label);
                
                var type = fieldInfo.FieldType.GetGenericArguments()[0];
                var values = fieldInfo.GetValue(targetObject) as IList;
                
                if (childOnly)
                {
                    var parent = GetParent(property, parentStg);
                    var objs = parent.GetComponentsInChildren(type).ToArray();
                    if (objs.Length > 0 && values?.Count != objs.Length)
                    {
                        var objArray = Array.CreateInstance(type, objs.Length);
                        Array.Copy(objs, objArray, objs.Length);
                        var listType = typeof(List<>).MakeGenericType(type);
                        var list = Activator.CreateInstance(listType, objArray);
                        fieldInfo.SetValue(targetObject, list);
                        isChangeValue = true;
                    }
                    else if(objs.Length == 0 && values?.Count != 0)
                    {
                        fieldInfo.SetValue(targetObject, default);
                        isChangeValue = true;
                    }
                }
                else
                {
                    var objs = Object.FindObjectsOfType(type);
                    if (objs.Length > 0 && values?.Count != objs.Length)
                    {
                        var listType = typeof(List<>).MakeGenericType(type);
                        var list = Activator.CreateInstance(listType, new object[]{ objs });
                        fieldInfo.SetValue(targetObject, list);
                        isChangeValue = true;
                    }
                    else if(objs.Length == 0 && values?.Count != 0)
                    {
                        fieldInfo.SetValue(targetObject, default);
                        isChangeValue = true;
                    }
                }
            }
            else
            {
                var value = fieldInfo.GetValue(targetObject) as Component;
                
                if (value != default)
                {
                    EditorGUI.PropertyField(position, property, label);
                }
                else
                {
                    var type = fieldInfo.FieldType;
                    List<Component> objs = default;
                    var nonSelectedTitle = "Not Selected";
                    var defaultColor = GUI.contentColor;
                    
                    if (childOnly)
                    {
                        Component parent = default;
                        bool parentError = true;
                        
                        if (!string.IsNullOrEmpty(parentStg))
                        {
                            var newParent = property.serializedObject.FindProperty(parentStg);
                            parent = newParent.objectReferenceValue as Component;
                            parentError = parent != default;
                        }

                        if(parent == default)
                        {
                            parent = targetObject as Component;
                        }
                        
                        objs = parent.GetComponentsInChildren(type).ToList();

                        if (!parentError)
                        {
                            GUI.contentColor = Color.red;
                            nonSelectedTitle = $"[Error] Parent ({parentStg}) is null";
                        }
                    }
                    else
                    {
                        objs = Object.FindObjectsOfType(type).Select(x => (Component)x).ToList();
                    }
                
                    if (objs.Count > 0)
                    {
                        var ids = new List<string>();
                        ids = objs.Select(GetHierarchyPath).ToList();
                        ids.Insert(0, nonSelectedTitle);
                        objs.Insert(0, null);
                    
                        var optionsArray = ids.Select(o => new GUIContent(o)).ToArray();
                        var popUpLabel = EditorGUI.BeginProperty(Rect.zero, null, property);
                        var curIndex = objs.IndexOf(value);
                        var newIndex = EditorGUI.Popup(position, popUpLabel, curIndex, optionsArray);
                        var newValue = IsIndexValid(ids, newIndex) ? objs[newIndex] : objs[0];

                        if (curIndex != newIndex)
                        {
                            fieldInfo.SetValue(targetObject, newValue);
                            isChangeValue = true;
                        }
                    }

                    GUI.contentColor = defaultColor;
                }
            }

            if (isChangeValue)
            {
                EditorUtility.SetDirty(targetObject);
                property.serializedObject.Update();
                property.serializedObject.ApplyModifiedProperties();
            }
        }
        
        Component GetParent(SerializedProperty property, string parentStg)
        {
            Component parent = default;

            if (!string.IsNullOrEmpty(parentStg))
            {
                var newParent = property.serializedObject.FindProperty(parentStg);
                parent = newParent.objectReferenceValue as Component;
            }

            if(parent == default)
            {
                parent = property.serializedObject.targetObject as Component;
            }

            return parent;
        }
        
        bool IsIndexValid<T> (List<T> list, int index)
        {
            return list.Count > 0 && index >= 0 && index < list.Count;
        }
        
        public string GetHierarchyPath(Component self)
        {
            var path = self.gameObject.name;
            var parent = self.transform.parent;
            while (parent != default)
            {
                path = $"{parent.name}/{path}";
                parent = parent.parent;
            }
            return path;
        }
        
        bool IsExcludeNameSpace(FieldInfo info)
        {
            var namespaceStg = info.FieldType.GetTypeInfo().Namespace;
            return !string.IsNullOrEmpty(namespaceStg) && namespaceStg.Contains(ExcludeNameSpace);
        }
    }
}
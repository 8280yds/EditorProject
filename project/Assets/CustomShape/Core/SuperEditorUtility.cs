using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SuperEditorUtility
{
    public static void Show<T>(SerializedProperty property, T drawerData = default(T))
    {
        SuperDrawer<T>.drawerData = drawerData;
        EditorGUILayout.PropertyField(property);
    }

    public static void ShowList(SerializedProperty property, SuperEditor listEditor)
    {
        if (!property.isArray)
        {
            EditorGUILayout.HelpBox(property.name + "数据类型出错，并非Array！", MessageType.Error);
            return;
        }
        SerializedProperty sizeProperty = property.FindPropertyRelative("Array.size");
        if (sizeProperty.hasMultipleDifferentValues)
        {
            EditorGUILayout.HelpBox(property.name + "数组长度不一致，不能共同编辑！", MessageType.Warning);
            return;
        }

        int oldIndentLevel = EditorGUI.indentLevel;
        EditorGUILayout.PropertyField(property);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel = 0;
            if (property.arraySize == 0)
            {
                if (GUILayout.Button(new GUIContent("+", "添加")))
                {
                    listEditor.listItemInsert(0, property);
                }
            }
            else
            {
                showListElements(property, listEditor);
            }
        }

        EditorGUI.indentLevel = oldIndentLevel;
    }

    private static void showListElements(SerializedProperty property, SuperEditor listEditor)
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), GUIContent.none);

            if (GUILayout.Button(new GUIContent("+", "插入"), EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
            {
                listEditor.listItemInsert(i, property);
            }
            if (GUILayout.Button(new GUIContent("-", "移除"), EditorStyles.miniButtonMid, GUILayout.Width(20f)))
            {
                listEditor.listItemDelete(i, property);
            }
            if (GUILayout.Button(new GUIContent("▲", "上移"), EditorStyles.miniButtonRight, GUILayout.Width(20f)))
            {
                if (i > 0)
                {
                    listEditor.listItemMove(i, i - 1, property);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public static void ShowList<T>(SerializedProperty property, T[] tList, SuperEditor listEditor)
    {
        if (!property.isArray)
        {
            EditorGUILayout.HelpBox(property.name + "数据类型出错，并非Array！", MessageType.Error);
            return;
        }

        if (property.arraySize != tList.Length)
        {
            throw new Exception("EditorList.ShowList()参数异常：property.arraySize与tList.Length不一致");
        }

        SerializedProperty sizeProperty = property.FindPropertyRelative("Array.size");
        if(sizeProperty.hasMultipleDifferentValues)
        {
            EditorGUILayout.HelpBox(property.name + "数组长度不一致，不能共同编辑！", MessageType.Warning);
            return;
        }

        int oldIndentLevel = EditorGUI.indentLevel;
        EditorGUILayout.PropertyField(property);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel = 0;
            if (property.arraySize == 0)
            {
                if(GUILayout.Button(new GUIContent("+", "添加")))
                {
                    listEditor.listItemInsert(0, property);
                }
            }
            else
            {
                showListElements<T>(property, tList, listEditor);
            }
        }

        EditorGUI.indentLevel = oldIndentLevel;
    }

    private static void showListElements<T>(SerializedProperty property, T[] tList, SuperEditor listEditor)
    {
        for (int i = 0; i < property.arraySize; i++ )
        {
            EditorGUILayout.BeginHorizontal();
            SuperDrawer<T>.drawerData = tList[i];
            EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), GUIContent.none);

            if(GUILayout.Button(new GUIContent("+", "插入"), EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
            {
                List<T> arrList = new List<T>(tList);
                arrList.Insert(i,tList[i]);
                tList = arrList.ToArray();
                listEditor.listItemInsert(i, property);
            }
            if (GUILayout.Button(new GUIContent("-", "移除"), EditorStyles.miniButtonMid, GUILayout.Width(20f)))
            {
                List<T> arrList = new List<T>(tList);
                arrList.RemoveAt(i);
                tList = arrList.ToArray();
                listEditor.listItemDelete(i, property);
            }
            if (GUILayout.Button(new GUIContent("∧", "上移"), EditorStyles.miniButtonRight, GUILayout.Width(20f)))
            {
                if (i > 0)
                {
                    T t = tList[i];
                    tList[i] = tList[i - 1];
                    tList[i - 1] = t;
                    listEditor.listItemMove(i, i - 1, property);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}

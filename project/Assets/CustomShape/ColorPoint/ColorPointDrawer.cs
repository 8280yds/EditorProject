using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorPoint))]
public class ColorPointDrawer : SuperDrawer<ColorPointDrawerData>
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel = 0;

        string str = "●";
        float w = 14f;
        if (drawerData != null && drawerData.index >= 0)
        {
            str += (drawerData.index + 1);
            w = 32f;
        }
        Rect rect = new Rect(position.x, position.y, w, position.height);
        EditorGUI.LabelField(rect, str);

        position.x += w;
        position.width -= w;

        SerializedProperty point = property.FindPropertyRelative("point");
        SerializedProperty color = property.FindPropertyRelative("color");

        if (drawerData != null && drawerData.index >= 0)
        {
            rect = new Rect(position.x, position.y, 0.25f * (position.width - 6), position.height);
            EditorGUI.PropertyField(rect, point.FindPropertyRelative("x"), GUIContent.none);

            rect = new Rect(rect.x + rect.width + 2, position.y, 0.25f * (position.width - 6), position.height);
            EditorGUI.PropertyField(rect, point.FindPropertyRelative("y"), GUIContent.none);

            rect = new Rect(rect.x + rect.width + 2, position.y, 0.25f * (position.width - 6), position.height);
            EditorGUI.PropertyField(rect, point.FindPropertyRelative("z"), GUIContent.none);

            rect = new Rect(rect.x + rect.width + 2, position.y, 0.25f * (position.width - 6), position.height);
            EditorGUI.PropertyField(rect, color, GUIContent.none);
        }
        else
        {
            rect = new Rect(position.x, position.y, 0.7f * position.width, position.height);
            EditorGUI.PropertyField(rect, point, GUIContent.none);

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 14f;

            rect = new Rect(rect.x + rect.width, position.y, 0.3f * position.width, position.height);
            EditorGUI.PropertyField(rect, color, new GUIContent("C"));
            EditorGUIUtility.labelWidth = labelWidth;
        }

        EditorGUI.EndProperty();
        clearDrawerData();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

}
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Triangle))]
public class TriangleDrawer : SuperDrawer<TriangleDrawerData>
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel = 0;

        string str = "▲";
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

        float labelWindth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 14f;

        rect = new Rect(position.x, position.y, position.width / 3, position.height);
        SerializedProperty pro = property.FindPropertyRelative("a");
        pro.intValue = EditorGUI.Popup(rect, pro.intValue, drawerData.strs);

        rect = new Rect(position.x + position.width / 3, position.y, position.width / 3, position.height);
        pro = property.FindPropertyRelative("b");
        pro.intValue = EditorGUI.Popup(rect, pro.intValue, drawerData.strs);

        rect = new Rect(position.x + 2 * position.width / 3, position.y, position.width / 3, position.height);
        pro = property.FindPropertyRelative("c");
        pro.intValue = EditorGUI.Popup(rect, pro.intValue, drawerData.strs);

        EditorGUIUtility.labelWidth = labelWindth;
        EditorGUI.EndProperty();
        clearDrawerData();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

}
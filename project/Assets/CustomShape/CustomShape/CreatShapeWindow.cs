using UnityEditor;
using UnityEngine;

public class CreatShapeWindow : EditorWindow
{
    private int sidesNum = 3;
    private int shapeSelectedIndex = 0;

    private string[] shapeNames = { "平面", "棱锥", "晶体", "棱柱" };

    void OnGUI()
    {
        EditorGUILayout.LabelField("请输入图形信息并创建：");
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("类型", GUILayout.Width(30));
        shapeSelectedIndex = GUILayout.SelectionGrid(shapeSelectedIndex, shapeNames, 4, EditorStyles.radioButton);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("边数", GUILayout.Width(30));
        sidesNum = EditorGUILayout.IntSlider(sidesNum, 3, 20);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(150);
        if (GUILayout.Button("创建图形", GUILayout.Height(22)))
        {
            CustomShapeMenu.creatCustomShape(sidesNum, (ShapeType)shapeSelectedIndex);
            this.Close();
        }
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
    }

}
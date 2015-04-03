using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ShapeType
{
    Polygon = 0,
    Pyramid = 1,
    Crystal = 2,
    Prism = 3
}

[CustomEditor(typeof(CustomShape))]
public class CustomShapeEditor : SuperEditor
{
    private SerializedProperty colorPoints;
    private SerializedProperty triangles;

    CustomShape customShape;

    private string[] pointNames;
    private string[] abc = { "a", "b", "c" };

    private int selectedIndex = -1;
    private float ratio = 1f;

    string[] shapeNames = { "平面", "棱锥", "晶体", "棱柱" };
    private int shapeSelectedIndex = 0;
    private int sidesNum = 3;

    private int selectedPointIndex = -1;

    void OnEnable()
    {
        colorPoints = serializedObject.FindProperty("colorPoints");
        triangles = serializedObject.FindProperty("triangles");

        customShape = (CustomShape)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        showBtns();

        ColorPointDrawerData[] datas = new ColorPointDrawerData[colorPoints.arraySize];
        pointNames = new string[colorPoints.arraySize];
        for (int i = 0; i < colorPoints.arraySize; i++)
        {
            datas[i] = new ColorPointDrawerData();
            datas[i].index = i;
            datas[i].selected = i == selectedPointIndex;
            pointNames[i] = "●" + (i + 1);
        }

        SuperEditorUtility.ShowList(colorPoints, datas, this);
        SuperEditorUtility.ShowList<TriangleDrawerData>(triangles, getTriangleDrawerDatas(triangles.arraySize), this);
        
        if (serializedObject.ApplyModifiedProperties() || 
            (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed"))
        {
            customShape.updateMesh();
        }
    }

    private TriangleDrawerData[] getTriangleDrawerDatas(int len)
    {
        TriangleDrawerData[] t = new TriangleDrawerData[len];
        for (int i = 0; i < len; i++)
        {
            t[i] = new TriangleDrawerData();
            t[i].strs = pointNames;
            t[i].index = i;
        }
        return t;
    }

    /// <summary>
    /// 显示选择按钮组
    /// </summary>
    private void showBtns()
    {
        EditorGUILayout.BeginHorizontal();

        GUIContent[] contents = { new GUIContent("☆", "图形"), new GUIContent("⊙", "缩放") };
        int len = contents.Length;
        for (int i = 0; i < len; i++)
        {
            bool selected = (selectedIndex == i);
            GUIStyle style = i == 0 ? EditorStyles.miniButtonLeft : (i == len - 1 ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid);
            if (selected != GUILayout.Toggle(selected, contents[i], style, GUILayout.Width(24)))
            {
                selectedIndex = selected ? -1 : i;
            }
        }

        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("※", "整理"), EditorStyles.miniButtonLeft, GUILayout.Width(24)))
        {
            arrange();
        }

        if (GUILayout.Button(new GUIContent("♂","清空"), EditorStyles.miniButtonRight, GUILayout.Width(24)))
        {
            clearUp();
        }

        EditorGUILayout.EndHorizontal();

        if (selectedIndex == 0)
        {
            EditorGUILayout.BeginVertical("box");
            shapeSelectedIndex = GUILayout.SelectionGrid(shapeSelectedIndex, shapeNames, 4, EditorStyles.radioButton);

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("边数", GUILayout.Width(30));
            sidesNum = EditorGUILayout.IntSlider(sidesNum, 3, 20);
            if(GUILayout.Button("创建图形"))
            {
                if ((colorPoints.arraySize==0 && triangles.arraySize==0) || EditorUtility.DisplayDialog("提示", "创建新图形将会清空当前数据，是否继续？", "是", "否"))
                {
                    creatShape(sidesNum, (ShapeType)shapeSelectedIndex);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        else if (selectedIndex == 1)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("缩放比", GUILayout.Width(40));

            EditorGUI.BeginChangeCheck();
            float newRatio = EditorGUILayout.Slider(ratio, 0.1f, 10f);
            if (EditorGUI.EndChangeCheck())
            {
                radioExpand(newRatio / ratio);
                ratio = newRatio;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 创建图形
    /// </summary>
    /// <param name="sidesNum">边数</param>
    /// <param name="type">图形类型</param>
    private void creatShape(int sidesNum, ShapeType type)
    {
        ShapeData shapeData = ShapeUtillty.creatShape(sidesNum, type);

        int len = shapeData.triangles.Count;
        triangles.arraySize = len;
        SerializedProperty property;
        for (int i = 0; i < len; i++)
        {
            property = triangles.GetArrayElementAtIndex(i);
            property.FindPropertyRelative("a").intValue = shapeData.triangles[i].a;
            property.FindPropertyRelative("b").intValue = shapeData.triangles[i].b;
            property.FindPropertyRelative("c").intValue = shapeData.triangles[i].c;
        }

        len = shapeData.points.Count;
        colorPoints.arraySize = len;
        for (int i = 0; i < len; i++)
        {
            property = colorPoints.GetArrayElementAtIndex(i);
            property.FindPropertyRelative("point").vector3Value = shapeData.points[i];
            property.FindPropertyRelative("color").colorValue = shapeData.colors[i];
        }
    }

    /// <summary>
    /// 缩放
    /// </summary>
    /// <param name="ratio">缩放比</param>
    private void radioExpand(float ratio)
    {
        SerializedProperty colorPointProperty;
        SerializedProperty pro;

        for (int i = 0; i < colorPoints.arraySize; i++)
        {
            colorPointProperty = colorPoints.GetArrayElementAtIndex(i);
            pro = colorPointProperty.FindPropertyRelative("point");
            pro.vector3Value *= ratio;
        }
    }

    /// <summary>
    /// 整理
    /// </summary>
    private void arrange()
    {
        if (colorPoints.arraySize == 0 && triangles.arraySize == 0 && EditorUtility.DisplayDialog("提示", "不存在需要整理的数据！", "确定"))
        {
            return;
        }

        int pointLen = colorPoints.arraySize;
        List<string> strs = new List<string>();
        List<int> nums = new List<int>(new int[] { 0, 0, 0 });
        bool[] pointBeUseds = new bool[pointLen];
        string str;

        SerializedProperty property;
        SerializedProperty aProperty;
        SerializedProperty bProperty;
        SerializedProperty cProperty;

        for (int i = 0; i < triangles.arraySize; i++)
        {
            property = triangles.GetArrayElementAtIndex(i);
            aProperty = property.FindPropertyRelative("a");
            bProperty = property.FindPropertyRelative("b");
            cProperty = property.FindPropertyRelative("c");

            nums[0] = aProperty.intValue;
            nums[1] = bProperty.intValue;
            nums[2] = cProperty.intValue;
            nums.Sort();
            str = nums[0] + "," + nums[1] + "," + nums[2];

            if (nums[0] == nums[1] || nums[1] == nums[2] || nums[0] < 0 || nums[0] >= pointLen || strs.IndexOf(str) >= 0)
            {
                triangles.DeleteArrayElementAtIndex(i);
                i--;
                continue;
            }
            strs.Add(str);
            aProperty.intValue = nums[0];
            bProperty.intValue = nums[1];
            cProperty.intValue = nums[2];

            pointBeUseds[nums[0]] = true;
            pointBeUseds[nums[1]] = true;
            pointBeUseds[nums[2]] = true;
        }

        nums.Clear();
        for (int i = 0; i < pointLen; i++)
        {
            if (!pointBeUseds[i + nums.Count])
            {
                nums.Add(i);
                colorPoints.DeleteArrayElementAtIndex(i);
                i--;
                pointLen--;
            }
        }

        string[] names = new string[] { "a", "b", "c" };
        for (int i = 0; i < triangles.arraySize; i++)
        {
            property = triangles.GetArrayElementAtIndex(i);
            for (int j = 0; j < 3; j++)
            {
                aProperty = property.FindPropertyRelative(names[j]);
                for (int k = 0; k < nums.Count; k++)
                {
                    if (aProperty.intValue < nums[k])
                    {
                        break;
                    }
                    aProperty.intValue--;
                }
            }
        }

        EditorUtility.DisplayDialog("提示", "数据整理完毕", "确定");
    }

    /// <summary>
    /// 清空
    /// </summary>
    private void clearUp()
    {
        if (colorPoints.arraySize > 0 || triangles.arraySize > 0)
        {
            if (EditorUtility.DisplayDialog("提示", "是否要清空所有数据？", "是", "否"))
            {
                triangles.ClearArray();
                colorPoints.ClearArray();
                ratio = 1f;
            }
        }
        else
        {
            if (EditorUtility.DisplayDialog("提示", "不存在需要清空的数据！", "确定"))
            {
                ratio = 1f;
            }
        }
    }

    public override void listItemDelete(int index, SerializedProperty property)
    {
        if (property.name == "colorPoints")
        {
            SerializedProperty triangleProperty;
            SerializedProperty pro;

            for (int i = 0; i < triangles.arraySize; i++)
            {
                triangleProperty = triangles.GetArrayElementAtIndex(i);

                for (int j = 0; j < abc.Length; j++)
                {
                    pro = triangleProperty.FindPropertyRelative(abc[j]);
                    if (pro.intValue == index)
                    {
                        triangles.DeleteArrayElementAtIndex(i);
                        i--;
                        break;
                    }
                    else if (pro.intValue > index)
                    {
                        pro.intValue--;
                    }
                }
            }
        }
        base.listItemDelete(index, property);
    }

    public override void listItemInsert(int index, SerializedProperty property)
    {
        if (property.name == "colorPoints")
        {
            SerializedProperty triangleProperty;
            SerializedProperty pro;

            for (int i = 0; i < triangles.arraySize; i++)
            {
                triangleProperty = triangles.GetArrayElementAtIndex(i);

                for (int j = 0; j < abc.Length; j++)
                {
                    pro = triangleProperty.FindPropertyRelative(abc[j]);
                    if (pro.intValue >= index)
                    {
                        pro.intValue++;
                    }
                }
            }
        }
        else if (property.name == "triangles")
        {
            if (colorPoints.arraySize < 1)
            {
                EditorUtility.DisplayDialog("提示", "请先确认是否已经创建了顶点！", "确定");
                return;
            }
        }
        base.listItemInsert(index, property);
    }

    public override void listItemMove(int srcIndex, int dstIndex, SerializedProperty property)
    {
        if (property.name == "colorPoints")
        {
            SerializedProperty triangleProperty;
            SerializedProperty pro;

            for (int i = 0; i < triangles.arraySize; i++)
            {
                triangleProperty = triangles.GetArrayElementAtIndex(i);

                for (int j = 0; j < abc.Length; j++)
                {
                    pro = triangleProperty.FindPropertyRelative(abc[j]);
                    if (pro.intValue == srcIndex)
                    {
                        pro.intValue = dstIndex;
                    }
                    else if (pro.intValue == dstIndex)
                    {
                        pro.intValue = srcIndex;
                    }
                }
            }
        }
        base.listItemMove(srcIndex, dstIndex, property);
    }

    void OnSceneGUI()
    {
        Undo.RecordObject(customShape, "Move Custom Shape Point");
        int newSelectedPointIndex = -1;

        Transform ts = customShape.transform;
        Vector3 oldPoint;
        Vector3 newPoint;
        Event evt = Event.current;

        if (evt.isMouse && evt.button == 0 && evt.type == EventType.MouseDown)
        {
            selectedPointIndex = -1;
        }

        for(int i=0; i<customShape.colorPoints.Length; i++)
        {
            if (evt.isMouse && evt.button == 0 && evt.type == EventType.MouseDown)
            {
                newSelectedPointIndex++;
            }
            oldPoint = ts.TransformPoint(customShape.colorPoints[i].point);
            newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity, 0.03f, Vector3.zero, Handles.DotCap);
            if (oldPoint != newPoint)
            {
                customShape.colorPoints[i].point = ts.InverseTransformPoint(newPoint);
                customShape.updateMesh();
            }
        }
        if (newSelectedPointIndex >= 0 && selectedPointIndex != newSelectedPointIndex)
        {
            selectedPointIndex = newSelectedPointIndex;
            Repaint();
        }
    }

}
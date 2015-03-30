using UnityEditor;
using UnityEngine;

public class CustomShapeMenu : ScriptableObject
{
    static private CreatShapeWindow win;

    [MenuItem("GameObject/Shape/CustomShape", false, 2)]
    static private void CreatShape(MenuCommand menuCommand)
    {
        if (win == null)
        {
            win = EditorWindow.CreateInstance<CreatShapeWindow>();
            win.title = "创建图形";
            win.maxSize = win.minSize = new Vector2(240, 110);
            win.ShowUtility();
        }
    }

    [MenuItem("GameObject/Shape/Polygon", false, 21)]
    static private void CreatPolygon(MenuCommand menuCommand)
    {
        creatCustomShape(3, ShapeType.Polygon);
    }

    [MenuItem("GameObject/Shape/Pyramid", false, 22)]
    static private void CreatPyramid(MenuCommand menuCommand)
    {
        creatCustomShape(3, ShapeType.Pyramid);
    }

    [MenuItem("GameObject/Shape/Crystal", false, 23)]
    static private void CreatCrystal(MenuCommand menuCommand)
    {
        creatCustomShape(3, ShapeType.Crystal);
    }

    [MenuItem("GameObject/Shape/Prism", false, 24)]
    static private void CreatPrism(MenuCommand menuCommand)
    {
        creatCustomShape(3, ShapeType.Prism);
    }

    static public void creatCustomShape(int sidesNum, ShapeType type)
    {
        string shapeName = "CustomShape";
        GameObject obj = new GameObject(shapeName);
        CustomShape customShape = obj.AddComponent<CustomShape>();
        obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("ShapeMaterial");
        setCustomShapeData(customShape, sidesNum, type);

        Selection.activeGameObject = obj;
        Undo.RegisterCreatedObjectUndo(obj, "Create " + shapeName);
    }

    static private void setCustomShapeData(CustomShape customShape, int sidesNum, ShapeType type)
    {
        ShapeData shapeData = ShapeUtillty.creatShape(sidesNum, type);

        int len = shapeData.points.Count;
        ColorPoint[] colorPoints = new ColorPoint[len];
        for (int i = 0; i < len; i++)
        {
            colorPoints[i] = new ColorPoint();
            colorPoints[i].point = shapeData.points[i];
            colorPoints[i].color = shapeData.colors[i];
        }
        customShape.colorPoints = colorPoints;
        customShape.triangles = shapeData.triangles.ToArray();
        customShape.updateMesh();
    }

}
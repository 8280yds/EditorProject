using System.Collections.Generic;
using UnityEngine;

public static class ShapeUtillty
{
    static public ShapeData creatShape(int sidesNum, ShapeType type)
    {
        ShapeData shapeData = null;
        switch(type)
        {
            case ShapeType.Polygon:
                shapeData = creatPolygon(sidesNum);
                break;
            case ShapeType.Pyramid:
                shapeData = creatPyramid(sidesNum);
                break;
            case ShapeType.Crystal:
                shapeData = creatCrystal(sidesNum);
                break;
            case ShapeType.Prism:
                shapeData = creatPrism(sidesNum);
                break;
        }
        return shapeData;
    }

    /// <summary>
    /// 创建多边形
    /// </summary>
    /// <param name="sidesNum"></param>
    /// <param name="centerPoint"></param>
    /// <returns></returns>
    static public ShapeData creatPolygon(int sidesNum, Vector3 centerPoint = default(Vector3))
    {
        ShapeData shapeData = new ShapeData();
        List<Vector3> points = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Triangle> triangles = new List<Triangle>();

        points.Add(centerPoint);
        float angle = 360f / sidesNum;

        for (int i = 0; i < sidesNum; i++)
        {
            points.Add(Quaternion.Euler(0f, angle * i, 0f) * Vector3.right + centerPoint);
        }

        for (int i = 0; i < points.Count; i++)
        {
            colors.Add(new Color(points[i].x, points[i].y, points[i].z));
        }

        for (int i = 0; i < sidesNum; i++)
        {
            triangles.Add(new Triangle(0, i + 1, i + 2 > sidesNum ? 1 : i + 2));
        }

        shapeData.points = points;
        shapeData.colors = colors;
        shapeData.triangles = triangles;
        return shapeData;
    }

    /// <summary>
    /// 创建棱锥
    /// </summary>
    /// <param name="sidesNum"></param>
    /// <returns></returns>
    static public ShapeData creatPyramid(int sidesNum)
    {
        ShapeData shapeData = creatPolygon(sidesNum, Vector3.down);
        Vector3 point = Vector3.up;
        shapeData.points.Add(point);
        shapeData.colors.Add(new Color(point.x, point.y, point.z));

        for (int i = 0; i < sidesNum; i++)
        {
            shapeData.triangles.Add(new Triangle(i + 1, i + 2 > sidesNum ? 1 : i + 2, sidesNum + 1));
        }
        return shapeData;
    }

    /// <summary>
    /// 创建晶体
    /// </summary>
    /// <param name="sidesNum"></param>
    /// <returns></returns>
    static public ShapeData creatCrystal(int sidesNum)
    {
        ShapeData shapeData = creatPolygon(sidesNum);
        Vector3 point = 2 * Vector3.down;
        shapeData.points[0] = point;
        shapeData.colors[0] = new Color(point.x, point.y, point.z);

        point = 2 * Vector3.up;
        shapeData.points.Add(point);
        shapeData.colors.Add(new Color(point.x, point.y, point.z));

        for (int i = 0; i < sidesNum; i++)
        {
            shapeData.triangles.Add(new Triangle(i + 1, i + 2 > sidesNum ? 1 : i + 2, sidesNum + 1));
        }
        return shapeData;
    }

    /// <summary>
    /// 创建棱柱
    /// </summary>
    /// <param name="sidesNum"></param>
    /// <returns></returns>
    static public ShapeData creatPrism(int sidesNum)
    {
        ShapeData shapeData = creatPolygon(sidesNum, Vector3.up);
        ShapeData shapeData2 = creatPolygon(sidesNum, Vector3.down);

        Triangle triangle;
        for (int i = 0; i < sidesNum; i++ )
        {
            triangle = shapeData2.triangles[i];
            triangle.a += sidesNum + 1;
            triangle.b += sidesNum + 1;
            triangle.c += sidesNum + 1;
        }

        shapeData.points.AddRange(shapeData2.points);
        shapeData.colors.AddRange(shapeData2.colors);
        shapeData.triangles.AddRange(shapeData2.triangles);

        for (int i = 0; i < sidesNum; i++)
        {
            triangle = new Triangle();
            triangle.a = i + 1;
            triangle.b = i + 2 > sidesNum ? 1 : i + 2;
            triangle.c = i + 1 + sidesNum + 1;
            shapeData.triangles.Add(triangle);

            triangle = new Triangle();
            triangle.a = i + 1;
            triangle.b = i == 0 ? 2 * sidesNum + 1 : i + 1 + sidesNum;
            triangle.c = i + 1 + sidesNum + 1;
            shapeData.triangles.Add(triangle);
        }
        return shapeData;
    }

}
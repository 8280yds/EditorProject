using System;

[Serializable]
public class Triangle
{
    public int a;
    public int b;
    public int c;

    public Triangle(int a = 0, int b = 0, int c = 0)
    {
        setData(a,b,c);
    }

    public void setData(int a = 0, int b = 0, int c = 0)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public bool hasIndex(int index)
    {
        return (index == a || index == b || index == c);
    }

    public int[] getIndexs()
    {
        return new int[3]{a,b,c};
    }

}

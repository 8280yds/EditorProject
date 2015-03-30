using UnityEditor;

public class SuperDrawer<T> : PropertyDrawer
{
    public static T drawerData;

    public static void clearDrawerData()
    {
        drawerData = default(T);
    }
}
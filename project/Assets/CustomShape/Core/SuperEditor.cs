using UnityEditor;

public class SuperEditor : Editor
{
    virtual public void listItemDelete(int index, SerializedProperty property)
    {
        int size = property.arraySize;
        property.DeleteArrayElementAtIndex(index);
        if (size == property.arraySize)
        {
            property.DeleteArrayElementAtIndex(index);
        }
    }

    virtual public void listItemInsert(int index, SerializedProperty property)
    {
        property.InsertArrayElementAtIndex(index);
    }

    virtual public void listItemMove(int srcIndex, int dstIndex, SerializedProperty property)
    {
        property.MoveArrayElement(srcIndex, dstIndex);
    }
}
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumData))]
public class EnumDataDrawer : PropertyDrawer
{
    private SerializedProperty _array;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnumData enumData = attribute as EnumData;
        string propertyPath = property.propertyPath;

        if (_array == null)
        {
            _array = property.serializedObject.FindProperty(propertyPath.Substring(0, propertyPath.IndexOf(".")));
            if (_array == null)
            {
                Debug.LogError("EnumData attribute must be used on an array field");
                return;
            }
        }

        if (_array.arraySize > enumData.Names.Length)
            _array.arraySize = enumData.Names.Length;

        int startIndex = propertyPath.IndexOf("[") + 1;
        int endIndex = propertyPath.IndexOf("]");

        string indexStr = propertyPath.Substring(startIndex, endIndex - startIndex);

        int index = int.Parse(indexStr);

        label.text = enumData.Names[index];

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
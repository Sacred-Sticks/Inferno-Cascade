using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    public static T CreateChild<T>(this VisualElement parent, params string[] classes) where T : VisualElement, new()
    {
        var child = new T();
        foreach (var @class in classes)
        {
            child.AddToClassList(@class);
        }
        parent.AddChild(child);
        return child;
    }

    public static VisualElement AddChild(this VisualElement parent, VisualElement child)
    {
        parent.hierarchy.Add(child);
        return child;
    }
}
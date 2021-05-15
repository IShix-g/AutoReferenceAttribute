
using System;
using System.Diagnostics;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
[Conditional("UNITY_EDITOR")]
public sealed class AutoReferenceAttribute : PropertyAttribute
{
    /// <summary>
    /// Only child objects of gameObjects will be targeted.
    /// </summary>
    public readonly bool ChildOnly;
    /// <summary>
    /// Specify the Transform to be the parent as a string
    /// </summary>
    public readonly string Parent;

    public AutoReferenceAttribute()
    {
        ChildOnly = false;
        Parent = string.Empty;
    }
    
    public AutoReferenceAttribute(bool childOnly)
    {
        ChildOnly = childOnly;
        Parent = string.Empty;
    }
    
    public AutoReferenceAttribute(string parent)
    {
        ChildOnly = !string.IsNullOrEmpty(parent);
        Parent = parent;
    }
}

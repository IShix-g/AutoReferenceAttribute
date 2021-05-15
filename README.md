# Auto Reference Attribute

Automatically resolves `SerializeField` references.
It collects the references on the Hierarchy and displays them in a drop-down.

## Field

```c#
[SerializeField, AutoReference] AutoReferenceExampleObject obj;
```

![Auto Reference](ReadMeImages/image.png)

## Array and List
```c#
[SerializeField, AutoReference] AutoReferenceExampleObject[] obj;
[SerializeField, AutoReference] List<AutoReferenceExampleObject> obj;
```
![childOnly](ReadMeImages/image4.png)

## Just give it an `AutoReference` attribute.

```c#
[SerializeField, AutoReference] MyScript script;
```

# Why use Auto Reference Attribute?
- It has better performance than `GetComponent`.
- You can check it in Hierarchy, so it is intuitive.
- It's easy to add `AutoReference`.
- If a script goes missing, you can reconfigure it by simply selecting it.

# Available Types
- Class(MonoBehaviour)
- Class(MonoBehaviour)[] array
- List<Class(MonoBehaviour)> List

> - It is only supported on Hierarchy.
> - UnityEngine's api is NOT supported due to performance reasons.
> - You can get up to child objects.

# Notes on Arrays and Lists

For some reasons, it will not work with **0**, so enter **1**.

![Notes on Arrays and Lists](ReadMeImages/image6.png)

# Unity Version
Unity 2018.4 higher

# Getting started

## Package Manager

URL : `https://github.com/IShix-g/AutoReferenceAttribute.git?path=Assets/Plugins/AutoReferenceAttribute`

### [Unity 2019.3 higher] Install via git URL
Add the Url to Package Manager

![UPM Package](ReadMeImages/image2.png)

### [Unity 2018.4 higher] Add URL manually

Add the Url to `Packages/manifest.json`.

## Unitypackage

[SerializeFieldAutoReferenceAttribute/releases](https://github.com/IShix-g/AutoReferenceAttribute/releases/)

![Auto Reference](ReadMeImages/image3.png)

# Options

## childOnly
For transform children only.

```c#
[SerializeField, AutoReference(childOnly:true)] AutoReferenceExampleObject objChildOnly;
```

![childOnly](ReadMeImages/image4.png)

## parent
By passing transform as a string, only the children of the selected transform will be targeted.

```c#
[SerializeField] Transform parentTransform;
[SerializeField, AutoReference(parent:"parentTransform")] List<AutoReferenceExampleObject> objList;
```

![parent](ReadMeImages/image5.png)
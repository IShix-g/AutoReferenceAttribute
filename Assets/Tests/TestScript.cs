
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour
{
    [SerializeField, AutoReference] TestObject obj;
    [SerializeField, AutoReference(childOnly:true)] TestObject objChildOnly;
    [SerializeField, AutoReference(parent:"parentTransform")] TestObject objFromParent;
    [SerializeField] Transform parentTransform;
        
    [SerializeField, AutoReference(childOnly:true)] TestObject[] objArray;
    [SerializeField, AutoReference(parent:"parentTransform")] List<TestObject> objList;
}

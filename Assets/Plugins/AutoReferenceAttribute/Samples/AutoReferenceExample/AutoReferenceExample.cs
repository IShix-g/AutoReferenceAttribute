#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace AutoReference.Example
{
    public class AutoReferenceExample : MonoBehaviour
    {
        [SerializeField, AutoReference] AutoReferenceExampleObject obj;
        [SerializeField, AutoReference(childOnly:true)] AutoReferenceExampleObject objChildOnly;
        [SerializeField, AutoReference(parent:"parentTransform")] AutoReferenceExampleObject objFromParent;
        [SerializeField] Transform parentTransform;
        
        [SerializeField, AutoReference(childOnly:true)] AutoReferenceExampleObject[] objArray;
        [SerializeField, AutoReference(parent:"parentTransform")] List<AutoReferenceExampleObject> objList;
    }
}
#endif
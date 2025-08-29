using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropCatalog", menuName = "Scriptable Objects/PropCatalog")]
public class PropCatalog : ScriptableObject
{
    public List<Entry> entries = new List<Entry>();

    [Serializable]
    public class Entry 
    {
        public string id;
        public Mesh mesh;
    }
}

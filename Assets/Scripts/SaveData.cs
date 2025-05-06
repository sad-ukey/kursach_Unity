using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlacedObjectData
{
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public int sizeX;
    public int sizeZ;
}

[Serializable]
public class SaveData
{
    public List<PlacedObjectData> placedObjects = new List<PlacedObjectData>();
    public int currency = 0; // можно расширять
}

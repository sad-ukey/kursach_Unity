using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacedObjectData
{
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public int sizeX;
    public int sizeZ;
}

[System.Serializable]
public class SaveData
{
    public int savedMoney = 0;
    public List<PlacedObjectData> placedObjects = new List<PlacedObjectData>();
}

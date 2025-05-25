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
    public int buildingLevel = 1;
    public float buildingHealth = 100f;
}


[System.Serializable]
public class StringIntPair
{
    public string key;
    public int value;
}

[System.Serializable]
public class SaveData
{
    public int savedMoney = 0;
    public bool isRatushaBuilt = false;

    public List<PlacedObjectData> placedObjects = new List<PlacedObjectData>();

    public List<StringIntPair> placedCountList = new List<StringIntPair>();

}
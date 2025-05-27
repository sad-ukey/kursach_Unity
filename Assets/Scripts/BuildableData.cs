using UnityEngine;

[System.Serializable]
public class BuildableData
{
    public GameObject prefab;
    public int sizeX = 1;
    public int sizeZ = 1;
    public int cost = 10; 

    public int buildLimit = -1;
}

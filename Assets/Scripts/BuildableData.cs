using UnityEngine;

[System.Serializable]
public class BuildableData
{
    public GameObject prefab;
    public int sizeX = 1;
    public int sizeZ = 1;
    public int cost = 10; // стоимость постройки в рублях

    public int buildLimit = -1;
}

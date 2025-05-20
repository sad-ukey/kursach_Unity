using UnityEngine;

[CreateAssetMenu(menuName = "Building Info", fileName = "New Building Info")]
public class BuildingInfo : ScriptableObject
{
    public string buildingName;
    public string description;
    public int buildingLevel = 1;
    public string buildingType;
    public float buildingHealth = 100f;

    public int upgradeCost = 50;
    public float healthIncrease = 25f;

    public void Upgrade()
    {
        buildingLevel++;
        buildingHealth += healthIncrease;
    }
}
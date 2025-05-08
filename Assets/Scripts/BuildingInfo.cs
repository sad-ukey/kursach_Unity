using UnityEngine;

[CreateAssetMenu(menuName = "Building Info", fileName = "New Building Info")]
public class BuildingInfo : ScriptableObject
{
    public string buildingName;
    public string description;
    public string buildingLevel;
    public string buildingType;
    public string buildingHealth;
}

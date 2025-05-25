using UnityEngine;

public class BuildingState : MonoBehaviour
{
    public BuildingInfo template;
    public int currentLevel = 1;
    public float currentHealth = 100f;

    public void Upgrade()
    {
        if (template.buildingName == "Ратуша")
        {
            currentLevel++;
            currentHealth += template.healthIncrease;
            return;
        }

        int maxLevel = TownhallManager.Instance.GetMaxAllowedLevel();

        if (currentLevel >= maxLevel)
        {
            Debug.Log("Нельзя улучшить: достигнут максимум уровня, ограниченный уровнем ратуши.");
            return;
        }

        currentLevel++;
        currentHealth += template.healthIncrease;
        AchievementManager.Instance.IncrementProgress("Начинающий инженер", 1);
        AchievementManager.Instance.IncrementProgress("Эксперт по улучшениям", 1);
    }
}
using UnityEngine;
using UnityEditor;

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

    public void OnDestroyed()
    {
        if (template != null && template.buildingName == "Склад")
        {
            int currentMoney = CurrencyManager.Instance.GetCurrentMoney();
            int loss = Mathf.FloorToInt(currentMoney * 0.1f);
            CurrencyManager.Instance.Spend(loss);
            Debug.Log($"Склад уничтожен — потеряно {loss} Р. (10% от средств).");
        }

        if (template != null && template.buildingName == "Ратуша")
        {
            Debug.LogError("Вы проиграли — ратуша уничтожена.");
            EditorApplication.isPlaying = false;
        }    
    }
}
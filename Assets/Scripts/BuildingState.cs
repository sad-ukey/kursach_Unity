using UnityEngine;

public class BuildingState : MonoBehaviour
{
    public BuildingInfo template;
    public int currentLevel = 1;
    public float currentHealth = 100f;

    public void Upgrade()
    {
        if (template.buildingName == "������")
        {
            currentLevel++;
            currentHealth += template.healthIncrease;
            return;
        }

        int maxLevel = TownhallManager.Instance.GetMaxAllowedLevel();

        if (currentLevel >= maxLevel)
        {
            Debug.Log("������ ��������: ��������� �������� ������, ������������ ������� ������.");
            return;
        }

        currentLevel++;
        currentHealth += template.healthIncrease;
        AchievementManager.Instance.IncrementProgress("���������� �������", 1);
        AchievementManager.Instance.IncrementProgress("������� �� ����������", 1);
    }
}
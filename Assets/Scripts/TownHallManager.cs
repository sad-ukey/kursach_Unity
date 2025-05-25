using UnityEngine;

public class TownhallManager : MonoBehaviour
{
    public static TownhallManager Instance;

    public int currentLevel = 1;
    public int maxPossibleLevel = 4; 

    private void Awake()
    {
        Instance = this;
    }

    public int GetMaxAllowedLevel()
    {
        return Mathf.Min(currentLevel, maxPossibleLevel);
    }

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, maxPossibleLevel);
        Debug.Log("������ ����������� �� ������� " + currentLevel);
    }

    public void UpgradeTownhall()
    {
        if (currentLevel >= maxPossibleLevel)
        {
            Debug.Log("��������� ������������ ������� ������.");
            return;
        }

        currentLevel++;
        Debug.Log("������ �������� �� ������ " + currentLevel);
    }
}
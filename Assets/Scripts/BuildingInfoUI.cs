using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoUI : MonoBehaviour
{
    public static BuildingInfoUI Instance;

    public GameObject panel;
    public Text titleText;
    public Text descriptionText;
    public Text levelText;
    public Text healthText;
    public Text typeText;
    public Button upgradeButton;

    private BuildingState currentState;


    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowInfo(BuildingState state)
    {
        panel.SetActive(true);
        currentState = state;
        var info = state.template;

        titleText.text = info.buildingName;
        descriptionText.text = info.description;
        levelText.text = "�������: " + state.currentLevel;
        healthText.text = "��������: " + state.currentHealth;
        typeText.text = "���: " + info.buildingType;
    }

    public void UpgradeBuilding()
    {
        if (currentState == null) return;

        bool isTownhall = currentState.template.buildingName == "������";
        int maxLevel = TownhallManager.Instance.maxPossibleLevel; 

        if (currentState.currentLevel >= maxLevel)
        {
            Debug.Log("������ ��������: ��������� ������������ ������� (" + maxLevel + ").");
            return;
        }

        int cost = currentState.template.upgradeCost;
        if (CurrencyManager.Instance.HasEnough(cost))
        {
            CurrencyManager.Instance.Spend(cost);
            currentState.Upgrade();

            if (isTownhall)
            {
                TownhallManager.Instance.SetLevel(currentState.currentLevel);
            }

            ShowInfo(currentState);
        }
        else
        {
            Debug.Log("������������ ������� ��� ���������.");
        }
    }

    public void Close()
    {
        panel.SetActive(false);
        currentState = null;
    }
}
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

    private BuildingInfo currentBuilding;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowInfo(BuildingInfo info)
    {
        currentBuilding = info;

        titleText.text = info.buildingName;
        levelText.text = "Уровень: " + info.buildingLevel.ToString();
        healthText.text = "Здоровье: " + info.buildingHealth.ToString();
        typeText.text = "Тип: " + info.buildingType;
        descriptionText.text = info.description;

        panel.SetActive(true);
    }

    public void UpgradeBuilding()
    {
        if (currentBuilding == null) return;

        int cost = currentBuilding.upgradeCost;
        if (CurrencyManager.Instance.HasEnough(cost))
        {
            CurrencyManager.Instance.Spend(cost);
            currentBuilding.Upgrade();
            ShowInfo(currentBuilding); // Обновляем отображение
        }
        else
        {
            Debug.Log("Недостаточно средств для улучшения.");
        }
    }

    public void Close()
    {
        panel.SetActive(false);
        currentBuilding = null;
    }
}
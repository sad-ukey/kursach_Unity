using UnityEngine;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int currentMoney = 100;
    public Text moneyText;

    private int baseMoneyLimit = 1000;
    private int storagePerWarehouse = 5000;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public bool HasEnough(int amount)
    {
        return currentMoney >= amount;
    }

    public void Spend(int amount)
    {
        currentMoney -= amount;
        UpdateUI();
    }

    public void Add(int amount)
    {

        currentMoney += amount;
        currentMoney = Mathf.Min(currentMoney, GetMoneyLimit());
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "₽ " + currentMoney.ToString() + " / " + GetMoneyLimit();
        }

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.SetProgress("Молодой бизнесмен", currentMoney);
        }
    }

    public void LoadMoney(int amount)
    {
        currentMoney = amount;
        currentMoney = Mathf.Min(currentMoney, GetMoneyLimit());
        UpdateUI();
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public int GetMoneyLimit()
    {
        return baseMoneyLimit + CountWarehouses() * storagePerWarehouse;
    }

    private int CountWarehouses()
    {
        int count = 0;
        foreach (var building in FindObjectsOfType<BuildingState>())
        {
            if (building.template != null && building.template.buildingName == "Склад")
            {
                count++;
            }
        }
        return count;
    }
}

   
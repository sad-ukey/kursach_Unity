using UnityEngine;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int currentMoney = 100;
    public Text moneyText;

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
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "₽ " + currentMoney.ToString();
        }

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.SetProgress("Молодой бизнесмен", currentMoney);
        }
    }

    public void LoadMoney(int amount)
    {
        currentMoney = amount;
        UpdateUI();
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }
}

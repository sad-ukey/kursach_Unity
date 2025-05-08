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

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowInfo(BuildingInfo info)
    {
        titleText.text = info.buildingName;
        levelText.text = info.buildingLevel;
        healthText.text = info.buildingHealth;
        typeText.text = info.buildingType;
        descriptionText.text = info.description;

        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<Achievement> achievements = new List<Achievement>();

    public GameObject achievementPanel; // Панель UI
    public GameObject achievementItemPrefab; // Префаб UI-элемента достижения
    public Transform achievementListParent; // Родитель UI-списка

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        achievementPanel.SetActive(false);
        PopulateUI();
    }

    public void IncrementProgress(string title, int amount = 1)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.title == title && !achievement.isCompleted)
            {
                achievement.currentProgress += amount;
                if (achievement.currentProgress >= achievement.requiredProgress)
                    achievement.currentProgress = achievement.requiredProgress;

                UpdateUI();
                break;
            }
        }
    }

    void PopulateUI()
    {
        foreach (Transform child in achievementListParent)
            Destroy(child.gameObject);

        foreach (var achievement in achievements)
        {
            Debug.Log("Создал достижение: " + achievement.title);
            GameObject item = Instantiate(achievementItemPrefab);
            item.transform.SetParent(achievementListParent, false); // <--- ключевой момент
            var ui = item.GetComponent<AchievementUI>();
            ui.SetData(achievement);
        }
    }

    void UpdateUI()
    {
        foreach (Transform child in achievementListParent)
        {
            var ui = child.GetComponent<AchievementUI>();
            ui.SetData(achievements.Find(a => a.title == ui.title));
        }
    }

    public void TogglePanel()
    {
        achievementPanel.SetActive(!achievementPanel.activeSelf);
    }
}
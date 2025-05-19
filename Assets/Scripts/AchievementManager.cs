using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<Achievement> achievements = new List<Achievement>();
    public GameObject achievementPanel;
    public Transform achievementListParent;  // Родитель UI элементов достижений
    public GameObject achievementUIPrefab;   // Префаб UI элемента достижения

    private List<AchievementUI> achievementUIList = new List<AchievementUI>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        achievementPanel.SetActive(false);
        InitializeUI();
        UpdateUI();
    }

    public Dictionary<string, int> GetAllProgress()
    {
        Dictionary<string, int> progressMap = new Dictionary<string, int>();
        foreach (var achievement in achievements)
        {
            progressMap[achievement.title] = achievement.currentProgress;
        }
        return progressMap;
    }
    // Создаем UI элементы для каждого достижения
    private void InitializeUI()
    {
        foreach (Transform child in achievementListParent)
        {
            Destroy(child.gameObject);
        }
        achievementUIList.Clear();

        foreach (var achievement in achievements)
        {
            GameObject uiObj = Instantiate(achievementUIPrefab, achievementListParent);
            AchievementUI ui = uiObj.GetComponent<AchievementUI>();
            ui.SetData(achievement);
            achievementUIList.Add(ui);
        }
    }

    // Обновляем все UI элементы (например, при изменении прогресса)
    public void UpdateUI()
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            if (i < achievementUIList.Count)
            {
                achievementUIList[i].SetData(achievements[i]);
            }
        }
    }

    // Метод для установки прогресса достижения (например, при накоплении денег)
    public void SetProgress(string title, int progress)
    {
        Achievement achievement = achievements.Find(a => a.title == title);
        if (achievement != null)
        {
            if (progress > achievement.currentProgress)
            {
                achievement.currentProgress = Mathf.Min(progress, achievement.requiredProgress);
                if (achievement.isCompleted)
                {
                    Debug.Log($"Достижение выполнено: {achievement.title}");
                    NotificationManager.Instance.ShowAchievement(achievement.title);
                }
                UpdateUI();
            }
        }
        else
        {
            Debug.LogWarning($"Достижение с названием '{title}' не найдено.");
        }
    }

    // Метод для увеличения прогресса на определенное значение
    public void IncrementProgress(string title, int increment = 1)
    {
        Achievement achievement = achievements.Find(a => a.title == title);
        if (achievement != null && !achievement.isCompleted)
        {
            achievement.currentProgress = Mathf.Min(achievement.currentProgress + increment, achievement.requiredProgress);
            if (achievement.isCompleted)
            {
                Debug.Log($"Достижение выполнено: {achievement.title}");
                NotificationManager.Instance.ShowAchievement(achievement.title);
            }
            UpdateUI();
        }
    }

    public void LoadProgress(Dictionary<string, int> achievementProgressMap)
    {
        foreach (var kvp in achievementProgressMap)
        {
            Achievement achievement = achievements.Find(a => a.title == kvp.Key);
            if (achievement != null)
            {
                achievement.currentProgress = Mathf.Clamp(kvp.Value, 0, achievement.requiredProgress);
            }
        }
        UpdateUI();
    }

    public void TogglePanel()
    {
        achievementPanel.SetActive(!achievementPanel.activeSelf);
    }
}
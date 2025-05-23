using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<Achievement> achievements = new List<Achievement>();
    public GameObject achievementPanel;
    public Transform achievementListParent;
    public GameObject achievementUIPrefab;

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
                    NotificationManager.Instance?.ShowAchievement(achievement.title);
                }
                UpdateUI();
            }
        }
    }

    public void IncrementProgress(string title, int increment = 1)
    {
        Achievement achievement = achievements.Find(a => a.title == title);
        if (achievement != null && !achievement.isCompleted)
        {
            achievement.currentProgress = Mathf.Min(achievement.currentProgress + increment, achievement.requiredProgress);
            if (achievement.isCompleted)
            {
                Debug.Log($"Достижение выполнено: {achievement.title}");
                NotificationManager.Instance?.ShowAchievement(achievement.title);
            }
            UpdateUI();
        }
    }

    public List<StringIntPair> GetAchievementProgressList()
    {
        List<StringIntPair> list = new List<StringIntPair>();
        foreach (var achievement in achievements)
        {
            list.Add(new StringIntPair { key = achievement.title, value = achievement.currentProgress });
        }
        return list;
    }

    public void LoadProgressFromList(List<StringIntPair> progressList)
    {
        foreach (var entry in progressList)
        {
            Achievement achievement = achievements.Find(a => a.title == entry.key);
            if (achievement != null)
            {
                achievement.currentProgress = Mathf.Clamp(entry.value, 0, achievement.requiredProgress);
            }
        }
        UpdateUI();
    }

    public void TogglePanel()
    {
        achievementPanel.SetActive(!achievementPanel.activeSelf);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    public Text titleText;
    public Text descriptionText;
    public Text progressNumber;
    public Slider progressBar;
    public GameObject checkmark;

    private Achievement currentAchievement;

    // Публичное свойство для доступа к названию достижения из других скриптов
    public string Title => currentAchievement != null ? currentAchievement.title : "";

    // Устанавливаем данные для отображения
    public void SetData(Achievement achievement)
    {
        if (achievement == null) return;

        currentAchievement = achievement;

        titleText.text = achievement.title;
        descriptionText.text = achievement.description;
        progressBar.maxValue = achievement.requiredProgress;
        progressBar.value = achievement.currentProgress;
        progressNumber.text = $"{achievement.currentProgress} / {achievement.requiredProgress}";

        checkmark.SetActive(achievement.isCompleted);
    }
}

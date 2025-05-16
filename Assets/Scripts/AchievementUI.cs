using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    public Text titleText;
    public Text descriptionText;
    public Slider progressBar;
    public GameObject checkmark;

    [HideInInspector]
    public string title;

    public void SetData(Achievement data)
    {
        title = data.title;
        titleText.text = data.title;
        descriptionText.text = data.description;
        progressBar.maxValue = data.requiredProgress;
        progressBar.value = data.currentProgress;

        checkmark.SetActive(data.isCompleted);
    }
}
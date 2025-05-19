using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public GameObject notificationPrefab;
    public Transform notificationParent;  
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowAchievement(string title)
    {
        GameObject obj = Instantiate(notificationPrefab, notificationParent);
        obj.GetComponent<Notification>().Show(title);
        Debug.Log("Вызов уведомления");
    }
}
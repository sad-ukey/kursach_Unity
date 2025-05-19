using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Notification : MonoBehaviour
{
    public Text titleText;
    public float displayTime = 3f;

    public void Show(string title)
    {
        titleText.text = $"{title}";
        gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        Destroy(gameObject);
    }
}

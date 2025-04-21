using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SidebarController : MonoBehaviour
{
    public RectTransform sidebar;
    public Button openMenuButton;
    public Button hideSidebarButton;
    public float animationDuration = 0.2f;

    public GameObject zaboryContent;
    public GameObject zdaniyaContent;
    public GameObject oruzhieContent;

    private Vector2 closedPos;
    private Vector2 openPos;
    private bool isOpen = false;

    void Start()
    {
        openPos   = sidebar.anchoredPosition;
        closedPos = new Vector2(-sidebar.rect.width, openPos.y);

        // 1) скрываем панель и кнопку «✖»
        sidebar.anchoredPosition = closedPos;
        hideSidebarButton.gameObject.SetActive(false);

        // 2) кнопка «Меню строительства» видна
        openMenuButton.gameObject.SetActive(true);

        // 3) подписываемся на нажатия
        openMenuButton.onClick.AddListener(ToggleSidebar);
        hideSidebarButton.onClick.AddListener(ToggleSidebar);

        ShowZabory();
    }

    public void ToggleSidebar()
    {
        // переключение видимости кнопок
        openMenuButton.gameObject.SetActive(isOpen);
        hideSidebarButton.gameObject.SetActive(!isOpen);

        StopAllCoroutines();
        StartCoroutine(AnimateSidebar(isOpen ? closedPos : openPos));

        isOpen = !isOpen;
    }

    private IEnumerator AnimateSidebar(Vector2 target)
    {
        Vector2 start = sidebar.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            sidebar.anchoredPosition = Vector2.Lerp(start, target, elapsed / animationDuration);
            yield return null;
        }
        sidebar.anchoredPosition = target;
    }

    public void ShowZabory()
    {
        zaboryContent.SetActive(true);
        zdaniyaContent.SetActive(false);
        oruzhieContent.SetActive(false);
    }
    public void ShowZdaniya()
    {
        zaboryContent.SetActive(false);
        zdaniyaContent.SetActive(true);
        oruzhieContent.SetActive(false);
    }
    public void ShowOruzhie()
    {
        zaboryContent.SetActive(false);
        zdaniyaContent.SetActive(false);
        oruzhieContent.SetActive(true);
    }
}

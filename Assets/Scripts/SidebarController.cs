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

    public Button skladButton;
    public Button xpGeneratorButton;
    public Button cannonButton;
    public Button crossbowButton;
    public Button ratushaButton;

    private Vector2 closedPos;
    private Vector2 openPos;
    private bool isOpen = false;

    private BuildManager buildManager;

    void Start()
    {
        openPos = sidebar.anchoredPosition;
        closedPos = new Vector2(-sidebar.rect.width, openPos.y);

        // 1) скрываем панель и кнопку «✖»
        sidebar.anchoredPosition = closedPos;
        hideSidebarButton.gameObject.SetActive(false);

        // 2) кнопка «Меню строительства» видна
        openMenuButton.gameObject.SetActive(true);

        // 3) подписываемся на нажатия
        openMenuButton.onClick.AddListener(ToggleSidebar);
        hideSidebarButton.onClick.AddListener(ToggleSidebar);

        // 4) находим BuildManager
        buildManager = FindObjectOfType<BuildManager>();
        if (buildManager == null)
        {
            Debug.LogError("BuildManager не найден на сцене!");
        }

        // 5) назначаем действия на кнопки
        if (ratushaButton != null)
            ratushaButton.onClick.AddListener(() => buildManager.StartRatushaPlacement());

        if (skladButton != null)
            skladButton.onClick.AddListener(() => buildManager.StartSkladPlacement());

        if (xpGeneratorButton != null)
            xpGeneratorButton.onClick.AddListener(() => buildManager.StartXPGeneratorPlacement());

        if (cannonButton != null)
            cannonButton.onClick.AddListener(() => buildManager.StartCannonPlacement());

        if (crossbowButton != null)
            crossbowButton.onClick.AddListener(() => buildManager.StartCrossbowPlacement());

        // 6) обновляем состояние кнопок в зависимости от наличия ратуши
        UpdateBuildingButtons();

        ShowZabory();
    }

    public void ToggleSidebar()
    {
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

    public void UpdateBuildingButtons()
    {
        bool isUnlocked = buildManager != null && buildManager.IsRatushaBuilt();

        if (skladButton != null)
            skladButton.interactable = isUnlocked;
        if (xpGeneratorButton != null)
            xpGeneratorButton.interactable = isUnlocked;
        if (cannonButton != null)
            cannonButton.interactable = isUnlocked;
        if (crossbowButton != null)
            crossbowButton.interactable = isUnlocked;
    }
}

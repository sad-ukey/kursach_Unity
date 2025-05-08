using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingInfoOpener : MonoBehaviour
{
    public BuildingInfo data; // скриптовый объект с описанием здания

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (data == null)
        {
            Debug.LogError("BuildingInfo не назначен!");
            return;
        }

        BuildingInfoUI.Instance.ShowInfo(data);
    }
}
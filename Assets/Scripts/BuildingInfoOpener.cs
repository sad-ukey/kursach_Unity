using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingInfoOpener : MonoBehaviour
{

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        var state = GetComponent<BuildingState>();
        if (state == null)
        {
            Debug.LogError("BuildingState не найден!");
            return;
        }

        BuildingInfoUI.Instance.ShowInfo(state);
    }
}
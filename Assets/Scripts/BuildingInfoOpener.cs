using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingInfoOpener : MonoBehaviour
{
    public BuildingInfo data; // ���������� ������ � ��������� ������

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (data == null)
        {
            Debug.LogError("BuildingInfo �� ��������!");
            return;
        }

        BuildingInfoUI.Instance.ShowInfo(data);
    }
}
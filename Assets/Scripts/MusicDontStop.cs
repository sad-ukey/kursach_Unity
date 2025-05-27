using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    void Awake()
    {
        BackgroundMusic[] instances = FindObjectsOfType<BackgroundMusic>();

        if (instances.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
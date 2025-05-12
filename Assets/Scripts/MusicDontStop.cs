using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    void Awake()
    {
        // Находим все объекты с этим компонентом
        BackgroundMusic[] instances = FindObjectsOfType<BackgroundMusic>();

        // Если уже есть такой объект — удаляем новый, чтобы не было дублирования
        if (instances.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Не уничтожаем объект при смене сцены
        DontDestroyOnLoad(gameObject);
    }
}
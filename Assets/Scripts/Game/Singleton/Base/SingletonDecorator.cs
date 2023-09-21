using UnityEngine;

public abstract class SingletonDecorator<T>
    where T : MonoBehaviour
{
    public SingletonDecorator(MonoBehaviour behavior)
    {
        if (Instance == null)
        {
            Instance = behavior.GetComponent<T>();
        }
        else
        {
            GameObject.Destroy(behavior.gameObject);
        }
    }

    public static T Instance { get; protected set; }
}
using UnityEngine;
public abstract class Manager<T> : MonoBehaviour where T : MonoBehaviour
{
    //This class uses a modified version of the Singleton pattern to create a manager paraent class that allows an instance of a manager to be instantiated but 
    //if one already exists of that type it deletes itself to prevent duplicates
    private static T instance;

    public static T Instance
    {
        get { return instance; }
        set
        {
            if (null == instance)
            {
                instance = value;
                DontDestroyOnLoad(instance);
            }
            else if (instance != value)
            {
                Destroy(value.gameObject);
            }
        }
    }

    virtual public void Awake()
    {
        Instance = this as T;
    }
}

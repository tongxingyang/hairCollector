using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSingleton<T> : MonoBehaviour where T : TSingleton<T>
{

    private static volatile T uniqueInstance = null;
    private static volatile GameObject uniqueObject = null;

    protected TSingleton()
    {

    }

    public static T instance
    {
        get
        {
            if (uniqueInstance == null)
            {
                lock (typeof(T))
                {
                    if (uniqueInstance == null && uniqueObject == null)
                    {
                        uniqueObject = new GameObject(typeof(T).Name, typeof(T));
                        uniqueInstance = uniqueObject.GetComponent<T>();

                        uniqueInstance.Init();
                    }
                }
            }
            return uniqueInstance;
        }
    }

    protected virtual void Init()
    {
        DontDestroyOnLoad(gameObject);
    }
}

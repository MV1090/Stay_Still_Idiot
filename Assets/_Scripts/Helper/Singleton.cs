using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{

    static T instance;

    public static T Instance
    {
        get
        {
            if (instance != null)
            return instance;

            GameObject obj = new GameObject();
            obj.name = typeof(T).Name;
            instance = obj.AddComponent<T>();
            DontDestroyOnLoad(obj);                               

            return instance;    
        }
    }


    protected virtual void Awake()
    {
        if (!instance)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            
            return;
        }

        Destroy(gameObject);
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

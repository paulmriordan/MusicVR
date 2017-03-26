using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T s_Instance;
    private static bool s_IsDestroyed;

    //Alternate Awake and Enable methods provided. 
    //The normal methods cannot be used, because this superclass is using 
    //them to listen to Unity event, and does not want to rely on subclasses calling base.Awake
    virtual protected void _Awake(){}
    virtual protected void _OnEnable(){}
    virtual protected void _OnDestroy(){}
    
    public static T Instance
    {
        get
        {
            if (s_IsDestroyed)
                return null;

            return s_Instance;
        }
    }

    private void SetupInstance()
    {
        s_Instance = this as T;
    }

    //Do not hide/override. Use _Awake instead
    protected void Awake()
    {
        SetupInstance();
        _Awake();
    }

    //Do not hide/override. Use _OnEnable instead
    protected void OnEnable()
    {
        SetupInstance();
        _OnEnable();
    }

    //Do not hide/override. Use _OnDestroy instead
    protected void OnDestroy()
    {
        _OnDestroy();

        if (s_Instance)
            Destroy(s_Instance);
        
        s_Instance = null;
        s_IsDestroyed = true;
    }
}
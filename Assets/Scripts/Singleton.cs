using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Inheriting classes will become singletons.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    #region Singleton
    private static T _instance;
    public static T Instance { get
        {
            if (!IsInitialised) {
                var method = new StackTrace().GetFrame(1).GetMethod();
                UnityEngine.Debug.LogWarning($"{typeof(Singleton<T>)} was accessed before it was initialised.\nCalled from: {method.ReflectedType.Name}.{method.Name}");
            }
            return _instance;
        }
    }
    /// <summary>
    /// True if the singleton has been initialised. Note this does not guarantee that <see cref="Instance"/> is not null (may be on exit).
    /// </summary>
    public static bool IsInitialised { get; private set; } = false;
    public static bool TryGetInstance(out T singleton)
    {
        singleton = _instance;
        return _instance != null;
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = (T)this;
            IsInitialised = true;
        }

    }
    #endregion
}

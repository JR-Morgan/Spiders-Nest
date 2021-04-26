using UnityEngine;

/// <summary>
/// Triggers <see cref="Object.DontDestroyOnLoad"/> on <see cref="Awake"/>
/// </summary>
public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}

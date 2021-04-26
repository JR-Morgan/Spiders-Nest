using UnityEngine;

/// <summary>
/// Script ensures only a single instance exists in the scene
/// </summary>
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private void Awake()
    {
        //This ensures there is never more than one music manager at once
        if (instance == null) instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }
}

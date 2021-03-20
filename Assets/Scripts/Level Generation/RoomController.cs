using UnityEngine;

[SelectionBase]
public class RoomController : MonoBehaviour
{
    private const int ENUM_OFFSET = 1;
    [SerializeField]
    private RoomType roomType;

    [SerializeField]
    private GameObject[] types;

    private void OnValidate()
    {
        foreach (GameObject t in types) t.SetActive(false);
        int i = (int)roomType - ENUM_OFFSET;
        if (i >= 0 && i < types.Length)
        {
            types[i].SetActive(true);
        }
        
    }

}

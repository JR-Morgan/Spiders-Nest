using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class WallController : MonoBehaviour
{
    [SerializeField]
    private WallType wallType = WallType.Solid;
    [SerializeField]
    private GameObject[] types;

    [SerializeField]
    private List<RoomController> _rooms;
    public List<RoomController> Rooms { get => _rooms; set => _rooms = value; }

    public int X { get; set; }
    public int Y { get; set; }


    private void OnValidate()
    {
        foreach (GameObject t in types) t.SetActive(false);
        types[(int)wallType].SetActive(true);
    }


}

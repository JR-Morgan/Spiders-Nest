using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
//[ExecuteInEditMode]
public class WallController : MonoBehaviour
{
    [SerializeField]
    public WallType wallType = WallType.Solid;
    [SerializeField]
    private GameObject[] types;

    [SerializeField]
    private List<RoomController> _rooms;
    public List<RoomController> Rooms { get => _rooms; set => _rooms = value; }

    [SerializeField]
    private int _x;
    [SerializeField]
    private int _y;

    public int X { get => _x; set => _x = value; }
    public int Y { get => _y; set => _y = value; }
    public int Group { get; private set; }

    public void Awake()
    {
        //Some times (I don't know why) _x and _y get reset to default (0) even though they are serialised (ExecuteInEditModeAttribute ?) so this uses the objects name
        //To set the X and Y back again (not ideal but works for now)
        if (_x == 0 && _y == 0)
        {
            string[] split = this.name.Split(',');
              X = int.Parse(split[0]);
              Y = int.Parse(split[1]);
        }
        string[] group = this.transform.parent.name.Split(' ');
        this.Group = int.Parse(group[2]);
    }

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        if (types[(int)wallType].activeInHierarchy != true)
//        {
//            foreach (GameObject t in types) t.SetActive(false);
//            types[(int)wallType].SetActive(true);
//        }
//    }
//#endif

}

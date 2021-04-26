using UnityEngine;

/// <summary>
/// <see cref="MonoBehaviour"/> for instantiating new levels during level design
/// </summary>
[ExecuteInEditMode]
public class NewLevelGenerator : MonoBehaviour
{
    #region Geometry constants
    /// <summary>Length (in units) of walls</summary>
    public const float Wall_Length = 17f;
    /// <summary> Number of walls around a room</summary>
    public const int NUMBER_OF_WALLS = 6;
    /// <summary> Euler angle between walls (60° for <c><see cref="NUMBER_OF_WALLS"/> = 6</c>)</summary>
    public const float THETA = 360f / NUMBER_OF_WALLS;
    /// <summary>Inner radius of room polygon</summary>
    public static readonly float INNER_RADIUS = Wall_Length * (1f / Mathf.Tan(Mathf.PI / NUMBER_OF_WALLS));
    /// <summary>Outer radius of room polygon</summary>
    public static readonly float OUTER_RADIUS = Wall_Length * (1f / Mathf.Sin(Mathf.PI / NUMBER_OF_WALLS));
    #endregion

    #region Prefab References
    [Header("Prefab References")]
    [SerializeField]
    private GameObject wallPrefab;
    [SerializeField]
    private GameObject roomPrefab;
    #endregion

    #region Generation Properties
    [Header("Generation Properties")]
    [SerializeField]
    [Tooltip("The number of rooms between the centre and outside, inclusive of the centre room")]
    private int radius;

    [Header("Wall Generation Properties")]
    [SerializeField]
    [Tooltip("The number of rotation groups")]
    private int n = NUMBER_OF_WALLS / 2;
    #endregion

    #region Fields
    [Header("Read Only")]
    [SerializeField]
    private GameObject wallParent, roomParent;
    #endregion


    private void InstantiateParent(ref GameObject parent, string name)
    {
        DestroyImmediate(parent);

        parent = new GameObject(name);
        parent.transform.parent = this.transform;
    }


    /// <summary>
    /// (Re)Instantiates Rooms and Wall objects
    /// </summary>
    public void InstantiateAll()
    {
        InstantiateParent(ref roomParent, "Rooms");
        InstantiateParent(ref wallParent, "Walls");

        GameObject[][,] wallGroups = WallGenerator.InstantiateWalls(wallParent.transform, wallPrefab, n, radius);

        GameObject[,] rooms = RoomGenerator.InstantiateRooms(roomParent.transform, roomPrefab, radius);


        for(int i = 0; i < wallGroups.Length; i++)
        {
            float angle = WallGenerator.CalculateRotation(i, n) * Mathf.Deg2Rad;
            for (int roomY = 0; roomY < rooms.GetLength(1); roomY++)
            {
                int offset = -Mathf.Max(radius - roomY - 1, 0);

                for (int roomX = Mathf.Max(radius - roomY - 1, 0); roomX < rooms.GetLength(0); roomX++)
                {

                    GameObject go = rooms[roomX, roomY];
                    if (go != null)
                    {
                        RoomController room = go.GetComponent<RoomController>();

                        if(i == 0)
                        {
                            WallController w1 = wallGroups[i][roomX + offset, roomY].GetComponent<WallController>();
                            WallController w2 = wallGroups[i][roomX + offset + 1, roomY].GetComponent<WallController>();
                            AddWalls(room, w1, w2);
                        }
                        else
                        {
                            Vector2 roomPosition = new Vector2(room.transform.position.x, room.transform.position.z); 
                            Vector2 rotatedPosition = roomPosition.RotateAround(0f, 0f, angle);


                            if (TryGetGameObjectAtPosition(rooms, new Vector3(rotatedPosition.x, 0f, rotatedPosition.y), out GameObject targetRoom))
                            {
                                RoomController target = targetRoom.GetComponent<RoomController>();

                                WallController w1 = target.Walls[0], w2 = target.Walls[1];

                                w1 = wallGroups[i][w1.X, w1.Y].GetComponent<WallController>();
                                w2 = wallGroups[i][w2.X, w2.Y].GetComponent<WallController>();
      
                                AddWalls(room, w1, w2);
                            }
                            else
                            {
                                Debug.Log($"Couldn't find room at position {rotatedPosition}");
                            }

                        }

                    }

                }
            }
        }

        static void AddWalls(RoomController room, params WallController[] walls)
        {
            foreach(WallController wall in walls)
            {
                room.Walls.Add(wall);
                wall.Rooms.Add(room);
            }
        }

    }


    private static bool TryGetGameObjectAtPosition(GameObject[,] rooms, Vector3 position, out GameObject room)
    {
        for(int x = 0; x < rooms.GetLength(0); x++)
        {
            for (int y = 0; y < rooms.GetLength(1); y++)
            {
                GameObject r = rooms[x, y];
                if(r != null)
                {
                    if (Vector3.Distance(r.transform.position, position) < 0.001f) // This allows for floating point rounding errors
                    {
                        room = r;
                        return true;
                    }
                }
            }
        }
        room = null;
        return false;
    }
}

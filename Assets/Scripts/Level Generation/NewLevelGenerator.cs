using UnityEngine;

[ExecuteInEditMode]
public class NewLevelGenerator : MonoBehaviour
{
    #region Geometry constants
    public const float WALLSIZE = 17f;
    public const int NUMBER_OF_WALLS = 6;
    public const float THETA = 360f / NUMBER_OF_WALLS;
    public static readonly float INNER_RADIUS = WALLSIZE * (1f / Mathf.Tan(Mathf.PI / 6f));
    public static readonly float OUTER_RADIUS = WALLSIZE * (1f / Mathf.Sin(Mathf.PI / 6f));
    #endregion

    #region Prefab References
    [Header("Prefab References")]
    [SerializeField]
    private GameObject WallPrefab;
    [SerializeField]
    private GameObject RoomPrefab;
    #endregion

    #region Generation Properties
    [Header("Generation Properties")]
    [Tooltip("The number of rooms between the centre and outside, inclusive of the centre room")]
    [SerializeField]
    private int radius;
    [SerializeField]
    [Header("Wall Generation Properties")]
    [Tooltip("The number of rotation groups")]
    private int n = 3;
    #endregion

    #region Fields
    private GameObject wallParent, roomParent;
    #endregion


    private void InstantiateParent(ref GameObject parent, string name)
    {
        DestroyImmediate(parent);

        parent = new GameObject(name);
        parent.transform.parent = this.transform;
    }

    /// <summary>
    /// (Re)Instantiates Wall Objects
    /// </summary>
    public void InstantiateWalls()
    {
        InstantiateParent(ref wallParent, "Walls");

        WallGenerator.InstantiateWalls(wallParent.transform, WallPrefab, n, radius);
    }
    

    /// <summary>
    /// (Re)Instantiates Room Objects
    /// </summary>
    public void InstantiateRooms()
    {
        InstantiateParent(ref roomParent, "Rooms");

        RoomGenerator.InstantiateRooms(roomParent.transform, RoomPrefab, radius);

    }


}

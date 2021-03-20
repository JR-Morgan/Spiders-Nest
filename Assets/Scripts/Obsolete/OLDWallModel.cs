using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
[Obsolete]
public class OLDWallModel : MonoBehaviour
{
    public const float WALLSIZE = 17f;
    public const int NUMBER_OF_WALLS = 6;
    public const float THETA = 360f / NUMBER_OF_WALLS;
    public static readonly float INNER_RADIUS = WALLSIZE * (1f / Mathf.Tan(Mathf.PI / 6f));
    public static readonly float OUTER_RADIUS = WALLSIZE * (1f / Mathf.Sin(Mathf.PI / 6f));

    [SerializeField]
    private WallType[] wallTypes = new WallType[NUMBER_OF_WALLS];

    private readonly GameObject[] walls = new GameObject[NUMBER_OF_WALLS];
    [SerializeField]
    private GameObject wallGroup = null;

    [SerializeField]
    private Vector2Int _index;
    public Vector2Int Index { get => _index; set => _index = value; }

    void Awake()
    {
        wallGroup = Instantiate(new GameObject("WallGroup"), transform);
        UpdateWalls();
    }

    void OnValidate()
    {
        if(wallGroup != null) UpdateWalls();
    }
    
    public void SetWall(int index, WallType type)
    {
        index = Mod(index);

        wallTypes[index] = type;
    }

    public bool SetWallTypes(WallType[] wallTypes)
    {
        if (ValidUpdate(wallTypes))
        {
            this.wallTypes = wallTypes;
            return UpdateWalls();
        }
        return false;
        
    }


    private static int Mod(int number) => ((number %= NUMBER_OF_WALLS) < 0) ? number + NUMBER_OF_WALLS : number;

    public void Rotate(int steps = 1)
    {
        steps = Mod(steps);

        for (int step = 0; step < steps; step++)
        {
            WallType a = wallTypes[0];
            for (int i = 0; i < wallTypes.Length - 1; i++)
            {
                wallTypes[i] = wallTypes[i + 1];
            }
            wallTypes[wallTypes.Length - 1] = a;
        }
        UpdateWalls();
        UpdateNeighbours();
    }

    private static readonly Vector2Int[] NeighbourMap = new Vector2Int[NUMBER_OF_WALLS] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, 0) };

    private void UpdateNeighbours()
    {
        OldLevelGenerator levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<OldLevelGenerator>();

        for(int wallIndex = 0; wallIndex < NUMBER_OF_WALLS; wallIndex++)
        {
            Vector2Int neighbourPosition = Index + NeighbourMap[wallIndex];
            if (levelManager.InBounds(neighbourPosition))
            {
                GameObject neighbourObject = levelManager[neighbourPosition.x, neighbourPosition.y];
                OLDWallModel neighbour = neighbourObject.GetComponent<OLDWallModel>();

                int reflectedWall = (wallIndex + NUMBER_OF_WALLS / 2) % NUMBER_OF_WALLS;
                neighbour.SetWall(reflectedWall, wallTypes[wallIndex]);

            }
        }

    }

    private bool ValidUpdate(in WallType[] wallTypes) =>  wallTypes.Length == NUMBER_OF_WALLS;

    private bool UpdateWalls()
    {
        if (!ValidUpdate(wallTypes)) return false;

        Vector3 distance = new Vector3(0f, 0f, -(INNER_RADIUS / 2f));

        wallGroup.DestroyChildren();

        for (int i = 0; i < walls.Length; i++)
        {
            walls[i] = Instantiate(WallFactoryManager.Instance.GetWall(wallTypes[i]), wallGroup.transform);
            walls[i].transform.position += distance;
            walls[i].transform.RotateAround(wallGroup.transform.position, Vector3.up, 90f + THETA + (THETA * i));
        }
        return true;
    }

    private void OnDestroy()
    {
        //TODO not sure if this is needed but seems to fix a problem where walls are not cleanedup
        wallGroup.DestroyChildren();
    }


    private void Update()
    {
        if (name == "Room {1,1}" && Input.GetKeyDown(KeyCode.A))
        {
            //this.Rotate();
        }
    }


}

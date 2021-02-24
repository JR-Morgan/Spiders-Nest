using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[ExecuteInEditMode]
[Obsolete]
public class OldLevelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject Room;

    [SerializeField]
    private int radius;

    private GameObject[,] boardArray;

    void Start()
    {
        GenerateLevel();
    }


    public GameObject this[int x, int y]
    {
        get => boardArray[x, y];
    }

    private void OnValidate()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        transform.DestroyChildren();

        if (radius <= 0) return;

        int size = Size(radius);
        this.boardArray = new GameObject[size, size];

        // int radius = boardArray.length;
        for (int x = 0; x < size; x++)
        for (int y = 0; y < size; y++)
        {
            if (InBounds(radius,x,y))
            {
                this.boardArray[x, y] = SetupRoom(x, y);
            }
        }
    }

    public static bool InBounds(int radius, int x, int y) => (x + y) > (radius - 2) && (x + y) <= Size(radius) + (radius - 2);
    public bool InBounds(Vector2Int index) => InBounds(radius, index.x, index.y);
    public bool InBounds(int x, int y) => InBounds(radius, x, y);

    private GameObject SetupRoom(int x, int y)
    {
        int size = Size(radius);
        GameObject room = Instantiate(Room, new Vector3(x * WallModel.INNER_RADIUS + (WallModel.INNER_RADIUS * y / 2f), 0f, ((WallModel.OUTER_RADIUS + WallModel.WALLSIZE) / 2f)*y), Quaternion.identity, transform);
        room.name = $"Room {{{x},{y}}}";

        WallModel walls = room.GetComponent<WallModel>();
        walls.Index = new Vector2Int(x, y);

        WallType[] types = new WallType[WallModel.NUMBER_OF_WALLS];
        for(int i=0; i< types.Length; i++)
        {
            types[i] = WallType.Solid;
        }

        //walls.SetWallTypes(types);

        //if (x <= 0 || y <= 0 || x >= size || y >= size)
        //{
            
        //} 
        //else
        //{
        //    int[] pool = new int[Enum.GetNames(typeof(WallModel)).Length];
        //}
        

        


        return room;
    }

    private static int Size(int radius) => (radius * 2) - 1;
}

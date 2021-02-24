using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class NewLevelGenerator : MonoBehaviour
{
    private GameObject[][,] walls;

    [SerializeField]
    private GameObject WallPrefab;

    [SerializeField]
    private int radius;

    [SerializeField]
    private int n = 3;
    [SerializeField]
    private bool applyGroupRotation = true;

    public void InstantiateWalls()
    {
        transform.DestroyChildren();
        walls = new GameObject[n][,];

        for (int i = 0; i < n; i++)
        {
            GameObject parent = new GameObject("Wall Group " + i);
            parent.transform.parent = transform;
            

            walls[i] = InstantiateWallSet(parent.transform);
            parent.transform.localRotation = Quaternion.Euler(0f, ((float)i / n) * 360f, 0f);
        }

    }


    private GameObject[,] InstantiateWallSet(Transform parent)
    {
        GameObject[,] gameObjects;
        int size = Size(radius);
        gameObjects = new GameObject[size, size];

        for (int y = 0; y < size - 1; y++)
        {
            int t = Mathf.Abs(y - (radius - 1));
            for (int x = 0; x < size - t; x++)
            {
                //if (InBounds(radius, x, y))
                {
                    GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(WallPrefab, parent);
                    wall.name = $"{x}, {y}";
                    wall.transform.position = applyGroupRotation? AbsolutePosition(radius, x, y) : RelativePosition(radius, x, y);
                    //gameObjects[x, y] = wall;
                }
            }
        }
            

        return gameObjects;
    }



    private static Vector3 AbsolutePosition(int radius, float x, float y) => RelativePosition(radius, x, y) - new Vector3((radius-1) * WallModel.INNER_RADIUS + WallModel.INNER_RADIUS / 2f  ,0f, ((radius - 1) * (WallModel.OUTER_RADIUS - ((WallModel.OUTER_RADIUS/2)-(WallModel.WALLSIZE/2)))) + WallModel.OUTER_RADIUS / 2);
    //private static Vector3 RelativePosition(float x, float y) => new Vector3(x * WallModel.INNER_RADIUS + (WallModel.INNER_RADIUS * y / 2f), 0f, ((WallModel.OUTER_RADIUS + WallModel.WALLSIZE) / 2f) * y);

    private static Vector3 RelativePosition(int radius, float x, float y) => new Vector3((x * WallModel.INNER_RADIUS) + (Mathf.Abs(y - (radius - 1)) * (WallModel.INNER_RADIUS / 2f)), 0f, (y+1) * (WallModel.OUTER_RADIUS / 2f) + y * (WallModel.WALLSIZE / 2f));
    private static bool InBounds(int radius, int x, int y) => (x + y) > (radius - 2) && (x + y) <= Size(radius) + (radius - 2);

    private static int Size(int radius) => radius * 2;


}

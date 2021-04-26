using UnityEditor;
using UnityEngine;

/// <summary>
/// Class for generating hexagonal wall groups
/// </summary>
public static class WallGenerator
{

    /// <summary>
    /// (Re)Instantiates Wall Objects
    /// </summary>
    public static GameObject[][,] InstantiateWalls(Transform parent, GameObject wallPrefab, int n, int radius)
    {

        GameObject[][,] walls = new GameObject[n][,];

        for (int i = 0; i < n; i++)
        {
            GameObject group = new GameObject("Wall Group " + i);
            group.transform.parent = parent.transform;

            walls[i] = InstantiateWallSet(group.transform, radius, wallPrefab);
            group.transform.localRotation = Quaternion.Euler(0f, CalculateRotation(i,n), 0f);
        }

        return walls;

    }

    /// <summary>
    /// Calculates the rotation
    /// </summary>
    /// <param name="i">the index of the wall group (normally between 0-2)</param>
    /// <param name="n">the number of wall groups (normally 3)</param>
    /// <returns>rotation angle in degrees</returns>
    public static float CalculateRotation(int i, int n) => ((float)i / n) * 360f;

    private static GameObject[,] InstantiateWallSet(Transform parent, int radius, GameObject wallPrefab)
    {
#if UNITY_EDITOR
        GameObject[,] gameObjects;
        int size = Size(radius);
        gameObjects = new GameObject[size, size];

        for (int y = 0; y < size - 1; y++)
        {
            int t = Mathf.Abs(y - (radius - 1));
            for (int x = 0; x < size - t; x++)
            {
                GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent);
                wall.name = $"{x}, {y}";
                wall.transform.position = AbsolutePosition(radius, x, y);

                if(wall.TryGetComponent(out WallController controller))
                {
                    controller.X = x;
                    controller.Y = y;
                }


                gameObjects[x, y] = wall;
            }
        }


        return gameObjects;
#else
        return null;
#endif
    }

    private static int Size(int radius) => radius * 2;
    private static Vector3 RelativePosition(int radius, int x, int y) => new Vector3((x * NewLevelGenerator.INNER_RADIUS) + (Mathf.Abs(y - (radius - 1)) * (NewLevelGenerator.INNER_RADIUS / 2f)), 0f, (y + 1) * (NewLevelGenerator.OUTER_RADIUS / 2f) + y * (NewLevelGenerator.Wall_Length / 2f));

    private static Vector3 AbsoluteTransform(int radius, Vector3 localPosition) => localPosition - new Vector3((radius - 1) * NewLevelGenerator.INNER_RADIUS + NewLevelGenerator.INNER_RADIUS / 2f, 0f, ((radius - 1) * (NewLevelGenerator.OUTER_RADIUS - ((NewLevelGenerator.OUTER_RADIUS / 2) - (NewLevelGenerator.Wall_Length / 2)))) + NewLevelGenerator.OUTER_RADIUS / 2);
    private static Vector3 AbsolutePosition(int radius, int x, int y) => AbsoluteTransform(radius, RelativePosition(radius, x, y));
}

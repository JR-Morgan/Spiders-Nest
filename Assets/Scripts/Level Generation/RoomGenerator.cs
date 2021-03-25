using UnityEditor;
using UnityEngine;

public static class RoomGenerator
{
    public static GameObject[,] InstantiateRooms(Transform parent, GameObject roomPrefab, int radius)
    {
#if UNITY_EDITOR

        GameObject[,] boardArray;

        if (radius <= 0) return null;

        int size = Size(radius);
        boardArray = new GameObject[size, size];

        // int radius = boardArray.length;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            if (InBounds(radius, x, y))
            {
                GameObject room = (GameObject)PrefabUtility.InstantiatePrefab(roomPrefab,  parent.transform) ;
                room.transform.position = AbsolutePosition(radius, x, y);
                room.name = $"Room {{{x},{y}}}";
                boardArray[x, y] = room;
            }
        }

        return boardArray;
#else
        return null;
#endif
    }

    private static int Size(int radius) => (radius * 2) - 1;
    private static bool InBounds(int radius, int x, int y) => (x + y) > (radius - 2) && (x + y) <= Size(radius) + (radius - 2);
    private static Vector3 RelativePosition(int x, int y) => new Vector3(x * NewLevelGenerator.INNER_RADIUS + (NewLevelGenerator.INNER_RADIUS * y / 2f), 0f, ((NewLevelGenerator.OUTER_RADIUS + NewLevelGenerator.WALLSIZE) / 2f) * y);
    
    private static Vector3 AbsoluteTransform(int radius, Vector3 localPosition) => localPosition - new Vector3((radius - 1) * 1.5f * NewLevelGenerator.INNER_RADIUS, 0f, (radius - 1) * ((NewLevelGenerator.OUTER_RADIUS/2f) + (NewLevelGenerator.WALLSIZE/2f)));
    private static Vector3 AbsolutePosition(int radius, int x, int y) => AbsoluteTransform(radius, RelativePosition(x, y));
}

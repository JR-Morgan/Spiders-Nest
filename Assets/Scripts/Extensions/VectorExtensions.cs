using UnityEngine;

public static class Extension
{
    public static Vector2 RotateAround(this Vector2 pos, Vector2 origin, float radians) => RotateAround(pos, origin.x, origin.y, radians);
    public static Vector2 RotateAround(this Vector2 pos, float originX, float originY, float radians)
    {
        float sine = Mathf.Sin(radians);
        float cosine = Mathf.Cos(radians);

        float x = pos.x;
        float y = pos.y;

        x -= originX;
        y -= originY;

        float newX = x * cosine - y * sine;
        float newY = x * sine + y * cosine;

        pos.x = newX + originX;
        pos.y = newY + originY;
        return pos;
    }
}

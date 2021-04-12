using UnityEngine;


public static class GameObjectExtension
{
    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (child != null) Object.DestroyImmediate(child.gameObject);
            };
    #else
        GameObject.Destroy(child);
    #endif
        }
    }
    public static void DestroyChildren(this GameObject gameObject) => DestroyChildren(gameObject.transform);

    public static int ToLayerNumber(this LayerMask mask) => Mathf.RoundToInt(Mathf.Log(mask.value, 2));

    #region Try Get Component
    public static bool TryGetComponentInParents<T>(this GameObject gameObject, out T component, bool includeInactive = false) where T : Component
    {
        component = gameObject.GetComponentInParent<T>(includeInactive);
        return component != null;
    }

    public static bool TryGetComponentsInParents<T>(this GameObject gameObject, out T[] components, bool includeInactive = false) where T : Component
    {
        components = gameObject.GetComponentsInParent<T>(includeInactive);
        return components != null;
    }

    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component, bool includeInactive = false) where T : Component
    {
        component = gameObject.GetComponentInChildren<T>(includeInactive);
        return component != null;
    }

    public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, out T[] components, bool includeInactive = false) where T : Component
    {
        components = gameObject.GetComponentsInChildren<T>(includeInactive);
        return components != null;
    }
    #endregion
}

using UnityEngine;


internal static class GameObjectExtension
{
    #region Destroy
    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (child != null) UnityEngine.Object.DestroyImmediate(child.gameObject);
            };
#else
        GameObject.Destroy(child.gameObject);
#endif
        }
    }
    public static void DestroyChildren(this GameObject gameObject) => DestroyChildren(gameObject.transform);
    #endregion

    #region Layers
    public static int ToLayerNumber(this LayerMask mask) => Mathf.RoundToInt(Mathf.Log(mask.value, 2));
    #endregion

    #region Try Get Component
    private const bool DEFAULT_INCLUDE_INTERACTIVE = false;
    public static bool TryGetComponentInParents<T>(this Component source, out T component, bool includeInactive = DEFAULT_INCLUDE_INTERACTIVE) where T : Component => TryGetComponentInParents(source.gameObject, out component, includeInactive);
    public static bool TryGetComponentInParents<T>(this GameObject gameObject, out T component, bool includeInactive = DEFAULT_INCLUDE_INTERACTIVE) where T : Component
    {
        component = gameObject.GetComponentInParent<T>(includeInactive);
        return component != null;
    }

    public static bool TryGetComponentsInParents<T>(this Component source, out T[] components, bool includeInactive = DEFAULT_INCLUDE_INTERACTIVE) where T : Component => TryGetComponentsInParents(source.gameObject, out components, includeInactive);
    public static bool TryGetComponentsInParents<T>(this GameObject gameObject, out T[] components, bool includeInactive = DEFAULT_INCLUDE_INTERACTIVE) where T : Component
    {
        components = gameObject.GetComponentsInParent<T>(includeInactive);
        return components != null;
    }

    public static bool TryGetComponentInChildren<T>(this Component source, out T component, bool includeInactive = DEFAULT_INCLUDE_INTERACTIVE) where T : Component => TryGetComponentInChildren(source.gameObject, out component, includeInactive);
    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component, bool includeInactive = DEFAULT_INCLUDE_INTERACTIVE) where T : Component
    {
        component = gameObject.GetComponentInChildren<T>(includeInactive);
        return component != null;
    }

    public static bool TryGetComponentsInChildren<T>(this Component source, out T[] components, bool includeInactive = DEFAULT_INCLUDE_INTERACTIVE) where T : Component => TryGetComponentsInChildren(source.gameObject, out components, includeInactive);
    public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, out T[] components, bool includeInactive = DEFAULT_INCLUDE_INTERACTIVE) where T : Component
    {
        components = gameObject.GetComponentsInChildren<T>(includeInactive);
        return components != null;
    }
    #endregion

    #region Require Get
    /// <summary>
    /// Finds the first occurrence of <typeparamref name="T"/> in <paramref name="source"/> or children of.
    /// Prints a warning to console if no <typeparamref name="T"/> were found.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Component"/></typeparam>
    /// <param name="source"></param>
    /// <param name="component"></param>
    /// <param name="includeInactive"></param>
    /// <returns><c>true</c> if <paramref name="component"/> was assigned</returns>
    public static bool RequireComponentInChildren<T>(this Component source, out T component, bool includeInactive = false) where T : Component
    {
        bool found = TryGetComponentInChildren<T>(source, out component, includeInactive);
        if (!found) Debug.LogWarning($"{source.GetType()} could not find any {typeof(T)} {nameof(component)} in self or children!", source);
        return found;
    }

    public static bool RequireComponentsInChildren<T>(this Component source, out T[] components, bool includeInactive = false) where T : Component
    {
        bool found = TryGetComponentsInChildren<T>(source, out components, includeInactive);
        if (!found) Debug.LogWarning($"{source.GetType()} could not find any {typeof(T)} {nameof(components)} in self or children!", source); ;
        return found;
    }

    #endregion
}

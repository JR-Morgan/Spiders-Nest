using UnityEngine;


public static class GameObjectExtension
{
    public static void DestroyChildren(this Transform transform)
    {
        Debug.Log("DESTROY CHILDREN");
        foreach (Transform child in transform)
        {
#if UNITY_EDITOR
            Debug.Log("We are in edit mode");
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (child != null) GameObject.DestroyImmediate(child.gameObject);
            };
#else
    Debug.Log("We are not edit mode")
        GameObject.Destroy(child);
#endif
        }
    }
    public static void DestroyChildren(this GameObject gameObject) => DestroyChildren(gameObject.transform);

}

using UnityEngine;

[CreateAssetMenu(fileName = "New Action Type", menuName = nameof(ScriptableObject) + "/" + nameof(ActionType), order = 1)]
public class ActionType : ScriptableObject
{
    public string actionName;
    public float cost;
    public bool freeIfBankrupt;
    public bool placeable;
    public GameObject prefab;
    public Texture2D icon;
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Type", menuName = nameof(ScriptableObject) + "/" + nameof(EnemyTypeData), order = 1)]
public class EnemyTypeData : ScriptableObject
{
    public EnemyType typeID;
    public GameObject prefab;
    [Header("Evolution Settings")]
    [Range(0f, 1f)]
    public float proababiltyToEvolve;
    public float timeUntilEvolve;
    public float timeToEvolve;
    [Header("Health")]
    public float maxHealth;
    public float movementSpeed;
}
public enum EnemyType
{
    Small,
    Medium,
    Big,
    Deamon,
}
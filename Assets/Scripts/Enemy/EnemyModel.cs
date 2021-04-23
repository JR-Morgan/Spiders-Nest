using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Model", menuName = nameof(ScriptableObject) + "/" + nameof(EnemyModel), order = 1)]
public class EnemyModel : ScriptableObject
{
    public EnemyType typeID;
    public GameObject prefab;
    public float damage;
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
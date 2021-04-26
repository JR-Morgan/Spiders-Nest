using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Encapsulates a players Money and transactions
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private float money;

    public float Money { get => money;
        set {
            money = Mathf.Max(value, 0f);
            OnValueChange.Invoke(Money);
        }
    }

    public UnityEvent<float> OnValueChange;

    public bool CheckValidSubtract(float amount) => CheckValidAdd(-amount);
    public bool CheckValidAdd(float amount) => Money + amount >= 0f;

    public bool CanAfford(ActionType action) => CheckValidSubtract(CostOfAction(action));

    public float CostOfAction(ActionType action) => Mathf.RoundToInt(action.cost + Money * action.proportionalCost);

    public bool TrySubtract(float amount) => TryAdd(-amount);
    public bool TryAdd(float amount)
    {
        if (!CheckValidAdd(amount)) return false;

        AddUnchecked(amount);
        return true;
    }

    public void SubtractUnchecked(float amount) => AddUnchecked(-amount);
    public void AddUnchecked(float amount)
    {
        money += amount;
        money = Mathf.Max(money, 0f);
        OnValueChange.Invoke(money);
    }

}

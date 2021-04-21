using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private float money;

    public float Money => money;

    public UnityEvent<float> OnValueChange;

    public bool CheckValidSubtract(float amount) => CheckValidAdd(-amount);
    public bool CheckValidAdd(float amount) => Money + amount > 0f;

    public bool CanAfford(ActionType action) => CheckValidSubtract(action.cost) || action.freeIfBankrupt;

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

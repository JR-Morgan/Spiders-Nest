using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private float money;

    public float Money => money;

    public UnityEvent<float> OnValueChange;

    public bool CheckValidAdd(float amount) => amount < 0f || amount + money < 0f;
    public bool TryAdd(float amount)
    {
        if (!CheckValidAdd(amount)) return false;

        money += amount;
        OnValueChange.Invoke(money);
        return true;
    }


    public bool CheckValidSubtract(float amount) => CheckValidAdd(-amount);
    public bool TrySubtract(float amount) => TryAdd(-amount);

}

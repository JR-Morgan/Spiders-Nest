using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HudController : MonoBehaviour
{
    private const string CONTAINER_ELEMENT_NAME = "HudContainer";
    private void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement container = document.rootVisualElement.Q(CONTAINER_ELEMENT_NAME);

        Debug.Assert(container != null, $"{typeof(UIDocument)} did not contain an element with name {CONTAINER_ELEMENT_NAME}", this);

        { // Kills
            HudTextField kills = new HudTextField("Enemies Killed:");
            EnemyManager.Instance.OnEnemyDeath.AddListener((e,n) => kills.Value = n.ToString());
            container.Add(kills);
        }

        { // Money
            PlayerInventory inventory = PlayerManager.Instance.Local.Inventory;

            HudTextField money = new HudTextField("Money:", FormatMoney(inventory.Money));

            inventory.OnValueChange.AddListener(newValue => money.Value = FormatMoney(newValue));
            container.Add(money);
        }

    }

    private static string FormatMoney(float money) => $"{money:0.##}";
}

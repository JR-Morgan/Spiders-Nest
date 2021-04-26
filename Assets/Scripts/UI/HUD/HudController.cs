using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// UI Controller for setting up HUD <see cref="HudTextField"/>s
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class HudController : MonoBehaviour
{
    private const string CONTAINER_ELEMENT_NAME = "HudContainer";
    private void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement container = document.rootVisualElement.Q(CONTAINER_ELEMENT_NAME);

        Debug.Assert(container != null, $"{typeof(UIDocument)} did not contain an element with name {CONTAINER_ELEMENT_NAME}", this);

        { // Health
            HudTextField health = new HudTextField("Health:", RoundValue(PlayerManager.Instance.Local.Health));
            PlayerManager.Instance.Local.OnHealthChange.AddListener((h) => health.Value = RoundValue(h));
            container.Add(health);
        }

        { // Kills
            HudTextField kills = new HudTextField("Enemies Killed:", "0");
            EnemyManager.Instance.OnEnemyDeath.AddListener((e,n) => kills.Value = n.ToString());
            kills.Value = EnemyManager.Instance.NumberOfKills.ToString();
            container.Add(kills);
        }

        { // Money
            PlayerInventory inventory = PlayerManager.Instance.Local.Inventory;

            HudTextField money = new HudTextField("Money:", RoundValue(inventory.Money));

            inventory.OnValueChange.AddListener(newValue => money.Value = RoundValue(newValue));
            container.Add(money);
        }

    }

    private static string RoundValue(float value) => $"{value:0.##}";
}

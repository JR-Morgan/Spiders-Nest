using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]
public class ChestInteractable : MonoBehaviour, IInteractable
{
    private const KeyCode DEFAULT_KEY = KeyCode.E;

    #region Serialised Fields
    [Tooltip("The players cost of opening chest")]
    [SerializeField]
    private float _cost;
    [SerializeField]
    private KeyCode keycode = DEFAULT_KEY;
    #endregion

    private UIDocument document;
    private InteractableDisplay display;


    private void Awake()
    {
        display = new InteractableDisplay();

        display.SetMessage(
            cost: _cost.ToString(),
            prompt: keycode.ToString(),
            action: "Complete Level"
            );
    }

    private void Start()
    {
        document = FindObjectOfType<UIDocument>();
    }

    public void OnHoverEnd(Interactor interactor = null)
    {
        display.RemoveFromHierarchy();
    }

    public void OnHoverStart(Interactor interactor)
    {
        document.rootVisualElement.Add(display);
        interactor.AddKeyListener(keycode, () =>  BuyEventHandler(interactor));
    }

    private void BuyEventHandler(Interactor interactor)
    {
        if (interactor.TryGetComponent(out PlayerInventory inventory)
            && inventory.TrySubtract(_cost))
        {

            OnOpen.Invoke();

            OnHoverEnd();

            Destroy(this.gameObject);
        }
    }

    public UnityEvent OnOpen;

}

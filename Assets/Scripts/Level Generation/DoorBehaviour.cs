using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]
public class DoorBehaviour : MonoBehaviour, IInteractable
{
    private const KeyCode DEFAULT_KEY = KeyCode.E;
    #region Serialised Fields
    [Tooltip("The players cost of opening the door")]
    [SerializeField]
    private float _cost;
    [SerializeField]
    private KeyCode keycode = DEFAULT_KEY;
    #endregion

    private UIDocument document;
    private InteractableDisplay display;
    private WallController wallParent;
    public WallController WallParent
    {
        get
        {
            if (wallParent == null) wallParent = GetComponentInParent<WallController>();
            return wallParent;

        }
    }




    public float Cost
    {
        get => _cost;
        set
        {
            _cost = value;
            display.Cost = value.ToString();
        }
    }

    private void Awake()
    {
        this.RequireComponentInParents(out wallParent);
        display = new InteractableDisplay();
        display.SetMessage(
            cost: _cost.ToString(),
            prompt: keycode.ToString(),
            action: "Open Door"
            );
    }

    private void Start()
    {
        document = FindObjectOfType<UIDocument>();
    }



    private void OnDisable()
    {
        gameObject.layer = 0;
        if (GameObject.FindGameObjectWithTag("LevelManager").TryGetComponent(out NavMeshSurface nav))
        {
            nav.BuildNavMesh();
        }
        else Debug.LogError($"Could not find {typeof(NavMeshSurface)}", this);
    }

    private void BuyEventHandler(Interactor interactor)
    {

        if (interactor.TryGetComponent(out PlayerInventory inventory)
        && inventory.TrySubtract(_cost))
        {
            if (this.TryGetComponentInParents(out AudioSource a)) a.Play();
            LevelStateManager.Instance.ChangeDoorState(this, false);
            OnHoverEnd();
        }
    }

    public void OnHoverStart(Interactor interactor)
    {
        if(document == null) document = FindObjectOfType<UIDocument>();
        document.rootVisualElement.Add(display);
        interactor.AddKeyListener(keycode, () => BuyEventHandler(interactor));
    }

    public void OnHoverEnd(Interactor interactor = null)
    {
        display.RemoveFromHierarchy();
    }
}

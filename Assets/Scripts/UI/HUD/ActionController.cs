using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

/// <summary>
/// UI controller for <see cref="ActionType"/> selection and action
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class ActionController : Singleton<ActionController>
{
    private const int DEFAULT_I = -(short.MaxValue - 1);
    private VisualElement root;

    [SerializeField]
    private List<ActionType> actions;

    [SerializeField]
    private int activeIndex;

    public ActionType Active => actions[Mathf.Abs(activeIndex % actions.Count)];

    private Dictionary<ActionType, ActionElement> elementOfType;

    public UnityEvent<ActionType> OnActiveChange;

    private void Start()
    {
        elementOfType = new Dictionary<ActionType, ActionElement>();
        root = GetComponent<UIDocument>().rootVisualElement.Q("ActionContainer");

        //Action setup
        if(actions.Count > 0)
        {
            foreach (ActionType a in actions)
            {
                ActionElement element = new ActionElement
                {
                    Action_Name_Text = a.actionName,
                    Action_Cost = 0,
                    Icon = a.icon,
                };
                root.Add(element);
                elementOfType.Add(a, element);
            }

            activeIndex = DEFAULT_I;
            elementOfType[Active].Is_Active = true;
        }
        else
        {
            Debug.LogWarning($"{nameof(actions)} was empty", this);
        }


        //Money Setup
        PlayerInventory inventory = PlayerManager.Instance.Local.GetComponent<PlayerInventory>();
        inventory.OnValueChange.AddListener(e => CheckCanAfford(inventory));
        CheckCanAfford(inventory);
    }

    private void CheckCanAfford(PlayerInventory inventory)
    {
        foreach (ActionType action in elementOfType.Keys)
        {
            elementOfType[action].Action_Cost = inventory.CostOfAction(action);
            elementOfType[action].Can_Afford = inventory.CanAfford(action);
        }
    }

    private void SetActive(int index)
    {
        elementOfType[Active].Is_Active = false;
        activeIndex = index;
        elementOfType[Active].Is_Active = true;
        OnActiveChange.Invoke(Active);
    }

    private void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if(scrollDelta != 0)
        {
            SetActive(activeIndex + (scrollDelta > 0 ? 1 : -1));
        }

        int alpha1 = (int)KeyCode.Alpha1;
        int max = Mathf.Min(elementOfType.Count + alpha1, (int)KeyCode.Alpha9 + 1);
        for (int i = alpha1; i < max; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
            {
                SetActive(DEFAULT_I - (i - (int)KeyCode.Alpha1));
            }
        }
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ActionController : Singleton<ActionController>
{
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

        if(actions.Count > 0)
        {
            foreach (ActionType a in actions)
            {
                ActionElement element = new ActionElement
                {
                    Action_Name_Text = a.actionName,
                    Action_Cost = a.cost,
                    Icon = a.icon,
                };
                root.Add(element);
                elementOfType.Add(a, element);
            }

            activeIndex = 0;
            elementOfType[Active].Is_Active = true;
        }
        else
        {
            Debug.LogWarning($"{nameof(actions)} was empty", this);
        }

    }

    private void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if(scrollDelta != 0)
        {
            elementOfType[Active].Is_Active = false;
            activeIndex += scrollDelta > 0? 1 : -1;
            elementOfType[Active].Is_Active = true;
            OnActiveChange.Invoke(Active);
        }

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractableDisplay : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>(@"UI/Views/InteractableDisplay");

    private Label label;

    private string _prompt = "";
    public string Prompt { get => _prompt;
        set {
            _prompt = value;
            SetMessage();
        }
    }

    private string _cost = "";
    public string Cost { get => _cost;
        set {
            _cost = value;
            SetMessage();
        }
}

    private string _action = "";
    public string Action { get => _action;
        set {
            _action = value;
            SetMessage();
        }
    }

    public void SetMessage(string prompt, string cost, string action)
    {
        _prompt = prompt;
        _cost = cost;
        _action = action;
        if(label != null) SetMessage();
    }

    private void SetMessage() => label.text = Format(Prompt, Cost, Action);
    private static string Format(string prompt, string cost, string action)
    {
        return $"Press {prompt} and\n spend {cost} to\n {action}";
    }

    private void Setup()
    {
        this.style.position = Position.Absolute;
        this.style.bottom = 0f;
        this.style.left = 0f;
        this.style.right = 0f;

        string messageName = "Message";
        label = this.Q<Label>(messageName);
        if (label == null) Debug.LogWarning($"{this} could not find {typeof(Label)} with name {messageName} as a child element");
        SetMessage();
    }



    public InteractableDisplay()
    {
        Add(view.CloneTree());
        void GeometryChange(GeometryChangedEvent evt)
        {
            this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
            Setup();
        }

        this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
    }


    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<InteractableDisplay, UxmlTraits>
    { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    { }
    #endregion
}

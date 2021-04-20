using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionElement : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>(@"UI/Views/ActionView");

    #region Child Elements
    private VisualElement icon;
    private Label actionName, actionCost;
    #endregion

    #region Properties
    private bool _isActive;
    public bool Is_Active
    {
        get => _isActive;
        set
        {
            _isActive = value;
            UpdateSelected();
        }
    }
    private void UpdateSelected()
    {
        if (actionName != null) actionName.visible = Is_Active;
        if (actionCost != null) actionCost.visible = Is_Active;
    }

    private string _actionNameText;
    public string Action_Name_Text
    {
        get => _actionNameText;
        set
        {
            _actionNameText = value;
            UpdateName();
        }
    }
    private void UpdateName()
    {
        if (actionName != null) actionName.text = Action_Name_Text;
    }


    private Texture2D _icon;
    public Texture2D Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            UpdateIcon();
        }
    }
    private void UpdateIcon()
    {
        if (icon != null) icon.style.backgroundImage = Icon;
    }


    private float _actionCost;
    public float Action_Cost
    {
        get => _actionCost;
        set
        {
            _actionCost = value;
            UpdateCost();
        }
    }
    private void UpdateCost()
    {
        if (actionCost != null) actionCost.text = FormatCost(Action_Cost);
    }
    #endregion

    private static string FormatCost(float cost) => cost == 0 ? $"Cost: FREE!" : $"Cost: {cost:0.##}";

    private void Setup()
    {
        icon = this.Q<VisualElement>("Icon");
        actionName = this.Q<Label>("ActionName");
        actionCost = this.Q<Label>("ActionCost");
        UpdateName();
        UpdateCost();
        UpdateSelected();
        UpdateIcon();
    }

    public ActionElement()
    {
        Add(view.CloneTree());
        void GeometryChange(GeometryChangedEvent evt)
        {
            //this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
            Setup();
        }

        this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
    }

    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<ActionElement, UxmlTraits>
    { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlBoolAttributeDescription selected = new UxmlBoolAttributeDescription { name = nameof(Is_Active), defaultValue = false };
        private readonly UxmlStringAttributeDescription name = new UxmlStringAttributeDescription { name = nameof(Action_Name_Text), defaultValue = "Action Name" };
        private readonly UxmlFloatAttributeDescription cost = new UxmlFloatAttributeDescription { name = nameof(Action_Cost), defaultValue = 0 };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var element = ve as ActionElement;

            element.Is_Active = selected.GetValueFromBag(bag, cc);
            element.Action_Name_Text = name.GetValueFromBag(bag, cc);
            element.Action_Cost = cost.GetValueFromBag(bag, cc);
        }
    }
    #endregion
}
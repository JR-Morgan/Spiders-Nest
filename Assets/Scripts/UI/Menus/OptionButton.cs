using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// <see cref="VisualElement"/> for a main menu button
/// </summary>
public class OptionButton : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>(@"UI/Menus/OptionButton");

    #region Child Elements
    private Label label;
    private VisualElement leftIcon, rightIcon;

    #endregion


    private bool _selected;
    public bool Is_Selected { get => _selected;
        set {
            _selected = value;
            SetSelected();
        }
    }
    private string _text;
    public string Label_Text
    {
        get => _text;
        set
        {
            _text = value;
            SetText();
        }
    }

    private void SetText()
    {
        if (label != null) label.text = Label_Text;
    }


    private void SetSelected()
    {
        if(leftIcon != null) leftIcon.visible = Is_Selected;
        if (rightIcon != null) rightIcon.visible = Is_Selected;
    }

    private void Setup()
    {
        label = this.Q<Label>("Label");
        leftIcon = this.Q<VisualElement>("LeftIcon");
        rightIcon = this.Q<VisualElement>("RightIcon");

        SetText();
        SetSelected();
    }

    public OptionButton()
    {
        Add(view.CloneTree());
        void GeometryChange(GeometryChangedEvent evt)
        {
            //this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
            Setup();
            //this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
        }

        this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
    }

    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<OptionButton, UxmlTraits>
    { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription text = new UxmlStringAttributeDescription { name = nameof(Label_Text), defaultValue = "Label Text" };
        private readonly UxmlBoolAttributeDescription selected = new UxmlBoolAttributeDescription { name = nameof(Is_Selected), defaultValue = false };
        
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var OptionButton = ve as OptionButton;

            OptionButton.Label_Text = text.GetValueFromBag(bag, cc);
            OptionButton.Is_Selected = selected.GetValueFromBag(bag, cc);
        }
    }
    #endregion
}

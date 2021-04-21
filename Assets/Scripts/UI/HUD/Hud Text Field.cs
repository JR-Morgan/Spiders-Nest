using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HudTextField : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>($@"UI/Views/{typeof(HudTextField)}");

    #region Child Elements
    private Label titleElement, valueElement;
    #endregion

    #region Properties
    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            if (titleElement != null) titleElement.text = Title;
        }
    }

    private string _value;
    public string Value
    {
        get => _value;
        set
        {
            _value = value;
            if (valueElement != null) valueElement.text = Value;
        }
    }

    #endregion

    private void Setup()
    {
        titleElement = this.Q<Label>("Title");
        valueElement = this.Q<Label>("Value");

        Value = Value;
        Title = Title;
    }

    public HudTextField() : this("", "") { }
    public HudTextField(string title, string value = "")
    {
        Add(view.CloneTree());
        void GeometryChange(GeometryChangedEvent evt)
        {
            //this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
            Setup();
        }

        this.RegisterCallback<GeometryChangedEvent>(GeometryChange);

        Title = title;
        Value = value;
    }

    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<HudTextField, UxmlTraits>
    { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription title = new UxmlStringAttributeDescription { name = nameof(Title), defaultValue = nameof(Title) };
        private readonly UxmlStringAttributeDescription value = new UxmlStringAttributeDescription { name = nameof(Value), defaultValue = nameof(Value) };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var element = ve as HudTextField;

            element.Title = title.GetValueFromBag(bag, cc);
            element.Value = value.GetValueFromBag(bag, cc);
        }
    }
    #endregion
}

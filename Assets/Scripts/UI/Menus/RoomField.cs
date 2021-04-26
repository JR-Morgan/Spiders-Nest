using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A custom <see cref="VisualElement"/> that allows user to enter and submit a Photon network room ID.
/// </summary>
public class RoomField : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>(@"UI/Menus/RoomField");

    #region Child Elements
    private TextField textField;
    private Button submitButton;
    private Label errorLabel;

    #endregion

    #region Events
    public event Action OnSubmit;
    #endregion

    #region Properties
    private string _labelText;
    public string Label_Text
    {
        get => _labelText;
        set
        {
            _labelText = value;
            if (textField != null) textField.label = Label_Text;
        }
    }

    private string _value;
    public string Value
    {
        get => _value;
        set
        {
            _value = value;
            if (textField != null) textField.value = _value;
        }
    }

    private string _buttonText;
    public string Button_Text
    {
        get => _buttonText;
        set
        {
            _buttonText = value;
            if (submitButton != null) submitButton.text = _buttonText;
        }
    }

    private string _errorText;
    public string Error_Text
    {
        get => _errorText;
        set
        {
            _errorText = value;
            if (errorLabel != null) errorLabel.text = _errorText;
        }
    }

    #endregion

    private void Setup()
    {
        textField = this.Q<TextField>("TextField");
        submitButton = this.Q<Button>("SubmitButton");
        errorLabel = this.Q<Label>("ErrorLabel");

        submitButton.RegisterCallback<ClickEvent>(e => OnSubmit?.Invoke());
        textField.RegisterCallback<ChangeEvent<string>>(e => Value = textField.value);

        Label_Text = Label_Text;
        Value = Value;
        Button_Text = Button_Text;
        Error_Text = Error_Text;
    }

    public RoomField()
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

    public RoomField(string labelText, string value, string buttonText, string errorText = "") : this()
    {
        Label_Text = labelText;
        Value = value;
        Button_Text = buttonText;
        Error_Text = errorText;
    }

    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<RoomField, UxmlTraits>
    { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription text = new UxmlStringAttributeDescription { name = nameof(Label_Text), defaultValue = "" };
        private readonly UxmlStringAttributeDescription value = new UxmlStringAttributeDescription { name = nameof(Value), defaultValue = "" };
        private readonly UxmlStringAttributeDescription button = new UxmlStringAttributeDescription { name = nameof(Button_Text), defaultValue = "Submit" };
        private readonly UxmlStringAttributeDescription error = new UxmlStringAttributeDescription { name = nameof(Error_Text), defaultValue = "" };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var element = ve as RoomField;

            element.Label_Text = text.GetValueFromBag(bag, cc);
            element.Value = value.GetValueFromBag(bag, cc);
            element.Button_Text = button.GetValueFromBag(bag, cc);
            element.Error_Text = error.GetValueFromBag(bag, cc);
        }
    }
    #endregion
}

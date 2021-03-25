using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuElement : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>(@"UI/Menus/MainMenu");







    private void Setup()
    {

    }

    public MainMenuElement()
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
    public new class UxmlFactory : UxmlFactory<MainMenuElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    { }
    #endregion
}

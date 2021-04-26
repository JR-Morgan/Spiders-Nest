using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Implementation of <see cref="MenuController"/> for a menu container
/// </summary>
public class MainMenuElement : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>(@"UI/Menus/MainMenu");

    public MainMenuElement()
    {
        Add(view.CloneTree());
    }

    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<MainMenuElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    { }
    #endregion
}

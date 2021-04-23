using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class LeaderBoardRecord : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>($@"UI/Menus/Leader Board/{nameof(LeaderBoardRecord)}");

    #region Child Elements
    public VisualElement Container { get; private set; }

    #endregion



    public void Setup()
    {
        Container = this.Q<VisualElement>("Container");
    }

    public LeaderBoardRecord()
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
    public new class UxmlFactory : UxmlFactory<LeaderBoardRecord, UxmlTraits>
    { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    { }
    #endregion
}
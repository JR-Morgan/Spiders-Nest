using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class LeaderBoardReport : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>($@"UI/Menus/Leader Board/{nameof(LeaderBoardReport)}");

    #region Child Elements
    public VisualElement Root { get; private set; }
    public Label Header { get; private set; }
    public VisualElement Cols { get; private set; }
    public VisualElement Records { get; private set; }

    #endregion



    public void Setup()
    {
        foreach (PropertyInfo property in typeof(LeaderBoardReport).GetProperties())
        {
            var element = this.Q(property.Name, property.PropertyType.ToString());
            if(element != null)
            {
                property.SetValue(this, element);
            }
        }
    }

    public LeaderBoardReport()
    {
        Add(view.CloneTree());
        //void GeometryChange(GeometryChangedEvent evt)
        //{
        //    this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
        //    Setup();
        //    //this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
        //}

        //this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
    }

    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<LeaderBoardReport, UxmlTraits>
    { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    { }
    #endregion
}
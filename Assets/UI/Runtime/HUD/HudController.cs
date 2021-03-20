using UnityEngine.UIElements;

public class Hud : VisualElement
{
    TextField score;
    private void Setup()
    {
        score = this.Q<TextField>("Score");
        score.Q("unity-text-input").pickingMode = PickingMode.Ignore;

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnChange.AddListener(Update);
        }

        Update(0);
    }

    void Update(int score)
    {
        this.score.value = score.ToString();
    }


    #region UXML Factory
    public new class UxmlFactory : UxmlFactory<Hud, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    { }


    public Hud()
    {
        void GeometryChange(GeometryChangedEvent evt)
        {
            this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
            Setup();
        }

        this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
    }
    #endregion
}

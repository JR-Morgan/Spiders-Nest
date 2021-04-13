using UnityEngine;
using UnityEngine.UIElements;

public class Hud : VisualElement
{
    private static readonly VisualTreeAsset view = Resources.Load<VisualTreeAsset>(@"UI/Views/HudElement");

    TextField score;
    private void Setup()
    {
        score = this.Q<TextField>("Score");
        score.Q("unity-text-input").pickingMode = PickingMode.Ignore;

        if (EnemyManager.TryGetInstance(out EnemyManager enemyManager))
        {
            enemyManager.OnEnemyKilled.AddListener(Update);
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
        Add(view.CloneTree());
        void GeometryChange(GeometryChangedEvent evt)
        {
            this.UnregisterCallback<GeometryChangedEvent>(GeometryChange);
            Setup();
        }

        this.RegisterCallback<GeometryChangedEvent>(GeometryChange);
    }
    #endregion
}

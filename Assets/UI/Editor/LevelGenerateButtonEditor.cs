using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(LevelGenerateButton))]
public class LevelGenerateButtonEditor : Editor
{
    private VisualElement root;
    private LevelGenerateButton levelGen;


    private void OnEnable()
    {
        levelGen = (LevelGenerateButton)target;
        root = new VisualElement();
        
    }



    public override VisualElement CreateInspectorGUI()
    {
        root.Clear();

        Button button = new Button
        {
            text = "Generate"
        };

        button.RegisterCallback<ClickEvent>(evt =>
        {
            levelGen.Generate();
        });

        root.Add(button);

        return root;
    }


}



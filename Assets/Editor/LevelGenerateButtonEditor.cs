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

        //Rooms
        {
            Button rooms = new Button
            {
                text = "Generate Rooms"
            };

            rooms.RegisterCallback<ClickEvent>(evt =>
            {
                levelGen.GenerateRooms();
            });

            root.Add(rooms);
        }

        return root;
    }


}



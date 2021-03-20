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

        //Walls
        {
            Button generate = new Button
            {
                text = "Generate Walls"
            };

            generate.RegisterCallback<ClickEvent>(evt =>
            {
                levelGen.GenerateWalls();
            });
       
        root.Add(generate);
        }

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



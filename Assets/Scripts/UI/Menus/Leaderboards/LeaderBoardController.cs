using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


[RequireComponent(typeof(UIDocument))]
public class LeaderBoardController : Singleton<LeaderBoardController>
{
    UIDocument uiDocument;

    protected void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        GenerateReport();
    }

    private void GenerateReport()
    { 
        LeaderBoardReport report = new LeaderBoardReport();
        Label errorLabel = report.Q<Label>("Error");
        report.Setup();

        report.Q<Button>("Back").RegisterCallback<ClickEvent>(e =>
        {
            SceneManager.LoadScene(0);
        });

        uiDocument.rootVisualElement.Q("Root").Add(report);

        StartCoroutine(BackendlessHelper.GetAll(Populate));

        void Populate(TableData data, string error)
        {
            if (data != null)
            {
                if (data.data.Length <= 0)
                {
                    errorLabel.text = "Nothing to show";
                }
                else
                {
                    errorLabel.RemoveFromHierarchy();
                }

                Debug.Log($"Received table data with {data.data.Length} elements", this);
                foreach (PlayerRecord p in data.data)
                {
                    bool isMe = p.Player_Name == System.Environment.UserName;
                    LeaderBoardRecord element = GenerateRecord(isMe, p.Player_Name, p.Total_Kills, p.Total_Levels);

                    report.Q<VisualElement>("Records").Add(element);
                }
            }
            else
            {
                errorLabel.text = error;
            }
        }
    }




    private LeaderBoardRecord GenerateRecord(bool isMe, params object[] values) => GenerateRecord(isMe, (IList<object>)values);
    private LeaderBoardRecord GenerateRecord(bool isMe, IList<object> values)
    {
        var record = new LeaderBoardRecord();
        record.Setup();
        foreach (object v in values)
        {
            VisualElement parent = new VisualElement();
            Label label = new Label(v.ToString());
            if (isMe)
            {
                label.style.fontSize = new StyleLength(17);
            }
            parent.Add(label);;
            record.Container.Add(parent);
        }

        if (isMe)
        {
            record.Container.style.backgroundColor = new Color(0f, 0f, 0f, 0.25f);
            record.Container.style.borderTopWidth = 2f;
            record.Container.style.borderBottomWidth = 2f;
            record.Container.style.borderLeftWidth = 2f;
            record.Container.style.borderRightWidth = 2f;
        }

        return record;
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[Serializable]
public class TableData
{
    public PlayerRecord[] data;
}

[Serializable]
public class PlayerRecord
{
    public string Player_Name;
    public int Total_Levels, Total_Kills;
} 
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

        StartCoroutine(RecieveData(Populate));

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
                    LeaderBoardRecord element = GenerateRecord(false, p.Player_Name, p.Total_Kills, p.Total_Levels);

                    report.Q<VisualElement>("Records").Add(element);
                }
            }
            else
            {
                errorLabel.text = error;
            }
        }
    }

    private IEnumerator RecieveData(Action<TableData, string> OnCompleteCallback)
    {
        Uri url = ConstructURL();
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest(); 

        if(request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogWarning(request.error);
        }
        //Debug.Log(request.downloadHandler.text);


        TableData tableRecords = null;
        string error = "";
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            error = request.error;
            Debug.Log(request.error);
        }
        else
        {
            tableRecords = JsonUtility.FromJson<TableData>("{\"data\":" + request.downloadHandler.text + "}");
        }

        OnCompleteCallback.Invoke(tableRecords, error);

    }


    private static Uri ConstructURL(string tableName = GlobalConstants.BACKENDLESS_LEADERBOARDS_TABLE, string apiKey = GlobalConstants.BACKENDLESS_API_KEY, string appID = GlobalConstants.BACKENDLESS_APP_ID)
    {
        return new Uri($@"https://eu-api.backendless.com/{appID}/{apiKey}/data/{tableName}");
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
                label.style.fontSize = new StyleLength(label.style.fontSize.value.value + 2);
            }
            parent.Add(label);;
            record.Container.Add(parent);
        }

        if (isMe)
        {
            record.style.borderTopWidth = 2f;
            record.style.borderBottomWidth = 2f;
            record.style.borderLeftWidth = 2f;
            record.style.borderRightWidth = 2f;
        }

        return record;
    }


}

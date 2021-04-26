using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class TableData
{
    public PlayerRecord[] data;
}

[Serializable]
public class PlayerRecord : PostPlayerRecord
{
    public string  objectId;
}
[Serializable]
public class PostPlayerRecord
{
    public string Player_Name;
    public int Total_Levels, Total_Kills;
}

/// <summary>
/// Allows the running of <see cref="BackendlessHelper"/> methods with a persistent <see cref="MonoBehaviour"/>
/// </summary>
public class BackendlessController : Singleton<BackendlessController>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Updates the backendless record by adding the sepcified <paramref name="killsToAdd"/> and <paramref name="levelsToAdd"/>
    /// </summary>
    /// <param name="playerName">The identifying name of the client</param>
    /// <param name="killsToAdd">Number of kills to add</param>
    /// <param name="levelsToAdd">Nunber of levels to add</param>
    public void AddScore(string playerName, int killsToAdd, int levelsToAdd)
    {
        this.StartCoroutine(BackendlessHelper.GetRecord(BackendlessHelper.URIByName(playerName), (player, errors) =>
        {

            if (player == null)
            {
                var newPlayer = new PostPlayerRecord()
                {
                    Player_Name = playerName,
                    Total_Kills = killsToAdd,
                    Total_Levels = levelsToAdd,
                };
                this.StartCoroutine(BackendlessHelper.PostRecord(newPlayer));
            }
            else
            {
                player.Total_Kills += killsToAdd;
                player.Total_Levels += levelsToAdd;
                this.StartCoroutine(BackendlessHelper.PutRecord(player));
            }
        }));
    }

}


/// <summary>
/// This class controls the <see cref="UnityEngine.Networking"/> requests to the Backendless server<\br>
/// Note that there is no checking of user permissions
/// </summary>
public class BackendlessHelper
{



    #region Helper Methods
    internal static string ConstructURL(string tableName = GlobalConstants.BACKENDLESS_LEADERBOARDS_TABLE, string apiKey = GlobalConstants.BACKENDLESS_API_KEY, string appID = GlobalConstants.BACKENDLESS_APP_ID, string objectID = null)
    {
        string uriString = $@"https://eu-api.backendless.com/{appID}/{apiKey}/data/{tableName}";

        if (objectID != null)
            uriString += '/' + objectID;

        return uriString;
    }

    internal static string URIByName(string name) => $"{ConstructURL()}?where=Player_Name%20%3D%20%27{name}%27";
    internal static string URIByID(string objectID) => ConstructURL(objectID: objectID);

    #endregion



    #region Get Request Coroutines
    /// <summary>
    /// Fetches all player score records from the backendless table specified in <see cref="GlobalConstants"/>
    /// </summary>
    /// <param name="OnCompleteCallback">This delegate will be called when the <see cref="TableData"/> has been succesfully fetched. The <see cref="string"/> parameter will contain any errors</param>
    /// <returns></returns>
    /// <remarks>Designed to run as a <see cref="Coroutine"/></remarks>
    public static IEnumerator GetAll(Action<TableData, string> OnCompleteCallback)
    {
        string uri = ConstructURL();
        using UnityWebRequest request = UnityWebRequest.Get(uri + "?sortBy=Total_Kills%20desc");
        LogRequest(request);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
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

    private static void LogRequest(UnityWebRequest request)
    {
        Debug.Log($"{request.method} Request to {request.uri}");
    }

    /// <summary>
    /// Querys the Backendless table for a specific player by object ID
    /// </summary>
    /// <param name="OnCompleteCallback">This delegate will be called when the <see cref="PlayerRecord"/> has been succesfully fetched. The <see cref="string"/> parameter will contain any errors</param>
    /// <returns></returns>
    /// <remarks>Designed to run as a <see cref="Coroutine"/></remarks>
    public static IEnumerator GetRecord(string uri, Action<PlayerRecord, string> OnCompleteCallback)
    {
        using UnityWebRequest request = UnityWebRequest.Get(uri);
        LogRequest(request);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(request.error);
        }
        //Debug.Log(request.downloadHandler.text);

        
        PlayerRecord record = null;
        string error = "";
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            error = request.error;
            Debug.Log(request.error);
        }
        else
        {
            TableData records = JsonUtility.FromJson<TableData>("{\"data\":" + request.downloadHandler.text + "}");
            if(records.data.Length > 0)
            {
                record = records.data[0];
            }
        }

        OnCompleteCallback.Invoke(record, error);
    }

    #endregion

    #region Put Requests
    /// <summary>
    /// Updates record with the <see cref="PlayerRecord.objectId"/> specified in <paramref name="record"/> with the values from <paramref name="record"/>
    /// </summary>
    /// <param name="record">The modified <see cref="PlayerRecord"/>, <see cref="PlayerRecord.objectId"/> must already exist in the table</param>
    /// <returns></returns>
    /// <remarks>Designed to run as a <see cref="Coroutine"/></remarks>
    public static IEnumerator PutRecord(PlayerRecord record)
    {
        string uri = ConstructURL(objectID: record.objectId);

        string data = JsonUtility.ToJson(record);

        using UnityWebRequest request = UnityWebRequest.Put(uri, data);
        request.SetRequestHeader("Content-Type", "application/json");
        LogRequest(request);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(request.error);
        }
    }

    #endregion

    #region Post Requests
    /// <summary>
    /// Creates a new record on server with data from <paramref name="record"/>.<br/>
    /// Note that the <see cref="PostPlayerRecord.objectId"/> of <paramref name="record"/> is ignored.
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    /// <remarks>Designed to run as a <see cref="Coroutine"/></remarks>
    public static IEnumerator PostRecord(PostPlayerRecord record)
    {
        string uri = ConstructURL();

        string data = JsonUtility.ToJson(record);

        //For some bizarre reason, UnityWebRequest can't send JSON through POST requests, so I have to use a Put here.
        using UnityWebRequest request = UnityWebRequest.Put(uri, data);
        request.SetRequestHeader("Content-Type", "application/json");
        LogRequest(request);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(request.error);
        }
    }

    #endregion




}

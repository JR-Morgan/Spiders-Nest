using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitchoverManager : Singleton<LevelSwitchoverManager>
{
    private const int CREDITS_INDEX = 0;

    protected override void Awake()
    {
        base.Awake();
        PlayerManager p = FindObjectOfType<PlayerManager>();

        if (p == null) //If there aren't managers in this scene then add them.
        {
            SceneManager.LoadScene("Level Managers", LoadSceneMode.Additive);
        }


        foreach(ChestInteractable chest in FindObjectsOfType<ChestInteractable>())
        {
            chestsToWin++;
            chest.OnOpen.AddListener(ChestOpenEventHandler);
        }
    }

    private int chestsToWin;
    private void ChestOpenEventHandler()
    {
        chestsToWin--;
        if(chestsToWin <= 0)
        {
            //WIN GAME!
            int level = SceneManager.GetActiveScene().buildIndex + 1;
            if(level >= SceneManager.sceneCountInBuildSettings)
            {
                LoadScene(true, CREDITS_INDEX, CursorLockMode.None, typeof(PlayerBehaviour));

                if (File.Exists(PlayerBehaviour.PLAYER_STATE_PATH))
                {
                    File.Delete(PlayerBehaviour.PLAYER_STATE_PATH);
                }
            }
            else
            {
                LoadScene(true, level, CursorLockMode.Locked);
            }
        }
    }


    public static void LoadScene(bool levelComplete, int sceneIndex, CursorLockMode cursorMode, params Type[] typesToDestroy) => LoadScene(levelComplete, sceneIndex, cursorMode, (IEnumerable<Type>) typesToDestroy);
    /// <summary>
    /// Loads <see cref="Scene"/> with build index <paramref name="sceneIndex"/>
    /// </summary>
    /// <param name="shouldSave">When <c>true</c> will call <see cref="SaveGame"/></param>
    /// <param name="sceneIndex">The build index of the <see cref="Scene"/> to be loaded</param>
    /// <param name="cursorMode">The desired <see cref="CursorLockMode"/> to be applied on scene change</param>
    /// <param name="typesToDestroy">The <see cref="Component"/>'s to be destroyed on scene load</param>
    public static void LoadScene(bool levelComplete, int sceneIndex, CursorLockMode cursorMode, IEnumerable<Type> typesToDestroy)
    {
        SaveGame(levelComplete ? 1:0);
        //Load new scene
        SceneManager.LoadScene(sceneIndex);
        UnityEngine.Cursor.lockState = cursorMode;

        //Reset time in case was paused
        Time.timeScale = 1;

        //Destroy player
        foreach(Type c in typesToDestroy)
        {
            if (c.IsSubclassOf(typeof(Component)))
            {
                foreach (Component player in FindObjectsOfType(c))
                {
                    Destroy(player.gameObject);
                }
            }
        }

    }


    /// <summary>
    /// Attempts to add kills to Backendless and will serialise level if the following condition is <c>true</c>
    /// <code>!<see cref="PhotonNetwork.IsConnected"/></code>
    /// </summary>
    public static void SaveGame(int levelsToAdd = 0)
    {
        BackendlessController.Instance.AddScore(System.Environment.UserName, EnemyManager.Instance.NumberOfKills, levelsToAdd);

        if (!PhotonNetwork.IsConnected)
        {
            SerialiseLevel();
            EnemyManager.Instance.SerialiseEnemies();
            LevelStateManager.Instance.SerialiseLevel();
            PlayerManager.Instance.Local.SerialisePlayer();
        }
    }



    #region Serialisation

    public static string LEVEL_STATE_PATH => Application.persistentDataPath + @"/levelState.json";

    public static bool DeserialiseLevel(out int sceneIndex)
    {
        sceneIndex = -1;
        try
        {
            string json = File.ReadAllText(LEVEL_STATE_PATH);
            LevelState state = JsonUtility.FromJson<LevelState>(json);
            sceneIndex = state.sceneIndex;
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n{e.StackTrace}");
        }

        return sceneIndex >= 0;
    }



    public static void SerialiseLevel()
    {
        LevelState l = new LevelState
        {
            sceneIndex = SceneManager.GetActiveScene().buildIndex,
        };

        if (!File.Exists(LEVEL_STATE_PATH)) File.Create(LEVEL_STATE_PATH).Dispose();
        File.WriteAllText(LEVEL_STATE_PATH, JsonUtility.ToJson(l));
    }


    [Serializable]
    private struct LevelState
    {
        public int sceneIndex;
    }


    #endregion
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Implementation of <see cref="MenuController"/> for the multi player menu
/// </summary>
public class PUNMenuController : MenuController
{
    private MainMenuElement menu;
    private PUNManager pun;


    protected override void Awake()
    {
        base.Awake();

        pun = PUNManager.Instance;

        if(pun == null)
        {
            Debug.LogError($"{typeof(PUNMenuController)} could not find an instance of {typeof(PUNManager)}", this);
            return;
        }

        menu = InitialiseNewElement("Multi Player");
        VisualElement optionRoot = menu.Q(OPTION_ROOT);

        { //Join Room
            RoomField newRoomElement = new RoomField("New Room", "Room Name", "Create");
            newRoomElement.OnSubmit += () => CreateRoom(newRoomElement);
            optionRoot.Add(newRoomElement);
        }

        { //Join Room
            RoomField joinRoomElement = new RoomField("Join Room", "Room Name", "Connect");
            joinRoomElement.OnSubmit += () => JoinRoom(joinRoomElement);
            optionRoot.Add(joinRoomElement);
        }
        optionRoot.Add(InitialiseOption("Back to Main Menu", () =>
        {
            StartAnimation(AnimState.Out, OnComplete: () =>
            {
                Destroy(PUNManager.Instance);
                SceneManager.LoadScene(0);
            });
        }));

    }

    protected override void Start()
    {
        base.Start();

        documentParent.Add(menu);
        _currentMenu = menu;

        StartAnimation(AnimState.In,
            OnComplete: () => animationSpeed = 1.5f,
            delay: 1f); ;
    }

    #region Button Callbacks
    private void CreateRoom(RoomField element)
    {
        element.Error_Text = "";
        if (IsValidInput(element))
        {
            if (pun.CreateNewRoom(element.Value))
            {
                element.Error_Text += "Attempting to create room...\n";
                pun.OnCreatedRoomEvent.AddListener(() =>
                {
                    element.Error_Text += "Room created successfully\n";
                });
                pun.OnJoinedRoomEvent.AddListener(() =>
                {
                    element.Error_Text += "Room joined successfully\n";
                });
                pun.OnCreateRoomFailedEvent.AddListener((c, r) =>
                {
                    element.Error_Text += $"Error {c}: {r}\n";
                });
                pun.OnJoinRoomFailedEvent.AddListener((c, r) =>
                {
                    element.Error_Text += $"Error {c}: {r}\n";
                });
            }
            else
            {
                element.Error_Text += "Failed to send message to photon server\n";
            }
        }
    }

    private void JoinRoom(RoomField element)
    {
        element.Error_Text = "";
        if (IsValidInput(element))
        {
            if (pun.JoinRoom(element.Value))
            {
                element.Error_Text = "Attempting to join room...\n";
                pun.OnJoinedRoomEvent.AddListener(() =>
                {
                    element.Error_Text += "Room joined successfully\n";
                });
                pun.OnJoinRoomFailedEvent.AddListener((c,r) =>
                {
                    element.Error_Text += $"Error {c}: {r}\n";
                });
            }
            else
            {
                element.Error_Text += "Failed to send message to photon server\n";
            }
        }
    }
    #endregion

    #region Helper Methods
    private bool IsValidInput(RoomField element, int minChar = 4)
    {
        Debug.Log(element.Value);
        if (string.IsNullOrWhiteSpace(element.Value))
        {
            element.Error_Text += "Room name cannot be blank\n";
            return false;
        }

        if (element.Value.Length <= minChar)
        {
            element.Error_Text += $"Room name must be greater than {minChar} characters\n";
            return false;
        }
        return true;
    }

    #endregion




}

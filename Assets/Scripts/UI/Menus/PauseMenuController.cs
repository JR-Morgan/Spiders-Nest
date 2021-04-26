using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Implementation of <see cref="MenuController"/> for a pause menu
/// </summary>
public class PauseMenuController : MenuController
{
    private const string CONTAINER_ELEMENT_NAME = "PauseMenuContainer";

    VisualElement container;
    protected override void Start()
    {
        base.Start();

        container = document.rootVisualElement.Q(CONTAINER_ELEMENT_NAME);
        Debug.Assert(container != null, $"{typeof(UIDocument)} did not contain an element with name {CONTAINER_ELEMENT_NAME}", this);


        MainMenuElement pause = InitialiseNewElement("Pause");
        VisualElement optionRoot = pause.Q(OPTION_ROOT);

        _currentMenu = pause;
        container.Add(pause);

        optionRoot.Add(InitialiseOption("Continue", Continue));

        OptionButton exit = InitialiseOption("Save and Exit");
        exit.RegisterCallback<ClickEvent>(e => BackToMain());
        optionRoot.Add(exit);

    }

    [SerializeField]
    private bool isPaused;

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        _currentMenu.SetEnabled(true);
        container.Add(_currentMenu);
        isPaused = true;
        if (!PhotonNetwork.IsConnected)
        {
            Time.timeScale = 0;
        }

        StartAnimation(AnimState.In);
    }

    private void Continue()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        _currentMenu.SetEnabled(false);
        StartAnimation(AnimState.Out, () => {
            isPaused = false;
            Time.timeScale = 1;
            _currentMenu.RemoveFromHierarchy();
        });
    }

    private void BackToMain()
    {
        _currentMenu.SetEnabled(false);
        Debug.Log("Exiting to main");

        LevelSwitchoverManager.LoadScene(false, 0, CursorLockMode.None, typeof(PlayerState));
    }
}

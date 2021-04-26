using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// This class controls the setup and functionality of the Main Menu scene.
/// </summary>
public class MainMenuController : MenuController
{
    private const string LEVEL_1 = "Level 1";
    [SerializeField]
    private AudioMixer masterMixer;

    private MainMenuElement mainMenu;
    private MainMenuElement singlePlayerMenu;
    private MainMenuElement optionsMenu;

    protected override void Awake()
    {
        base.Awake();

        { //Main Menu

            mainMenu = InitialiseNewElement(GlobalConstants.NAME_OF_GAME);
            VisualElement optionRoot = mainMenu.Q(OPTION_ROOT);

            optionRoot.Add(InitialiseOption("Single Player", () => CurrentMenu = singlePlayerMenu));
            optionRoot.Add(InitialiseOption("Multi Player", () => SceneManager.LoadScene(1)));
            optionRoot.Add(InitialiseOption("Options", () => CurrentMenu = optionsMenu));
            optionRoot.Add(InitialiseOption("Leaderboard", () => {
                StartAnimation(AnimState.Out, () => SceneManager.LoadScene(2));
            }));
            optionRoot.Add(InitialiseOption("Exit", () => {
                Application.Quit();
            }));

        }

        { //Single Player

            singlePlayerMenu = InitialiseNewElement("Single Player");
            VisualElement optionRoot = singlePlayerMenu.Q(OPTION_ROOT);

            optionRoot.Add(InitialiseOption("New Game", () => StartNewGame()));
            var continueLast = InitialiseOption("Continue Last", () => Continue());
            optionRoot.Add(continueLast);
            optionRoot.Add(InitialiseOption("Back to Main Menu", () => CurrentMenu = mainMenu));

            //Disable continue option if no save exists.
            continueLast.SetEnabled(File.Exists(PlayerState.PLAYER_STATE_PATH));


        }

        { //Options
            

            optionsMenu = InitialiseNewElement("Options");
            VisualElement optionRoot = optionsMenu.Q(OPTION_ROOT);

            Slider sfxSlider = new Slider("SFX Volume : ", -80f, 10f);
            optionRoot.Add(sfxSlider);
            sfxSlider.RegisterCallback<ChangeEvent<float>>(e =>
            {
                masterMixer.SetFloat("SFX", e.newValue);
            });

            Slider musicSlider = new Slider("Music Volume : ", -80f, 10f);
            optionRoot.Add(musicSlider);
            musicSlider.RegisterCallback<ChangeEvent<float>>(e =>
            {
                masterMixer.SetFloat("Music", e.newValue);
            });

            Toggle fullScreen = new Toggle("Full-screen : ");
            optionRoot.Add(fullScreen);
            fullScreen.value = Screen.fullScreen;
            fullScreen.RegisterCallback<ChangeEvent<bool>>(e =>
            {
                Screen.fullScreen = e.newValue;
            });


            optionRoot.Add(InitialiseOption("Back to Main Menu", () => CurrentMenu = mainMenu));

        }

    }

    protected override void Start()
    {
        base.Start();

        documentParent.Add(mainMenu);
        _currentMenu = mainMenu;

        StartAnimation(AnimState.In,
            OnComplete: () => animationSpeed = 1.5f,
            delay: 1f); ;
    }


    private void StartNewGame()
    {
        StartAnimation(AnimState.Out, () => SceneManager.LoadScene(LEVEL_1));
    }

    /// <summary>
    /// Attempts to restore game state from file
    /// </summary>
    private void Continue()
    {
        StartAnimation(AnimState.Out, () =>
        {
            if(LevelSwitchoverManager.DeserialiseLevel(out int level))
            {
                SceneManager.LoadScene(level);

                PlayerState.OnSerialisationReady += LoadFromSave;

            }
            else
            {
                Debug.Log("No game state exists to restore from, starting new game");
                SceneManager.LoadScene(LEVEL_1);
            }

        });
    }

    private void LoadFromSave()
    {
        PlayerState.OnSerialisationReady -= LoadFromSave;
        try
        {
            EnemyManager.Instance.DeserialiseEnemies();
            LevelStateManager.Instance.DeserialiseLevel();
            PlayerManager.Instance.Local.DeserialisePlayer();
            Debug.Log("Restore game state was successful");
        }
        catch (Exception e)
        {
            Debug.LogError($"Game save was corrupted\n{e.Message}\n{e.StackTrace}", this);
        }
    }

}

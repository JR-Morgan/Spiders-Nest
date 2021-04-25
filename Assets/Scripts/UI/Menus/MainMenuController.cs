using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MenuController
{
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

        }

        { //Single Player

            singlePlayerMenu = InitialiseNewElement("Single Player");
            VisualElement optionRoot = singlePlayerMenu.Q(OPTION_ROOT);

            optionRoot.Add(InitialiseOption("New Game", () => StartNewGame()));
            optionRoot.Add(InitialiseOption("Continue Last", () => Continue()));
            optionRoot.Add(InitialiseOption("Back to Main Menu", () => CurrentMenu = mainMenu));
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

        documentRoot.Add(mainMenu);
        _currentMenu = mainMenu;

        StartAnimation(AnimState.In,
            OnComplete: () => animationSpeed = 1.5f,
            delay: 1f); ;
    }


    private void StartNewGame()
    {
        StartAnimation(AnimState.Out, () => SceneManager.LoadScene("Level 1"));
    }

    private void Continue()
    {
        StartAnimation(AnimState.Out, () => {
            if(LevelSwitchoverManager.DeserialiseLevel(out int level))
            {
                SceneManager.LoadScene(level);

                PlayerBehaviour.OnSerialisationReady += () =>
                {
                    bool successful =
                    EnemyManager.Instance.DeserialiseEnemies()
                    && LevelStateManager.Instance.DeserialiseLevel()
                    && PlayerManager.Instance.Local.DeserialisePlayer();

                    if (!successful)
                    {
                        Debug.LogError("Game save was corrupted");
                    }
                };

            }
            else
            {
                SceneManager.LoadScene("Level 1");
            }


        });
    }


}

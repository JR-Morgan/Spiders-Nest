using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MenuController
{
    private const string NAME_OF_GAME = "Veins of Hell";

    private MainMenuElement mainMenu;
    private MainMenuElement singlePlayerMenu;
    private MainMenuElement multiPlayerMenu;

    protected override void Awake()
    {
        base.Awake();

        { //Main Menu

            mainMenu = InitialiseNewElement(NAME_OF_GAME);
            VisualElement optionRoot = mainMenu.Q(OPTION_ROOT);

            optionRoot.Add(InitialiseOption("Single Player", () => CurrentMenu = singlePlayerMenu));
            optionRoot.Add(InitialiseOption("Multi Player", () => CurrentMenu = multiPlayerMenu));
            optionRoot.Add(InitialiseOption("Leader Boards", () => {
                StartAnimation(AnimState.Out, () => SceneManager.LoadScene(2));
            }));

        }

        { //Single Player

            singlePlayerMenu = InitialiseNewElement("Single Player");
            VisualElement optionRoot = singlePlayerMenu.Q(OPTION_ROOT);

            optionRoot.Add(InitialiseOption("New Game", () => StartNewGame()));
            optionRoot.Add(InitialiseOption("Continue Last", () => Continue())); //TODO 
            optionRoot.Add(InitialiseOption("Back to Main Menu", () => CurrentMenu = mainMenu));
        }

        { //Multi Player

            multiPlayerMenu = InitialiseNewElement("Multi Player");
            VisualElement optionRoot = multiPlayerMenu.Q(OPTION_ROOT);

            optionRoot.Add(InitialiseOption("Photon Servers", () => {
                StartAnimation(AnimState.Out, () => SceneManager.LoadScene(1));
                }));
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
        StartAnimation(AnimState.Out, () => SceneManager.LoadScene(3));
    }

    private void Continue()
    {
        StartAnimation(AnimState.Out, () => SceneManager.LoadScene(3));
    }

    private void LeaderBoards()
    {

    }

}

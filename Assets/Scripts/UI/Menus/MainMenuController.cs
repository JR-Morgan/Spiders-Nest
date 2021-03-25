using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    private enum AnimState { In, Out, None }
    private AnimState animState;
    private Action OnOut;
    private float animProgress = 0f;


    [SerializeField]
    private float animationSpeed = 1f;



    private MainMenuElement element;
    private UIDocument document;


    private void Awake()
    {
        document = GetComponent<UIDocument>();
        element = new MainMenuElement();


        void InitialiseOption(OptionButton b, EventCallback<ClickEvent> OnClick)
        {
            b.RegisterCallback(OnClick);
            b.RegisterCallback<MouseEnterEvent>(evt => SelectedStart(b));
            b.RegisterCallback<MouseLeaveEvent>(evt => SelectedEnd(b));
        }


        OptionButton newGame = element.Q<OptionButton>("New");
        InitialiseOption(newGame, StartNewGame);

        OptionButton continueGame = element.Q<OptionButton>("Continue");
        InitialiseOption(continueGame, Continue);

        OptionButton options = element.Q<OptionButton>("Options");
        InitialiseOption(options, Options);

    }

    public void Start()
    {
        MainMenuElement existing = document.rootVisualElement.Q<MainMenuElement>();
        if (existing != null) existing.RemoveFromHierarchy();
        document.rootVisualElement.Q<VisualElement>("Root").Add(element);

        StartAnimation(AnimState.In, delay: 1f);
    }

    private void SelectedStart(OptionButton e)
    {
        e.Is_Selected = true;
    }

    private void SelectedEnd(OptionButton e)
    {
        e.Is_Selected = false;
    }

    private void StartNewGame(ClickEvent evt = null)
    {
        StartAnimation(AnimState.Out, () => SceneManager.LoadScene(1));
    }

    private void Continue(ClickEvent evt = null)
    {
        StartAnimation(AnimState.Out, () => SceneManager.LoadScene(1));
    }

    private void Options(ClickEvent evt = null)
    {

    }

    private void StartAnimation(AnimState anim, Action OnOut = null, float delay = 0f)
    {
        if (OnOut != null) this.OnOut = OnOut;

        animState = anim;
        animProgress = 1f + delay;
    }

    private void Update()
    {
        if(animState != AnimState.None)
        {
            animProgress -= animationSpeed * Time.deltaTime;

            float alpha = animState switch
            {
                AnimState.In => Mathf.Lerp(1f, 0f, animProgress),
                AnimState.Out => Mathf.Lerp(0f, 1f, animProgress),
                _ => 1f,
            };

            element.style.opacity = alpha;

            if (animProgress <= 0)
            {
                if (animState == AnimState.Out)
                {
                    OnOut?.Invoke();
                }

                animState = AnimState.None;
            }

        }
    }





}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class MenuController : MonoBehaviour
{
    protected const string OPTION_ROOT = "OptionRoot";

    protected UIDocument document;
    protected VisualElement documentRoot;

    protected MainMenuElement _currentMenu;
    protected MainMenuElement CurrentMenu
    {
        get => _currentMenu;
        set
        {
            if (_currentMenu == null)
            {
                enableNext();
                return;
            }

            StartAnimation(AnimState.Out, enableNext);

            void enableNext()
            {
                _currentMenu.RemoveFromHierarchy();

                _currentMenu = value;

                documentRoot.Add(_currentMenu);
                StartAnimation(AnimState.In);
            }
        }
    }

    protected virtual void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    protected static MainMenuElement InitialiseNewElement(string title)
    {
        MainMenuElement menu = new MainMenuElement();
        menu.style.opacity = 0f;
        menu.Q<Label>("Title").text = title;
        return menu;
    }

    protected OptionButton InitialiseOption(string text, Action OnClick)
    {
        OptionButton b = new OptionButton
        {
            Label_Text = text
        };
        b.RegisterCallback<ClickEvent>(
            evt => {
                OnClick.Invoke();
                SelectedEnd(b);
            });

        b.RegisterCallback<MouseEnterEvent>(evt => SelectedStart(b));
        b.RegisterCallback<MouseLeaveEvent>(evt => SelectedEnd(b));
        return b;
    }

    protected virtual void Start()
    {
        documentRoot = document.rootVisualElement.Q<VisualElement>("Root");
    }

    protected virtual void Update()
    {
        AnimationUpdate();
    }


    protected void SelectedStart(OptionButton e)
    {
        e.Is_Selected = true;
    }

    protected void SelectedEnd(OptionButton e)
    {
        e.Is_Selected = false;
    }

    #region Animation
    protected enum AnimState { None, In, Out }
    private AnimState animState;
    private Action OnAnimationComplete;
    private float animProgress = 0f;
    protected float animationSpeed = 1f;

    protected void StartAnimation(AnimState anim, Action OnComplete = null, float delay = 0f)
    {
        this.OnAnimationComplete = OnComplete;

        animState = anim;
        animProgress = 1f + delay;
    }

    protected void AnimationUpdate()
    {
        if (animState != AnimState.None)
        {
            animProgress -= animationSpeed * Time.deltaTime;

            float alpha = animState switch
            {
                AnimState.In => Mathf.Lerp(1f, 0f, animProgress),
                AnimState.Out => Mathf.Lerp(0f, 1f, animProgress),
                _ => 1f,
            };

            _currentMenu.style.opacity = alpha;

            if (animProgress <= 0) //If animation has finished
            {
                animState = AnimState.None;
                OnAnimationComplete?.Invoke();
            }

        }
    }

    #endregion



}

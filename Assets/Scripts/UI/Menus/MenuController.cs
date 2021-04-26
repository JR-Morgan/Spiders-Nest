using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This class is an abstract implementation of a menu controller that may be used for constructing menus such as main menus with fade in/out transitions.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public abstract class MenuController : MonoBehaviour
{
    /// <summary>This is the default root element name in the active UXML document that menu objects should be added to</summary>
    protected const string OPTION_ROOT = "OptionRoot";

    protected UIDocument document;
    /// <summary>The desired parent element of the menu</summary>
    protected VisualElement documentParent;

    protected MainMenuElement _currentMenu;

    /// <summary>Sets/Gets the currently active menu. Setting this variable will animate out the last current menu and remove it from the hierarchy.</summary>
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

                documentParent.Add(_currentMenu);
                StartAnimation(AnimState.In);
            }
        }
    }

    protected virtual void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    #region Factory Methods
    /// <summary>
    /// Helper method to create a new <see cref="MainMenuElement"/>
    /// </summary>
    /// <param name="title">The desired text of the "Title" element</param>
    /// <returns>A new setup  <see cref="MainMenuElement"/></returns>
    protected static MainMenuElement InitialiseNewElement(string title)
    {
        MainMenuElement menu = new MainMenuElement();
        menu.style.opacity = 0f;
        menu.Q<Label>("Title").text = title;
        return menu;
    }

    /// <summary>
    /// Helper method to create a <see cref="OptionButton"/>
    /// </summary>
    /// <param name="text">The desired <see cref="OptionButton.Label_Text"/></param>
    /// <param name="OnClick">Desired handler for the <see cref="ClickEvent"/></param>
    /// <returns>A new setup <see cref="OptionButton"/> </returns>
    protected OptionButton InitialiseOption(string text, Action OnClick = null)
    {
        OptionButton b = new OptionButton
        {
            Label_Text = text
        };
        b.RegisterCallback<ClickEvent>(
            evt => {
                OnClick?.Invoke();
                HoverEnd(b);
            });

        b.RegisterCallback<MouseEnterEvent>(evt => HoverStart(b));
        b.RegisterCallback<MouseLeaveEvent>(evt => HoverEnd(b));
        return b;
    }
    protected void HoverStart(OptionButton b)
    {
        b.Is_Selected = true;
    }

    protected void HoverEnd(OptionButton b)
    {
        b.Is_Selected = false;
    }
    #endregion

    protected virtual void Start()
    {
        documentParent = document.rootVisualElement.Q<VisualElement>("Root");
    }

    protected virtual void Update()
    {
        AnimationUpdate();
    }


    #region Animation
    protected enum AnimState { None, In, Out }
    private AnimState animState;
    private Action OnAnimationComplete;
    private float animProgress = 0f;
    protected float animationSpeed = 1f;

    /// <summary>
    /// Triggers an animation of type <paramref name="anim"/>
    /// </summary>
    /// <param name="anim">the desired animation type</param>
    /// <param name="OnComplete">Callback on animation complete</param>
    /// <param name="delay">delay in seconds</param>
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
            animProgress -= animationSpeed * Time.unscaledDeltaTime;

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

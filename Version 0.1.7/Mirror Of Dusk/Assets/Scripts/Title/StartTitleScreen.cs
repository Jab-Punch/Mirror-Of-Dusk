using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartTitleScreen : AbstractMB
{
    public AudioClip[] SelectSound;
    [SerializeField] private SpriteAnimator JabPunchSplash;
    [SerializeField] private SpriteRenderer fader;

    private MirrorOfDuskInput.AnyPlayerInput input;
    private bool isConsole;
    private bool shouldLoadSlotSelect;
    [SerializeField] private Text[] titleMenuItems;
    private StartTitleScreen.MainMenuItem[] _availableTitleMenuItems;
    [SerializeField] private RectTransform pressAnyChild;
    [SerializeField] private RectTransform titleMenuChild;
    [SerializeField] private FlashText titlePressAnyTextChildFlash;
    [SerializeField] private FlashText titleStartTextChildFlash;
    [SerializeField] private Color titleMenuSelectedColor;
    [SerializeField] private Color titleMenuUnselectedColor;
    [SerializeField] private ScrollingSprite[] _scrollingItems;
    private int _titleMenuSelection;

    private const string PATH = "Audio/TitleScreenAudio";
    private float timeSinceStart;

    public enum State
    {
        Animating,
        Title,
        TitleSelect,
        EnterGame
    }

    public enum MainMenuItem
    {
        Start,
        Exit
    }

    public StartTitleScreen.State state { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        MirrorOfDusk.Init(false);
        //MirrorOfDuskTime.Reset();
        PauseManager.Reset();
        this.shouldLoadSlotSelect = false;
        this.input = new MirrorOfDuskInput.AnyPlayerInput(false);
        this.isConsole = PlatformHelper.IsConsole;
        PlayerData.inGame = false;
        PlayerManager.ResetPlayers();
        List<Text> list = new List<Text>(this.titleMenuItems);
        List<StartTitleScreen.MainMenuItem> list2 = new List<StartTitleScreen.MainMenuItem>((StartTitleScreen.MainMenuItem[])Enum.GetValues(typeof(StartTitleScreen.MainMenuItem)));
        if (this.isConsole)
        {
            this.titleMenuItems[1].gameObject.SetActive(false);
            list.RemoveAt(1);
            list2.RemoveAt(1);
        }
        this.titleMenuItems = list.ToArray();
        this._availableTitleMenuItems = list2.ToArray();
    }

    void Start()
    {
        if (PlatformHelper.PreloadSettingsData)
        {
            SettingsData.ApplySettingsOnStartup();
        }
        /*if (AudioNoiseHandler.Instance != null)
        {
            AudioNoiseHandler.Instance.ConfirmSound();
        }*/
        if (StartScreenAudio.Instance == null)
        {
            StartScreenAudio startScreenAudio = UnityEngine.Object.Instantiate(Resources.Load("Audio/TitleScreenAudio")) as StartScreenAudio;
            startScreenAudio.name = "StartScreenAudio";
        }
        SettingsData.ApplySettingsOnStartup();
        this.SetState(StartTitleScreen.State.Animating);
        base.FrameDelayedCallback(new Action(this.StartFrontendSnapshot), 1);
        base.StartCoroutine(this.loop_cr());
    }
    
    void Update()
    {
        StartTitleScreen.State state = this.state;
        if (state != StartTitleScreen.State.Animating)
        {
            if (state == StartTitleScreen.State.Title)
            {
                if (shouldLoadSlotSelect)
                {
                    if (PlatformHelper.IsConsole)
                    {
                        this.EnterGame();
                    } else
                    {
                        this.state = StartTitleScreen.State.TitleSelect;
                        this.SetState(StartTitleScreen.State.TitleSelect);
                        for (int i = 0; i < this.titleMenuItems.Length; i++)
                        {
                            this.titleMenuItems[i].color = ((this._titleMenuSelection != i) ? this.titleMenuUnselectedColor : this.titleMenuSelectedColor);
                            this.titleMenuItems[i].fontSize = ((this._titleMenuSelection != i) ? 36 : 48);
                        }
                        if (PlatformHelper.IsConsole)
                        {
                            PlayerManager.LoadControllerMappings(PlayerId.PlayerOne);
                        }
                        this.SetRichPresence();
                    }
                }
            }
            else if(state == StartTitleScreen.State.TitleSelect)
            {
                this.timeSinceStart += Time.deltaTime;
                this.UpdateTitleMenu();
            }
        }
    }

    private void OnDestroy()
    {
        PlayerManager.OnPlayerJoinedEvent -= this.onPlayerJoined;
    }

    private void onPlayerJoined(PlayerId playerId)
    {
        this.shouldLoadSlotSelect = true;
    }

    private IEnumerator loop_cr()
    {
        yield return new WaitForSeconds(1f);
        SettingsData.Data.hasBootedUpGame = true;
        this.state = StartTitleScreen.State.Title;
        this.SetState(StartTitleScreen.State.Title);
        PlayerManager.OnPlayerJoinedEvent += this.onPlayerJoined;
        PlayerManager.SetPlayerCanJoin(PlayerId.PlayerOne, true, false);
        yield break;
    }

    protected virtual void StartFrontendSnapshot()
    {
        AudioManager.HandleSnapshot(AudioManager.Snapshots.FrontEnd.ToString(), 0.15f);
    }

    private void SetState(StartTitleScreen.State state)
    {
        this.state = state;
        this.pressAnyChild.gameObject.SetActive(state == StartTitleScreen.State.Title);
        this.titleMenuChild.gameObject.SetActive(state == StartTitleScreen.State.TitleSelect);
    }

    private void UpdateTitleMenu()
    {
        if (this.timeSinceStart < 0.25f)
        {
            return;
        }
        if (this.GetButtonDown(MirrorOfDuskButton.MenuDown))
        {
            this._titleMenuSelection = (this._titleMenuSelection + 1) % this.titleMenuItems.Length;
        }
        if (this.GetButtonDown(MirrorOfDuskButton.MenuUp))
        {
            this._titleMenuSelection--;
            if (this._titleMenuSelection < 0)
                this._titleMenuSelection = this.titleMenuItems.Length - 1;
        }
        for (int i = 0; i < this.titleMenuItems.Length; i++)
        {
            this.titleMenuItems[i].color = ((this._titleMenuSelection != i) ? this.titleMenuUnselectedColor : this.titleMenuSelectedColor);
            this.titleMenuItems[i].fontSize = ((this._titleMenuSelection != i) ? 36 : 48);
        }
        if (this.GetButtonDown(MirrorOfDuskButton.Accept) || this.GetButtonDown(MirrorOfDuskButton.Pause))
        {
            switch (this._availableTitleMenuItems[this._titleMenuSelection])
            {
                case StartTitleScreen.MainMenuItem.Start:
                    this.EnterGame();
                    break;
                case StartTitleScreen.MainMenuItem.Exit:
                    Application.Quit();
                    break;
            }
        }
    }

    private void EnterGame()
    {
        AudioManager.Play("enter_game");
        this.state = StartTitleScreen.State.EnterGame;
        for (int i = 0; i < _scrollingItems.Length; i++)
        {
            _scrollingItems[i].enabled = false;
        }
        if (PlatformHelper.IsConsole)
        {
            titlePressAnyTextChildFlash.flashOn = true;
            titlePressAnyTextChildFlash.flashOnTime = 2f;
            titlePressAnyTextChildFlash.flashOffTime = 2f;
        } else
        {
            titleStartTextChildFlash.flashOn = true;
        }
        StartCoroutine(this.enter_cr());
    }

    private IEnumerator enter_cr()
    {
        yield return new WaitForSeconds(1.5f);
        SceneLoader.LoadScene("scene_main_menu", SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.Shard);
        yield return null;
        yield break;
    }

    protected bool GetButtonDown(MirrorOfDuskButton button)
    {
        if (this.input.GetButtonDown(button))
        {
            return true;
        }
        return false;
    }

    private void SetRichPresence()
    {
        OnlineManager.Instance.Interface.SetRichPresence(PlayerId.Any, "TitleSelect", true);
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControllerDisconnectedPrompt : InterruptingPrompt
{
    public static ControllerDisconnectedPrompt Instance;

    public PlayerId currentPlayer;
    public bool allowedToShow;

    [SerializeField] private Text playerText;

    //[SerializeField] private LocalizationHelper localizationHelper;

    protected override void Awake()
    {
        base.useGUILayout = false;
        ControllerDisconnectedPrompt.Instance = this;
    }

    public void Show(PlayerId player)
    {
        this.currentPlayer = player;
        //this.localizationHelper.currentID = Localization.Find((player != PlayerId.PlayerOne) ? "XboxPlayer2" : "XboxPlayer1").id;
        PlayerManager.OnDisconnectPromptDisplayed(player);
        base.Show();
    }
    
    private void Update()
    {
        if (base.Visible && !PlayerManager.IsControllerDisconnected(this.currentPlayer, true))
        {
            FrameDelayedCallback(new Action(base.Dismiss), 2);
        }
    }

    private void OnDestroy()
    {
        ControllerDisconnectedPrompt.Instance = null;
    }

    public Coroutine FrameDelayedCallback(Action callback, int frames)
    {
        return StartCoroutine(this.frameDelayedCallback_cr(callback, frames));
    }

    public IEnumerator frameDelayedCallback_cr(Action callback, int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
        if (callback != null)
        {
            callback();
        }
        yield break;
    }
}

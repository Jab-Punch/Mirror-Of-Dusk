using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartLogoScreen : MonoBehaviour
{

    [SerializeField] private ActiveStillObject[] JabPunchSplash;
    [SerializeField] private SpriteRenderer fader;

    private int _pieceCount = 0;

    private delegate void LogoPieceCompleteHandler();

    public enum State
    {
        Animating,
        Complete,
        Fading,
        Title
    }

    public StartLogoScreen.State state { get; private set; }

    private void Awake()
    {
        useGUILayout = false;
        Debug.Log("Build version " + Application.version);
        MirrorOfDusk.Init(false);
        //MirrorOfDuskTime.Reset();
        PauseManager.Reset();
        PlayerData.inGame = false;
        PlayerManager.ResetPlayers();
    }

    // Start is called before the first frame update
    void Start()
    {
        SettingsData.ApplySettingsOnStartup();
        base.StartCoroutine(this.loop_cr());
    }

    // Update is called once per frame
    void Update()
    {
        StartLogoScreen.State state = this.state;
        if (state == StartLogoScreen.State.Complete)
        {
            this.state = State.Fading;
            base.StartCoroutine(this.tweenRenderer_cr(this.fader, 0.5f, true));
        } else if (state == StartLogoScreen.State.Title)
        {
            StartCoroutine(this.sceneManager_cr());
        }
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
    private IEnumerator loop_cr()
    {
        yield return new WaitForSeconds(1f);
        base.StartCoroutine(this.tweenRenderer_cr(this.fader, 1f, false));
        yield return null;
        for (int i = 0; i < JabPunchSplash.Length; i++)
        {
            JabPunchSplash[i].updateTweenEnd = new TweeningObject.TweenEndCompleteHandler(AddFinishedPiece);
            JabPunchSplash[i].ForceStart();
        }
        yield return null;

        //To Do: Add content
        yield break;
    }

    private IEnumerator tweenRenderer_cr(SpriteRenderer renderer, float time, bool fade)
    {
        if (this.state == State.Fading)
        {
            yield return new WaitForSeconds(2.5f);
        }
        float t = 0f;
        Color c = renderer.color;
        c.a = (fade) ? 0f : 1f;
        yield return null;
        while (t < time)
        {
            if (fade)
            {
                c.a = 0f + t / time;
            } else
            {
                c.a = 1f - t / time;
            }
            renderer.color = c;
            t += Time.deltaTime;
            yield return null;
        }
        c.a = (fade) ? 1f : 0f;
        renderer.color = c;
        if (this.state == State.Fading)
        {
            this.state = State.Title;
        }
        yield return null;
        yield break;
    }

    public void AddFinishedPiece()
    {
        _pieceCount++;
        if (_pieceCount >= JabPunchSplash.Length)
        {
            this.state = State.Complete;
        }
    }

    private IEnumerator sceneManager_cr()
    {
        yield return new WaitForSeconds(1f);
        SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.Shard);
        //SceneManager.LoadScene("scene_title");
        yield break;
    }
}

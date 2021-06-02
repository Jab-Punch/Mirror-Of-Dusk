using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AbstractPauseGUI : MonoBehaviour
{
    [SerializeField] private bool isCharacterSelectScreen;
    
    protected CanvasGroup canvasGroup;

    private MirrorOfDuskInput.AnyPlayerInput input;

    public enum State
    {
        Unpaused,
        Paused,
        Animating
    }

    public enum InputActionSet
    {
        StageInput,
        UIInput
    }

    private delegate void AnimationDelegate(float i);

    public AbstractPauseGUI.State state { get; protected set; }

    protected virtual MirrorOfDuskButton StageInputButton
    {
        get { return MirrorOfDuskButton.Pause; }
    }

    protected virtual MirrorOfDuskButton UIInputButton
    {
        get { return MirrorOfDuskButton.Accept; }
    }

    protected virtual AbstractPauseGUI.InputActionSet CheckedActionSet
    {
        get { return AbstractPauseGUI.InputActionSet.StageInput; }
    }

    protected abstract bool CanPause { get; }
    protected virtual bool CanUnpause { get { return false; } }
    protected virtual bool RespondToDeadPlayer { get { return false; } }

    protected void Awake()
    {
        base.useGUILayout = false;
        this.canvasGroup = base.GetComponent<CanvasGroup>();
        this.HideImmediate();
    }

    protected virtual void Update()
    {
        this.UpdateInput();
    }

    private void UpdateInput()
    {
        if (!this.CanPause)
        {
            return;
        }
        bool flag = (this.CheckedActionSet != AbstractPauseGUI.InputActionSet.StageInput) ? this.GetButtonDown(this.UIInputButton) : this.GetButtonDown(this.StageInputButton);
        if (flag)
        {
            base.StartCoroutine(this.ShowPauseMenu());
        }
    }

    public IEnumerator ShowPauseMenu()
    {
        /*if (MapEventNotification.Current != null)
        {
            while (MapEventNotification.Current.showing)
            {
                yield return null;
            }
        }*/
        if (this.state == AbstractPauseGUI.State.Unpaused && PauseManager.state == PauseManager.State.Unpaused)
        {
            this.Pause();
        }
        else if (this.state == AbstractPauseGUI.State.Paused && this.CanUnpause)
        {
            this.Unpause();
        }
        yield break;
    }

    /*public virtual void Init(bool checkIfDead, OptionsGUI options, AchievementsGUI achievements)
    {
        this.input = new MirrorOfDuskInput.AnyPlayerInput(checkIfDead);
    }*/

    public virtual void Init(bool checkIfKOed)
    {
        this.input = new MirrorOfDuskInput.AnyPlayerInput(checkIfKOed);
    }

    protected void Pause()
    {
        if (this.state == AbstractPauseGUI.State.Unpaused && PauseManager.state == PauseManager.State.Unpaused)
        {
            base.StartCoroutine(this.pause_cr());
        }
    }

    protected void Unpause()
    {
        if (this.state == AbstractPauseGUI.State.Paused)
        {
            base.StartCoroutine(this.unpause_cr());
        }
    }

    protected virtual void OnPause()
    {
        AudioManager.HandleSnapshot(AudioManager.Snapshots.Paused.ToString(), 0.15f);
        AudioManager.PauseAllSFX();
        if (PlatformHelper.GarbageCollectOnPause)
        {
            GC.Collect();
        }
    }

    protected virtual void OnPauseComplete()
    {
    }

    protected virtual void OnUnpause()
    {
        if (PlatformHelper.GarbageCollectOnPause)
        {
            GC.Collect();
        }
        //AudioManager.SnapshotReset((!this.isCharacterSelectScreen) ? Stage.Current.CurrentScene.ToString() : PlayerData.Data.CurrentMap.ToString(), 0.1f);
        this.OnUnpauseSound();
    }

    protected virtual void OnUnpauseComplete()
    {
    }

    protected virtual void OnUnpauseSound()
    {
        AudioManager.UnpauseAllSFX();
    }

    protected virtual void HideImmediate()
    {
        this.canvasGroup.alpha = 0f;
        this.SetInteractable(false);
    }

    protected virtual void ShowImmediate()
    {
        this.canvasGroup.alpha = 1f;
        this.SetInteractable(true);
    }

    private void SetInteractable(bool interactable)
    {
        this.canvasGroup.interactable = interactable;
        this.canvasGroup.blocksRaycasts = interactable;
    }

    private IEnumerator pause_cr()
    {
        //Vibrator.StopVibrating(PlayerId.PlayerOne);
        //Vibrator.StopVibrating(PlayerId.PlayerTwo);
        this.OnPause();
        PauseManager.Pause();
        this.SetInteractable(true);
        yield return base.StartCoroutine(this.animate_cr(this.InTime, new AbstractPauseGUI.AnimationDelegate(this.InAnimation), 0f, 1f));
        this.state = AbstractPauseGUI.State.Paused;
        this.OnPauseComplete();
        yield break;
    }

    private IEnumerator unpause_cr()
    {
        this.OnUnpause();
        this.SetInteractable(true);
        PauseManager.Unpause();
        yield return base.StartCoroutine(this.animate_cr(this.OutTime, new AbstractPauseGUI.AnimationDelegate(this.OutAnimation), 1f, 0f));
        this.state = AbstractPauseGUI.State.Unpaused;
        this.SetInteractable(false);
        this.OnUnpauseComplete();
        yield break;
    }

    private IEnumerator animate_cr(float time, AbstractPauseGUI.AnimationDelegate anim, float start, float end)
    {
        anim(0f);
        this.state = AbstractPauseGUI.State.Animating;
        this.canvasGroup.alpha = start;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.canvasGroup.alpha = Mathf.Lerp(start, end, val);
            anim(val);
            t += Time.deltaTime;
            yield return null;
        }
        this.canvasGroup.alpha = end;
        anim(1f);
        yield break;
    }
    
    protected abstract void InAnimation(float i);
    protected abstract void OutAnimation(float i);

    protected virtual float InTime
    {
        get { return 0.15f; }
    }

    protected virtual float OutTime
    {
        get { return 0.15f; }
    }

    protected bool GetButtonDown(MirrorOfDuskButton button)
    {
        /*if (MapEquipUI.Current != null && MapEquipUI.Current.CurrentState == MapEquipUI.ActiveState.Active && button == CupheadButton.EquipMenu)
        {
            return false;
        }*/
        if (this.input.GetButtonDown(button))
        {
            this.MenuSelectSound();
            return true;
        }
        return false;
    }

    protected void MenuSelectSound()
    {
        AudioManager.Play("stage_menu_select");
    }

    protected bool GetButton(MirrorOfDuskButton button)
    {
        return this.input.GetButton(button);
    }
}

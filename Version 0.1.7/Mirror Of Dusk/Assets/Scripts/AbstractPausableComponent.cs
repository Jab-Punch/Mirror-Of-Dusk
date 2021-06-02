using System;
using System.Collections;
using UnityEngine;

public class AbstractPausableComponent : AbstractMB
{
    [NonSerialized] public bool preEnabled;

    protected SoundEmitter emitAudioFromObject;

    protected virtual Transform emitTransform
    {
        get
        {
            return base.transform;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        PauseManager.AddChild(this);
        this.preEnabled = base.enabled;
        this.emitAudioFromObject = new SoundEmitter(this);
    }

    protected virtual void OnDestroy()
    {
        PauseManager.RemoveChild(this);
    }

    public virtual void OnPause() { }

    public virtual void OnUnpause() { }

    protected IEnumerator WaitForPause_CR()
    {
        while (PauseManager.state == PauseManager.State.Paused)
        {
            yield return null;
        }
        yield break;
    }

    public virtual void OnStageEnd()
    {
        if (this != null)
        {
            this.StopAllCoroutines();
            base.enabled = false;
        }
    }

    public void EmitSound(string key)
    {
        AudioManager.FollowObject(key, this.emitTransform);
    }
}

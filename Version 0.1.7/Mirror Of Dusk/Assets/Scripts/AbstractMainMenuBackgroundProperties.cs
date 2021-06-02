using System;
using System.Diagnostics;
using UnityEngine;

public abstract class AbstractMainMenuBackgroundProperties<STATE, STATE_NAMES> where STATE : AbstractMainMenuBackgroundState<STATE_NAMES>
{
    public readonly float movingTrigger;
    private readonly STATE[] states;
    private int stateIndex;

    public AbstractMainMenuBackgroundProperties(float mTrigger, STATE[] states)
    {
        this.movingTrigger = mTrigger;
        this.MoveTrigger = this.movingTrigger;
        this.states = states;
        this.stateIndex = 0;
    }

    public float MoveTrigger { get; private set; }

    public STATE CurrentState
    {
        get
        {
            this.stateIndex = Mathf.Clamp(this.stateIndex, 0, this.states.Length - 1);
            return this.states[this.stateIndex];
        }
    }
}

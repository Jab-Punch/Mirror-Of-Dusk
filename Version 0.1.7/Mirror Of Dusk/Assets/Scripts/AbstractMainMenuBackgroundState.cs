using System;
using UnityEngine;

public abstract class AbstractMainMenuBackgroundState<STATE_NAMES>
{
    public readonly float moveTrigger;
    public readonly STATE_NAMES stateName;

    public AbstractMainMenuBackgroundState(float moveTrigger, STATE_NAMES stateName)
    {
        this.moveTrigger = Mathf.Clamp(moveTrigger, -1f, 1f);
        this.stateName = stateName;
    }
}

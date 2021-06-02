using System;
using System.Collections.Generic;
using UnityEngine;

public static class PauseManager
{
    public enum State
    {
        Unpaused,
        Paused
    }

    public static PauseManager.State state;

    private static float oldSpeed;
    private static List<AbstractPausableComponent> children = new List<AbstractPausableComponent>();

    public static void Reset()
    {
        PauseManager.state = PauseManager.State.Unpaused;
    }

    public static void AddChild(AbstractPausableComponent child)
    {
        PauseManager.children.Add(child);
    }

    public static void RemoveChild(AbstractPausableComponent child)
    {
        PauseManager.children.Remove(child);
    }

    public static void Pause()
    {
        if (PauseManager.state == PauseManager.State.Paused)
        {
            return;
        }
        PauseManager.state = PauseManager.State.Paused;
        AudioListener.pause = true;
        //PauseManager.oldSpeed = MirrorOfDuskTime.GlobalSpeed;
        //MirrorOfDuskTime.GlobalSpeed = 0f;
        for (int i = 0; i < PauseManager.children.Count; i++)
        {
            PauseManager.children[i].OnPause();
        }
        /*foreach (AbstractPausableComponent abstractPausableComponent in PauseManager.children)
        {
            abstractPausableComponent.OnPause();
        }*/
        PauseManager.SetChildren(false);
    }

    public static void Unpause()
    {
        if (PauseManager.state == PauseManager.State.Unpaused)
        {
            return;
        }
        PauseManager.state = PauseManager.State.Unpaused;
        AudioListener.pause = false;
        //MirrorOfDuskTime.GlobalSpeed = PauseManager.oldSpeed;
        for (int i = 0; i < PauseManager.children.Count; i++)
        {
            PauseManager.children[i].OnUnpause();
        }
        /*foreach (AbstractPausableComponent abstractPausableComponent in PauseManager.children)
        {
            abstractPausableComponent.OnUnpause();
        }*/
        PauseManager.SetChildren(true);
    }

    public static void Toggle()
    {
        if (PauseManager.state == PauseManager.State.Paused)
        {
            PauseManager.Unpause();
        } else
        {
            PauseManager.Pause();
        }
    }

    private static void SetChildren(bool enabled)
    {
        for (int i = 0; i < PauseManager.children.Count; i++)
        {
            AbstractPausableComponent abstractPausableComponent = PauseManager.children[i];
            if (abstractPausableComponent == null)
            {
                PauseManager.children.Remove(abstractPausableComponent);
                i--;
            }
            else if (enabled)
            {
                abstractPausableComponent.enabled = abstractPausableComponent.preEnabled;
            } else
            {
                abstractPausableComponent.preEnabled = abstractPausableComponent.enabled;
                abstractPausableComponent.enabled = false;
            }
        }
    }
}

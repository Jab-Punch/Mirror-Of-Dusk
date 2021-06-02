using System;
using UnityEngine;

public class InterruptingPrompt : MonoBehaviour
{
    private bool wasPausedBeforeInterrupt;

    public static void SetCanInterrupt(bool canInterrupt)
    {
        if (ControllerDisconnectedPrompt.Instance != null)
        {
            ControllerDisconnectedPrompt.Instance.allowedToShow = canInterrupt;
        }
    }

    public static bool IsInterrupting()
    {
        return ControllerDisconnectedPrompt.Instance != null && ControllerDisconnectedPrompt.Instance.Visible;
    }

    public static bool CanInterrupt()
    {
        return ControllerDisconnectedPrompt.Instance != null && ControllerDisconnectedPrompt.Instance.allowedToShow;
    }

    public bool Visible
    {
        get { return base.gameObject.activeSelf; }
    }

    protected virtual void Awake()
    {
        base.useGUILayout = false;
        base.gameObject.SetActive(false);
    }

    public void Show()
    {
        base.gameObject.SetActive(true);
        this.wasPausedBeforeInterrupt = (PauseManager.state == PauseManager.State.Paused);
        if (!this.wasPausedBeforeInterrupt)
        {
            PauseManager.Pause();
        }
    }

    public void Dismiss()
    {
        base.gameObject.SetActive(false);
        if (!this.wasPausedBeforeInterrupt)
        {
            PauseManager.Unpause();
        }
    }
}

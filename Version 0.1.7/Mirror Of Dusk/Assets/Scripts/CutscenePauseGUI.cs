using System;
using System.Diagnostics;


public class CutscenePauseGUI : AbstractPauseGUI
{
    public bool pauseAllowed = true;

    public static event Action OnPauseEvent;
    public static event Action OnUnpauseEvent;

    protected override bool CanPause
    {
        get { return PauseManager.state != PauseManager.State.Paused && this.pauseAllowed; }
    }

    protected override void OnPause()
    {
        base.OnPause();
        //MirrorOfDuskCutsceneCamera.Current.StartBlur();
        if (CutscenePauseGUI.OnPauseEvent != null)
        {
            CutscenePauseGUI.OnPauseEvent();
        }
    }
    
    protected override void OnUnpause()
    {
        base.OnUnpause();
        //MirrorOfDuskCutsceneCamera.Current.EndBlur();
        if (CutscenePauseGUI.OnUnpauseEvent != null)
        {
            CutscenePauseGUI.OnUnpauseEvent();
        }
    }

    private void OnDestroy() { PauseManager.Unpause(); }

    protected override void Update()
    {
        base.Update();
        if (base.state != AbstractPauseGUI.State.Paused)
        {
            return;
        }
        if (base.GetButtonDown(MirrorOfDuskButton.Pause))
        {
            base.Unpause();
            return;
        }
        if (base.GetButtonDown(MirrorOfDuskButton.Cancel))
        {
            base.Unpause();
            return;
        }
        if (base.GetButtonDown(MirrorOfDuskButton.Accept))
        {
            //Cutscene.Current.Skip();
            return;
        }
    }

    private void Restart()
    {
        base.state = AbstractPauseGUI.State.Animating;
        //SceneLoader.ReloadLevel();
    }

    private void StartNewGame()
    {
        base.state = AbstractPauseGUI.State.Animating;
        //PlayerManager.ResetPlayers();
        SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass);
    }

    protected override void InAnimation(float i) { }
    protected override void OutAnimation(float i) { }
}

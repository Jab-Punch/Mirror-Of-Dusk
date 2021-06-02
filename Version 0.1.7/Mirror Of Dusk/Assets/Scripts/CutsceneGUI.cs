using System;
using UnityEngine;

public class CutsceneGUI : MonoBehaviour
{
    public const string PATH = "UI/Cutscene_UI";

    [SerializeField] private Canvas canvas;
    [SerializeField] public CutscenePauseGUI pause;
    
    [Space(10f)]
    [SerializeField]
    private MirrorOfDuskUICamera uiCameraPrefab;
    
    private MirrorOfDuskUICamera uiCamera;

    public static CutsceneGUI Current { get; private set; }

    public Canvas Canvas
    {
        get { return this.canvas; }
    }

    private void Awake()
    {
        base.useGUILayout = false;
        CutsceneGUI.Current = this;
    }

    void Start()
    {
        this.uiCamera = UnityEngine.Object.Instantiate<MirrorOfDuskUICamera>(this.uiCameraPrefab);
        this.uiCamera.transform.SetParent(base.transform);
        this.uiCamera.transform.localPosition = Vector3.zero;
        this.uiCamera.transform.localEulerAngles = Vector3.zero;
        this.uiCamera.transform.localScale = Vector3.one;
        this.canvas.worldCamera = this.uiCamera.camera;
    }

    private void OnDestroy()
    {
        if (CutsceneGUI.Current == this)
        {
            CutsceneGUI.Current = null;
        }
    }

    public void CutseneInit()
    {
        this.pause.Init(false);
    }

    protected virtual void CutsceneSnapshot()
    {
        AudioManager.HandleSnapshot(AudioManager.Snapshots.Cutscene.ToString(), 0.15f);
    }
}

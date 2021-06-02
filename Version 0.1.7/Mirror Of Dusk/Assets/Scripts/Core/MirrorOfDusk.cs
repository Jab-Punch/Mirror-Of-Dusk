using System;
using Rewired;
using Rewired.UI.ControlMapper;
using UnityEngine;

public class MirrorOfDusk : MonoBehaviour
{
    private const string PATH = "Core/MirrorOfDuskCore";
    private static bool didLightInit;
    private static bool didFullInit;

    [SerializeField] private AudioNoiseHandler noiseHandler;
    [SerializeField] private Rewired.InputManager rewired;
    public ControlMapper controlMapper;
    [SerializeField] private MirrorOfDuskEventSystem eventSystem;

    public static MirrorOfDusk Current {get; private set;}

    public static void Init(bool lightInit = false)
    {
        if (MirrorOfDusk.Current == null)
        {
            UnityEngine.Object.Instantiate<MirrorOfDusk>(Resources.Load<MirrorOfDusk>("Core/MirrorOfDuskCore"));
        } else
        {
            if (!MirrorOfDusk.didLightInit)
            {
                return;
            }
            MirrorOfDusk.didLightInit = false;
        }
        if (lightInit)
        {
            MirrorOfDusk.didLightInit = true;
        } else
        {
            MirrorOfDusk.Current.rewired.gameObject.SetActive(true);
            MirrorOfDusk.Current.eventSystem.gameObject.SetActive(true);
            MirrorOfDusk.Current.controlMapper.gameObject.SetActive(true);
            PlayerManager.Awake();
            UserConfigDataManager.Awake();
            if (!PlatformHelper.PreloadSettingsData)
            {
                OnlineManager.Instance.Init();
            }
            PlmManager.Instance.Init();
            PlayerManager.Init();
            ExperimentHitboxManager.Awake();
            MirrorOfDusk.didFullInit = true;
        }
    }

    private void Awake()
    {
        base.useGUILayout = false;
        if (MirrorOfDusk.Current == null)
        {
            MirrorOfDusk.Current = this;
            base.gameObject.name = base.gameObject.name.Replace("(Clone)", string.Empty);
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            this.noiseHandler = UnityEngine.Object.Instantiate<AudioNoiseHandler>(this.noiseHandler);
            this.noiseHandler.transform.SetParent(base.transform);
            bool hasBootedUpGame = SettingsData.Data.hasBootedUpGame;
            return;
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }

    private void OnDestroy()
    {
        if (MirrorOfDusk.Current == this)
        {
            MirrorOfDusk.Current = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MirrorOfDusk.didFullInit)
        {
            PlayerManager.Update();
        }
        Cursor.visible = !Screen.fullScreen;
    }

    public Rewired.InputManager inptm
    {
        get { return this.rewired; }
    }
}

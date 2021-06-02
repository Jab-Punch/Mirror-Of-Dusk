using System;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplayHandler : AbstractMB
{
    private static FPSDisplayHandler _instance;

    [SerializeField] private FPSDisplayCamera cameraPrefab;
    private FPSDisplayCamera fpsDisplayCamera;
    
    [SerializeField] private Canvas canvas;

    private float timer;
    private float refresh;
    private float avgFrameRate;
    private string display = "{0} FPS";
    [SerializeField] private Text fpsDisplayText;
    [SerializeField] private Text fpsDisplayTextShadow;

    public static bool Exists
    {
        get
        {
            return FPSDisplayHandler._instance != null;
        }
    }

    public static FPSDisplayHandler Instance
    {
        get
        {
            if (FPSDisplayHandler._instance == null)
            {
                FPSDisplayHandler._instance = (UnityEngine.Object.Instantiate(Resources.Load("UI/FPSDisplayHandler")) as GameObject).GetComponent<FPSDisplayHandler>();
            }
            return FPSDisplayHandler._instance;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        FPSDisplayHandler._instance = this;
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }

    public void Setup()
    {
        this.fpsDisplayCamera = UnityEngine.Object.Instantiate<FPSDisplayCamera>(this.cameraPrefab);
        this.fpsDisplayCamera.transform.SetParent(base.transform);
        //this.fpsDisplayCamera.transform.ResetLocalTransforms();
        if (!SettingsData.Data.fpsDisplay)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;
        if (timer <= 0) avgFrameRate = (int)(1f / timelapse);
        fpsDisplayText.text = string.Format(display, avgFrameRate.ToString());
        fpsDisplayTextShadow.text = string.Format(display, avgFrameRate.ToString());
    }
}

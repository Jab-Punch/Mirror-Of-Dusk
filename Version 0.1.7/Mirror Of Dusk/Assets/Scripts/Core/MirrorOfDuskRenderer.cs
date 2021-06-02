using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MirrorOfDuskRenderer : MonoBehaviour
{
    public enum RenderLayer
    {
        None,
        Game,
        UI,
        Loader
    }

    public static MirrorOfDuskRenderer Instance;

    [SerializeField] private MirrorOfDuskRendererCamera cameraPrefab;
    [SerializeField] private MirrorOfDuskRendererCamera rendererCamera;
    private Camera bgCamera;
    private Canvas canvas;
    private Dictionary<MirrorOfDuskRenderer.RenderLayer, RectTransform> rendererParents;

    private Image background;
    private Image fader;
    public bool darknessEffectPlaying;

    public MirrorOfDuskRendererCamera R_Camera
    {
        get
        {
            return rendererCamera;
        }
    }

    private void Awake()
    {
        base.useGUILayout = false;
        if (MirrorOfDuskRenderer.Instance == null)
        {
            MirrorOfDuskRenderer.Instance = this;
            this.Setup();
            return;
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }

    private void OnDestroy()
    {
        if (MirrorOfDuskRenderer.Instance == this)
        {
            MirrorOfDuskRenderer.Instance = null;
        }
    }

    private void Setup()
    {
        this.rendererCamera = UnityEngine.Object.Instantiate<MirrorOfDuskRendererCamera>(this.cameraPrefab);
        this.rendererCamera.transform.SetParent(base.transform);
        this.rendererCamera.transform.localPosition = Vector3.zero;
        this.rendererCamera.transform.localEulerAngles = Vector3.zero;
        this.rendererCamera.transform.localScale = Vector3.one;
    }
}

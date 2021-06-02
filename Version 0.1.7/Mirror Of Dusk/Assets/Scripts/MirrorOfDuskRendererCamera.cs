using System;
using System.Collections;
using UnityEngine;

public class MirrorOfDuskRendererCamera : MonoBehaviour
{
    private Camera rendCamera;

    public class PerStillCameraBuffer
    {
        public int _InvCameraViewProj;
        public int _ScaledScreenParams;
    }

    public PerStillCameraBuffer perStillCameraBuffer;

    [NonSerialized] public float cameraWidth;
    [NonSerialized] public float cameraHeight;

    private void Awake()
    {
        base.useGUILayout = false;
        perStillCameraBuffer = new PerStillCameraBuffer();
    }

    private void Start()
    {
        this.SetCamera();
    }

    private void SetCamera()
    {
        this.rendCamera = this.gameObject.GetComponent<Camera>();
        this.perStillCameraBuffer._InvCameraViewProj = Shader.PropertyToID("_InvCameraViewProj");
        this.perStillCameraBuffer._ScaledScreenParams = Shader.PropertyToID("_ScaledScreenParams");
        this.cameraWidth = (float)rendCamera.pixelWidth * 1f;
        this.cameraHeight = (float)rendCamera.pixelHeight * 1f;
    }
}

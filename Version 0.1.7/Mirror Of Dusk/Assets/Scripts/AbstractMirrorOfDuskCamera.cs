using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class AbstractMirrorOfDuskCamera : AbstractMB
{
    private Camera _camera;

    public Camera camera
    {
        get
        {
            if (this._camera == null)
            {
                this._camera = base.GetComponent<Camera>();
            }
            return this._camera;
        }
    }

    public bool ContainsPoint(Vector2 point)
    {
        return this.ContainsPoint(point, Vector2.zero);
    }

    public bool ContainsPoint(Vector2 point, Vector2 padding)
    {
        float width = this.camera.orthographicSize * 1.7777778f * 2f + padding.x * 2f;
        float height = this.camera.orthographicSize * 2f * padding.y * 2f;
        return RectUtils.NewFromCenter(base.transform.position.x, base.transform.position.y, width, height).Contains(point);
    }

    protected override void Awake()
    {
        base.Awake();
        this.camera.clearFlags = CameraClearFlags.Nothing;
    }

    public Rect Bounds
    {
        get
        {
            float width = this.camera.orthographicSize * 1.7777778f * 2f;
            float height = this.camera.orthographicSize * 2f;
            return RectUtils.NewFromCenter(base.transform.position.x, base.transform.position.y, width, height);
        }
    }

    protected virtual void LateUpdate()
    {
        this.UpdateRect();
    }

    public void UpdateRect()
    {
        float num = (float)Screen.width / (float)Screen.height;
        float num2 = 1f - 0.1f * 0f;
        Rect rect;
        if (num > 1.7777778f)
        {
            rect = RectUtils.NewFromCenter(0.5f, 0.5f, num2 * 1.7777778f / num, num2 * 1f);
        }
        else
        {
            rect = RectUtils.NewFromCenter(0.5f, 0.5f, num2 * 1f, num2 * num / 1.7777778f);
        }
        if (this.camera.rect != rect)
        {
            this.camera.rect = rect;
            CanvasScaler[] array = UnityEngine.Object.FindObjectsOfType<CanvasScaler>();
            foreach (CanvasScaler canvasScaler in array)
            {
                canvasScaler.referenceResolution = new Vector2(1920f / rect.height, 1080f / rect.height);
            }
        }
    }
}

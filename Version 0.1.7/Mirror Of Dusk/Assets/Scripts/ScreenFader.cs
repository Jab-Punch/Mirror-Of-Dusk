using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{

    [SerializeField] public float fadeSpeed = 1f;

    #region FIELDS
    public RawImage RUIImage;
    public bool openFade = true;
    public enum FadeDirection
    {
        In, //Alpha = 1
        Out // Alpha = 0
    }
    #endregion
    #region MONOBHEAVIOR

    protected virtual void OnEnable()
    {
        if (openFade)
        {
            StartCoroutine(Fade(FadeDirection.Out));
        } else
        {
            float alpha = 0f;
            SetColorImage(ref alpha, FadeDirection.Out);
        }
    }

    #endregion

    #region FADE
    private IEnumerator Fade(FadeDirection fadeDirection)
    {
        float alpha = (fadeDirection == FadeDirection.Out) ? 1 : 0;
        float fadeEndValue = (fadeDirection == FadeDirection.Out) ? 0 : 1;
        if (fadeDirection == FadeDirection.Out)
        {
            while (alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
            RUIImage.enabled = false;
        }
        else
        {
            RUIImage.enabled = true;
            while (alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
            SetColorImage(ref alpha, fadeDirection);
            yield return null;
        }
    }
    #endregion
    #region HELPERS
    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection)
    {
        yield return Fade(fadeDirection);
    }
    private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        RUIImage = GetComponent<RawImage>();
        RUIImage.color = new Color(RUIImage.color.r, RUIImage.color.g, RUIImage.color.b, alpha);
        alpha += Time.deltaTime * (1.0f / fadeSpeed) * ((fadeDirection == FadeDirection.Out) ? -1 : 1);
    }
    #endregion

    // Use this for initialization
    protected virtual void Start()
    {
        if (openFade)
        {
            StartCoroutine(FadeAndLoadScene(ScreenFader.FadeDirection.Out));
        } else
        {
            float alpha = 0f;
            SetColorImage(ref alpha, FadeDirection.Out);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void moveFadeLayer(int layer)
    {
        gameObject.transform.parent.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(gameObject.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.x, gameObject.transform.parent.gameObject.GetComponent<RectTransform>().localPosition.y, layer);
    }
}

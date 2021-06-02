using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashText : MonoBehaviour
{

    public bool flashOn = true;
    [SerializeField] public float flashOnTime = 5f;
    [SerializeField] public float flashOffTime = 5f;

    Text cText;
    float timer;
    float alpha;

    // Use this for initialization
    void Start()
    {
        cText = GetComponent<Text>();
        timer = flashOffTime;
        alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (flashOn)
        {
            timer -= 1f;

            if (timer <= (0f))
            {
                if (alpha == 0f)
                {
                    alpha = 1f;
                    timer = flashOnTime;
                }
                else
                {
                    alpha = 0f;
                    timer = flashOffTime;
                }
                cText.color = new Color(cText.color.r, cText.color.g, cText.color.b, alpha);
            }
        } else
        {
            timer = flashOffTime;
            alpha = 1f;
            cText.color = new Color(cText.color.r, cText.color.g, cText.color.b, alpha);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CSReadTitleText : MonoBehaviour
{

    [SerializeField] float fadeSpeed = 25.5f;
    [SerializeField] int readFrame = 200;
    private int frame = 0;
    public bool ttReadOn = false;
    private int currentRead = 0;
    private List<string> ttInfo;
    private List<int> ttFontSize;
    private class TTData
    {
        public int code;
        public int fontSize;
        public string info;
    }
    private List<TTData> ttData;
    TextMeshProUGUI theText;

    private void Awake()
    {
        theText = gameObject.GetComponent<TextMeshProUGUI>();
        ttData = new List<TTData>();
        ttData.Add(new TTData { code = 0, fontSize = 46, info = "TRAINING MODE" });
        ttData.Add(new TTData { code = 1, fontSize = 36, info = "CHOOSE THE DEMENTED SOUL" });
        ttData.Add(new TTData { code = 2, fontSize = 46, info = "TIME - ENDLESS" });
    }

    // Use this for initialization
    void Start()
    {
        ttInfo = new List<string>() { "TRAINING MODE", "CHOOSE THE DEMENTED SOUL", "TIME - ENDLESS" };
        ttFontSize = new List<int>() { 46, 36, 46 };
    }

    // Update is called once per frame
    void Update()
    {
        if (ttReadOn)
        {
            frame++;
            if (frame >= readFrame)
            {
                currentRead++;
                if (currentRead >= ttData.Count)
                {
                    currentRead = 0;
                }
                StartCoroutine("changeTT");
                frame = 0;
            }
        }
    }

    public IEnumerator changeTT()
    {
        float alpha = theText.color.a;
        while (alpha > 0)
        {
            alpha -= fadeSpeed;
            theText.color = new Color(theText.color.r, theText.color.g, theText.color.b, alpha);
            yield return null;
        }
        //theText.text = ttInfo[currentRead];
        //theText.fontSize = ttFontSize[currentRead];
        theText.text = ttData[currentRead].info;
        theText.fontSize = ttData[currentRead].fontSize;
        while (alpha < 1)
        {
            alpha += fadeSpeed;
            theText.color = new Color(theText.color.r, theText.color.g, theText.color.b, alpha);
            yield return null;
        }
        yield return null;
    }

    public void updateTimerInfo(int timeAmount)
    {
        string timeStr = "";
        if (timeAmount == 0)
        {
            timeStr = "ENDLESS";
        } else
        {
            timeStr = timeAmount.ToString() + ":00";
        }
        ttData[2].info = "TIME - " + timeStr;
    }
}

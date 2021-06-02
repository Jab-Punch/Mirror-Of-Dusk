using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuDetailsRoot : MonoBehaviour {

    //SpriteRenderer spr;
    public float scrollSpeed = 5f;

    private TextMeshProUGUI m_detailsTextObject;
    //private TextMeshProUGUI m_cloneTextObject;
    private Canvas m_detailsCanvas;

    private RectTransform m_textRectTransform;
    private bool enableScroll = false;
    private bool textUpdated = false;

	// Use this for initialization
	void Awake () {
        //spr = gameObject.GetComponent<SpriteRenderer>();
        m_detailsTextObject = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        m_textRectTransform = m_detailsTextObject.gameObject.GetComponent<RectTransform>();
        m_detailsCanvas = gameObject.transform.GetComponentInChildren<Canvas>();

        /*m_cloneTextObject = Instantiate(m_detailsTextObject as TextMeshProUGUI);
        RectTransform cloneRectTransform = m_cloneTextObject.GetComponent<RectTransform>();
        cloneRectTransform.SetParent(m_textRectTransform);
        cloneRectTransform.anchorMin = new Vector2(1, 0.5f);
        //cloneRectTransform.anchorMax = new Vector2(2, 0.5f);
        cloneRectTransform.position = new Vector3 (cloneRectTransform.position.x + m_detailsCanvas.GetComponent<RectTransform>().rect.x, m_textRectTransform.position.y, cloneRectTransform.position.z);
        cloneRectTransform.localScale = new Vector3(1, 1, 1);*/
    }

    public void updateDetails(NewMenuScreenRoot.MenuDetailSection mDS)
    {
        StopCoroutine("scrollDetails");
        /*if (m_cloneTextObject != null)
        {
            DestroyImmediate(m_cloneTextObject.gameObject);
        }*/
        m_detailsTextObject.text = mDS.currentDetails;
        /*
        float roundOffset = scrollSpeed - (m_detailsTextObject.preferredWidth % scrollSpeed);
        float spaceTab = 100f;*/
        float canvPosition = m_detailsCanvas.GetComponent<RectTransform>().rect.x;
        m_textRectTransform.position = new Vector3(canvPosition + mDS.pos, m_textRectTransform.position.y, m_textRectTransform.position.z);
        if (m_detailsTextObject.preferredWidth >= 1820)
        {
            enableScroll = true;
        }
        else
        {
            enableScroll = false;
        }
        //Old instantiation for cloning text in a full scroll. Keep for reference.
        /*if (m_detailsTextObject.preferredWidth >= 1180)
        {
            m_cloneTextObject = Instantiate(m_detailsTextObject as TextMeshProUGUI);
            RectTransform cloneRectTransform = m_cloneTextObject.GetComponent<RectTransform>();
            cloneRectTransform.SetParent(m_textRectTransform);
            //cloneRectTransform.anchorMin = new Vector2(1, 0.5f);
            //cloneRectTransform.anchorMax = new Vector2(2, 0.5f);
            cloneRectTransform.position = new Vector3(m_detailsTextObject.GetComponent<RectTransform>().position.x + m_detailsTextObject.preferredWidth + roundOffset + spaceTab, m_textRectTransform.position.y, cloneRectTransform.position.z);
            //cloneRectTransform.position = new Vector3(cloneRectTransform.position.x + m_detailsCanvas.GetComponent<RectTransform>().rect.x + roundOffset + spaceTab + mDS.pos + mDS.pos, m_textRectTransform.position.y, cloneRectTransform.position.z);
            cloneRectTransform.localScale = new Vector3(1, 1, 1);
            enableScroll = true;
        }
        else
        {
            enableScroll = false;
        }*/
        StartCoroutine("scrollDetails", mDS.pos);
        textUpdated = true;
    }

    //Coroutine for scrolling a line of text towards the end and resetting back to the original position
    public IEnumerator scrollDetails(float offset)
    {
        float width = m_detailsTextObject.preferredWidth;
        Vector3 startPosition = m_textRectTransform.position;

        float scrollPosition = 0f;
        float canvPosition = m_detailsCanvas.GetComponent<RectTransform>().rect.x;

        while (enableScroll)
        {
            if (textUpdated)
            {
                width = m_detailsTextObject.preferredWidth;
                textUpdated = false;
            }
            
            bool scrollNow = true;

            yield return new WaitForSeconds(2);

            while (scrollNow)
            {
                m_textRectTransform.position = new Vector3((-scrollPosition) + canvPosition + offset, startPosition.y, startPosition.z);
                scrollPosition += scrollSpeed;
                if ((width - (scrollPosition + scrollSpeed) + offset) < (1920 - offset))
                {
                    scrollNow = false;
                    scrollPosition = 0f;
                    yield return new WaitForSeconds(2);
                    m_textRectTransform.position = new Vector3((-scrollPosition) + canvPosition + offset, startPosition.y, startPosition.z);
                }
                yield return null;
            }
        }
    }

    //Coroutine for fully scrolling a line of text
    /*public IEnumerator fullScrollDetails(float offset)
    {
        float width = m_detailsTextObject.preferredWidth;
        Vector3 startPosition = m_textRectTransform.position;
        float roundOffset = scrollSpeed - (m_detailsTextObject.preferredWidth % scrollSpeed);
        float spaceTab = 100f;

        float scrollPosition = 0f;
        float canvPosition = m_detailsCanvas.GetComponent<RectTransform>().rect.x;

        while (enableScroll)
        {
            if (textUpdated)
            {
                width = m_detailsTextObject.preferredWidth;
                m_cloneTextObject.text = m_detailsTextObject.text;
                textUpdated = false;
            }

            int stillCount = 120;
            bool scrollNow = true;

            while (stillCount > 0)
            {
                stillCount--;
                yield return null;
            }

            while (scrollNow)
            {
                //m_textRectTransform.position = new Vector3(, startPosition.y, startPosition.z);
                //m_textRectTransform.position = new Vector3((-scrollPosition % (width + roundOffset + spaceTab)) + canvPosition + offset, startPosition.y, startPosition.z);
                m_textRectTransform.position = new Vector3((-scrollPosition) + canvPosition + offset, startPosition.y, startPosition.z);
                scrollPosition += scrollSpeed;
                if (scrollPosition >= (width + roundOffset + spaceTab))
                {
                    scrollNow = false;
                    scrollPosition = 0f;
                    m_textRectTransform.position = new Vector3((-scrollPosition) + canvPosition + offset, startPosition.y, startPosition.z);
                }
                yield return null;
            }
        }
    }*/
}

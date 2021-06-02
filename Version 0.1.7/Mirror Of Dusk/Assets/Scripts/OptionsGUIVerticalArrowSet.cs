using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsGUIVerticalArrowSet : MonoBehaviour
{
    [NonSerialized] public State state;

    [Header("Components")]
    [SerializeField] public Image upArrow;
    [SerializeField] public Image downArrow;
    private CanvasGroup _canvasGroup;

    [Header("Sprites")]
    [SerializeField] public Sprite upArrowOn;
    [SerializeField] public Sprite upArrowOff;
    [SerializeField] public Sprite downArrowOn;
    [SerializeField] public Sprite downArrowOff;

    private RectTransform upArrowPos;
    private RectTransform downArrowPos;
    private Vector2 initialUpArrowPos;
    private Vector2 initialDownArrowPos;

    public CanvasGroup canvasGroup
    {
        get {
            if (this._canvasGroup == null)
            {
                this._canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
            }
            return this._canvasGroup;
        }
    }

    public enum State
    {
        Inactive,
        Active,
        Min,
        Max
    }

    private void Awake()
    {
        this.upArrowPos = this.upArrow.GetComponent<RectTransform>();
        this.downArrowPos = this.downArrow.GetComponent<RectTransform>();
        this.initialUpArrowPos = this.upArrowPos.transform.localPosition;
        this.initialDownArrowPos = this.downArrowPos.transform.localPosition;
    }

    /*private void OnEnable()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(AnimateUp_cr());
        this.StartCoroutine(AnimateDown_cr());
    }*/

    private void OnDisable()
    {
        this.StopAllCoroutines();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpArrow()
    {
        if (this.state == State.Active || this.state == State.Min)
        {
            this.upArrow.sprite = this.upArrowOn;
            this.StartCoroutine(this.AnimateUp_cr());
        } else
        {
            this.upArrowPos.transform.localPosition = new Vector2(this.upArrowPos.transform.localPosition.x, this.initialUpArrowPos.y);
            this.upArrow.sprite = this.upArrowOff;
        }
    }

    public void SetDownArrow()
    {
        if (this.state == State.Active || this.state == State.Max)
        {
            this.downArrow.sprite = this.downArrowOn;
            this.StartCoroutine(this.AnimateDown_cr());
        }
        else
        {
            this.downArrowPos.transform.localPosition = new Vector2(this.downArrowPos.transform.localPosition.x, this.initialDownArrowPos.y);
            this.downArrow.sprite = this.downArrowOff;
        }
    }

    public IEnumerator AnimateUp_cr()
    {
        float movePos = 0;
        for (; ; )
        {
            while (movePos < 5)
            {
                movePos++;
                this.upArrowPos.transform.localPosition = new Vector2(this.upArrowPos.transform.localPosition.x, this.initialUpArrowPos.y + movePos);
                yield return null;
                yield return null;
            }
            while (movePos > 0)
            {
                movePos--;
                this.upArrowPos.transform.localPosition = new Vector2(this.upArrowPos.transform.localPosition.x, this.initialUpArrowPos.y + movePos);
                yield return null;
                yield return null;
            }
        }
        yield break;
    }

    public IEnumerator AnimateDown_cr()
    {
        float movePos = 0;
        for (; ; )
        {
            while (movePos < 5)
            {
                movePos++;
                this.downArrowPos.transform.localPosition = new Vector2(this.downArrowPos.transform.localPosition.x, this.initialDownArrowPos.y - movePos);
                yield return null;
                yield return null;
            }
            while (movePos > 0)
            {
                movePos--;
                this.downArrowPos.transform.localPosition = new Vector2(this.downArrowPos.transform.localPosition.x, this.initialDownArrowPos.y - movePos);
                yield return null;
                yield return null;
            }
        }
        yield break;
    }
}

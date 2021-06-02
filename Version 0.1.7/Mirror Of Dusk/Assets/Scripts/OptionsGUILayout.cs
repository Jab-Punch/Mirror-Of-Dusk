using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsGUILayout : AbstractMB
{
    [SerializeField] public int slotLimit = -1;
    [SerializeField] public int slotMax = -1;
    private float slotHeight = 60f;
    [NonSerialized] public int currentSlotPos = 0;

    public RectTransform slotLayout;
    public RectTransform textLayout;
    public Image layoutArrowUp;
    public Image layoutArrowDown;
    private Vector3 layoutArrowUpOriginalPos;
    private Vector3 layoutArrowDownOriginalPos;

    private IEnumerator ExecuteSlotLayoutVerticalScroll { get; set; }
    private IEnumerator ExecuteTextLayoutVerticalScroll { get; set; }
    private IEnumerator AnimateLayoutArrowUp { get; set; }
    private IEnumerator AnimateLayoutArrowDown { get; set; }
    private bool instantScroll = false;

    private int CurrentSlotPos
    {
        get
        {
            return this.currentSlotPos;
        }
        set
        {
            this.currentSlotPos = (value + ((slotMax + 1) - slotLimit)) % ((slotMax + 1) - slotLimit);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        slotLayout.localPosition = new Vector3(0f, 0f, 0f);
        textLayout.localPosition = new Vector3(0f, 0f, 0f);
        if (layoutArrowUp != null)
            layoutArrowUpOriginalPos = layoutArrowUp.rectTransform.localPosition;
        if (layoutArrowDown != null)
            layoutArrowDownOriginalPos = layoutArrowDown.rectTransform.localPosition;
    }

    private void OnEnable()
    {
        instantScroll = true;
        OptionsGUI.Current.OnUpdateVerticalScrollEvent += this.OnUpdateVerticalScroll;
        if (layoutArrowUp != null)
            layoutArrowUp.rectTransform.localPosition = layoutArrowUpOriginalPos;
        if (layoutArrowDown != null)
            layoutArrowDown.rectTransform.localPosition = layoutArrowDownOriginalPos;
        UpdateLayoutArrows(false);
    }

    private void OnDisable()
    {
        instantScroll = false;
        OptionsGUI.Current.OnUpdateVerticalScrollEvent -= this.OnUpdateVerticalScroll;
    }

    private void OnUpdateVerticalScroll(int currentVertPos)
    {
        if (currentVertPos >= slotLimit + CurrentSlotPos && slotLimit != -1)
        {
            CurrentSlotPos = (currentVertPos + 1) - slotLimit;
            if (instantScroll)
            {
                slotLayout.localPosition = new Vector3(slotLayout.localPosition.x, 0f + ((float)currentSlotPos * 60f), slotLayout.localPosition.z);
                textLayout.localPosition = new Vector3(textLayout.localPosition.x, 0f + ((float)currentSlotPos * 60f), textLayout.localPosition.z);
            } else
            {
                OptionsGUI.Current.selectingState = OptionsGUI.SelectingState.Busy;
                ExecuteVerticalScroll(slotLayout.localPosition, new Vector3(slotLayout.localPosition.x, 0f + ((float)currentSlotPos * 60f), slotLayout.localPosition.z), 8f, EaseUtils.EaseType.linear, slotLayout, textLayout);
            }
            UpdateLayoutArrows(true);
        } else if (currentVertPos < CurrentSlotPos && slotLimit != -1)
        {
            CurrentSlotPos = currentVertPos;
            if (instantScroll)
            {
                slotLayout.localPosition = new Vector3(slotLayout.localPosition.x, 0f + ((float)currentSlotPos * 60f), slotLayout.localPosition.z);
                textLayout.localPosition = new Vector3(textLayout.localPosition.x, 0f + ((float)currentSlotPos * 60f), textLayout.localPosition.z);
            }
            else
            {
                OptionsGUI.Current.selectingState = OptionsGUI.SelectingState.Busy;
                ExecuteVerticalScroll(slotLayout.localPosition, new Vector3(slotLayout.localPosition.x, 0f + ((float)currentSlotPos * 60f), slotLayout.localPosition.z), 8f, EaseUtils.EaseType.linear, slotLayout, textLayout);
            }
            UpdateLayoutArrows(true);
        }
        instantScroll = false;
    }

    private void UpdateLayoutArrows(bool skipAnim)
    {
        if (layoutArrowDown != null)
        {
            if (slotLimit + currentSlotPos < slotMax)
            {
                layoutArrowDown.enabled = true;   
            }
            else
            {
                layoutArrowDown.enabled = false;
            }
            if (!skipAnim)
            {
                this.AnimateLayoutArrowDown = AnimateLayoutArrow_cr(layoutArrowDown, layoutArrowDownOriginalPos, -1);
                this.StartCoroutine(AnimateLayoutArrowDown);
            }
        }
        if (layoutArrowUp != null)
        {
            if (slotLimit + currentSlotPos > slotLimit)
            {
                layoutArrowUp.enabled = true;
            }
            else
            {
                layoutArrowUp.enabled = false;
            }
            if (!skipAnim)
            {
                this.AnimateLayoutArrowUp = AnimateLayoutArrow_cr(layoutArrowUp, layoutArrowUpOriginalPos, 1);
                this.StartCoroutine(AnimateLayoutArrowUp);
            }
        }
    }

    private void ExecuteVerticalScroll(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, RectTransform layout, RectTransform layout2)
    {
        if (ExecuteSlotLayoutVerticalScroll != null) this.StopCoroutine(ExecuteSlotLayoutVerticalScroll);
        if (ExecuteTextLayoutVerticalScroll != null) this.StopCoroutine(ExecuteTextLayoutVerticalScroll);
        this.ExecuteSlotLayoutVerticalScroll = ExecuteVerticalScroll_cr(start, end, time, ease, layout, false);
        this.ExecuteTextLayoutVerticalScroll = ExecuteVerticalScroll_cr(start, end, time, ease, layout2, true);
        this.StartCoroutine(ExecuteSlotLayoutVerticalScroll);
        this.StartCoroutine(ExecuteTextLayoutVerticalScroll);
    }

    private IEnumerator ExecuteVerticalScroll_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, RectTransform layout, bool setState)
    {
        layout.localPosition = start;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            layout.SetLocalPosition(new float?(0f), new float?(y), new float?(0f));
            t += this.LocalDeltaTime;
            yield return null;
        }
        layout.localPosition = end;
        if (setState)
            OptionsGUI.Current.selectingState = OptionsGUI.SelectingState.Free;
        yield return null;
        yield break;
    }

    private IEnumerator AnimateLayoutArrow_cr(Image arrow, Vector3 originalPos, int aimDir)
    {
        arrow.rectTransform.localPosition = originalPos;
        int moveCount = 8;
        for (; ; )
        {
            moveCount--;
            arrow.rectTransform.localPosition = new Vector3(arrow.rectTransform.localPosition.x, arrow.rectTransform.localPosition.y + aimDir, arrow.rectTransform.localPosition.z);
            if (moveCount <= 0)
            {
                aimDir = aimDir * -1;
                moveCount = 8;
            }
            yield return null;
            yield return null;
        }
        yield break;
    }
}

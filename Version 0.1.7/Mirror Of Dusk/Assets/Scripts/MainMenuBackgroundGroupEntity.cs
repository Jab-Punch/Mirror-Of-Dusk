using System;
using System.Collections;
using UnityEngine;

public class MainMenuBackgroundGroupEntity : MainMenuBackgroundProperties.DefaultGroup.Entity
{
    private MainMenuBackgroundProperties.DefaultGroup.Tree_1 tree_1;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void BackgroundInit(MainMenuBackgroundProperties.DefaultGroup properties, int key)
    {
        base.BackgroundInit(properties, key);
        this.tree_1 = properties.CurrentState.treeCollection[key];
    }

    public void StartTravelForward(EaseUtils.EaseType ease)
    {
        base.StartCoroutine(startTravel_cr(ease));
    }

    public void FinishTravelForward(EaseUtils.EaseType ease)
    {
        base.StartCoroutine(finishTravel_cr(ease));
    }

    public void StartTravelBackward(EaseUtils.EaseType ease)
    {
        base.StartCoroutine(startTravelBack_cr(ease));
    }

    public void FinishTravelBackward(EaseUtils.EaseType ease, MainMenuSections section)
    {
        base.StartCoroutine(finishTravelBack_cr(ease, section));
    }

    public IEnumerator startTravel_cr(EaseUtils.EaseType ease)
    {
        Vector2 pos = (Vector2)base.transform.position + new Vector2(this.tree_1.nextPositionProperties[MainMenuScene.Current.CurrentSection].posX, this.tree_1.nextPositionProperties[MainMenuScene.Current.CurrentSection].posY);
        Vector2 localScale = (Vector2)base.transform.localScale + new Vector2(this.tree_1.nextPositionProperties[MainMenuScene.Current.CurrentSection].sizeX, this.tree_1.nextPositionProperties[MainMenuScene.Current.CurrentSection].sizeY);
        this.TweenPosition(base.transform.position, pos, 60f, ease);
        this.TweenScale(base.transform.localScale, localScale, 60f, ease);
        yield break;
    }

    public IEnumerator finishTravel_cr(EaseUtils.EaseType ease)
    {
        this.transform.SetPosition(this.tree_1.prevPositionProperties[MainMenuScene.Current.CurrentSection].posX, this.tree_1.prevPositionProperties[MainMenuScene.Current.CurrentSection].posY, null);
        this.transform.SetScale(this.tree_1.prevPositionProperties[MainMenuScene.Current.CurrentSection].sizeX, this.tree_1.prevPositionProperties[MainMenuScene.Current.CurrentSection].sizeY, null);
        Vector2 pos = new Vector2(this.tree_1.xPos, this.tree_1.yPos);
        Vector2 localScale = new Vector2(this.tree_1.sizeX, this.tree_1.sizeY);
        this.TweenPosition(base.transform.position, pos, 48f, ease);
        yield return this.TweenScale(base.transform.localScale, localScale, 48f, ease);
        this.SetSortingLayer = "Foreground_2";
        yield break;
    }

    public IEnumerator startTravelBack_cr(EaseUtils.EaseType ease)
    {
        this.SetSortingLayer = "Foreground_3";
        Vector2 pos = new Vector2(this.tree_1.prevPositionProperties[MainMenuScene.Current.PreviousSection].posX, this.tree_1.prevPositionProperties[MainMenuScene.Current.PreviousSection].posY);
        Vector2 localScale = new Vector2(this.tree_1.prevPositionProperties[MainMenuScene.Current.PreviousSection].sizeX, this.tree_1.prevPositionProperties[MainMenuScene.Current.PreviousSection].sizeY);
        this.TweenPosition(base.transform.position, pos, 60f, ease);
        this.TweenScale(base.transform.localScale, localScale, 60f, ease);
        yield break;
    }

    public IEnumerator finishTravelBack_cr(EaseUtils.EaseType ease, MainMenuSections section)
    {
        this.transform.SetPosition(this.tree_1.prevPositionProperties[section].posX, this.tree_1.prevPositionProperties[section].posY, null);
        this.transform.SetScale(this.tree_1.prevPositionProperties[section].sizeX, this.tree_1.prevPositionProperties[section].sizeY, null);
        Vector2 pos = new Vector2(this.tree_1.xPos, this.tree_1.yPos);
        Vector2 localScale = new Vector2(this.tree_1.sizeX, this.tree_1.sizeY);
        this.TweenPosition(base.transform.position, pos, 48f, ease);
        yield return this.TweenScale(base.transform.localScale, localScale, 48f, ease);
        yield break;
    }
}

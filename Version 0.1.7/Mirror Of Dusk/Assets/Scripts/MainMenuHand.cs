using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHand : AbstractMB
{
    [SerializeField] private ActiveStillObject handEnterPath;
    [SerializeField] private ActiveStillObject handTweenPath;
    [SerializeField] private ActiveStillObject handZoomPath;
    [SerializeField] private ActiveStillObject handExitPath;
    [SerializeField] private ActiveStillObject handExitTweenPath;
    [SerializeField] private ActiveStillObject handZoomOutPath;
    public State state { get; set; }

    public enum State
    {
        NotReady,
        Ready,
        Busy
    }

    [Header("Sprites")]
    public SpriteRenderer handSpriteBack;
    public SpriteRenderer handSpriteFront;
    public SpriteRenderer dewSprite;
    public SpriteRenderer innerShardSprite;
    public GameSpriteAnimator gleamSprite;
    public SpriteRenderer shardBackground;
    public SpriteRenderer tintScreen;

    public Sprite defaultBackground;
    public Color defaultTint;
    public List<ShardBackgroundCollection> shardBackgroundCollection;

    public Action SetShardBackgroundEventTarget;
    public Action ClearShardBackgroundEventTarget;
    public Action ZoomInHandEventTarget;
    public Action RestoreShardAndMenuTarget;

    

    [Serializable]
    public class ShardBackgroundCollection
    {
        public MainMenuItemSubType key;
        public Sprite background;
        public Color tint;
    }

    protected override void Awake()
    {
        base.Awake();
        SetShardBackgroundEventTarget = TriggerSetShardBackground;
        ClearShardBackgroundEventTarget = TriggerClearShardBackground;
        ZoomInHandEventTarget = TriggerZoomInHand;
        RestoreShardAndMenuTarget = TriggerRestoreShardAndMenu;
        handEnterPath.updateTweenEnd = new TweeningObject.TweenEndCompleteHandler(SetStateReady);
        handZoomPath.updateTweenEnd = new TweeningObject.TweenEndCompleteHandler(ResetState);
        handExitPath.updateTweenEnd = new TweeningObject.TweenEndCompleteHandler(ResetZoomOut);
    }

    public void TriggerSetShardBackground()
    {
        SetShardBackground(MainMenuScene.Current.CurrentItem.mainMenuItemSubType);
    }

    public void TriggerClearShardBackground()
    {
        ClearShardBackground(MainMenuScene.Current.CurrentItem.mainMenuItemSubType);
    }

    public void TriggerZoomInHand()
    {
        this.handTweenPath.ForceStart();
        this.handZoomPath.ForceStart();
        this.StartCoroutine(ClearDew_cr());
    }

    public void TriggerZoomOutHand()
    {
        this.handExitTweenPath.ForceStart();
        this.handZoomOutPath.ForceStart();
        this.StartCoroutine(RestoreDew_cr());
    }

    public void TriggerEnterHand()
    {
        this.handEnterPath.ForceStart();
    }

    public void TriggerExitHand()
    {
        this.handExitPath.ForceStart();
    }

    public void TriggerRestoreShardAndMenu()
    {
        this.RestoreShardAndMenu();
    }

    public void SetShardBackground(MainMenuItemSubType subType)
    {
        for (int i = 0; i < shardBackgroundCollection.Count; i++)
        {
            if (subType == shardBackgroundCollection[i].key)
            {
                shardBackground.sprite = shardBackgroundCollection[i].background;
                tintScreen.color = shardBackgroundCollection[i].tint;
                return;
            }
        }
        shardBackground.sprite = defaultBackground;
        tintScreen.color = defaultTint;
    }

    public void ClearShardBackground(MainMenuItemSubType subType)
    {
        this.gleamSprite.Play("gleam_end", ZoomInHandEventTarget);
        for (int i = 0; i < shardBackgroundCollection.Count; i++)
        {
            if (subType == shardBackgroundCollection[i].key)
            {
                shardBackground.enabled = false;
                tintScreen.color = shardBackgroundCollection[i].tint;
                return;
            }
        }
        shardBackground.enabled = false;
        tintScreen.color = defaultTint;
    }

    public void RestoreShardAndMenu()
    {
        shardBackground.enabled = true;
        MainMenuItemSubType subType = MainMenuScene.Current.RetrievePreviousSelection;
        MainMenuScene.Current.UpdateMenuItems(true);
        SetStateReady();
        for (int i = 0; i < shardBackgroundCollection.Count; i++)
        {
            if (subType == shardBackgroundCollection[i].key)
            {
                shardBackground.sprite = shardBackgroundCollection[i].background;
                tintScreen.color = shardBackgroundCollection[i].tint;
                return;
            }
        }
        shardBackground.sprite = defaultBackground;
        tintScreen.color = defaultTint;
    }

    public void SetStateReady()
    {
        this.state = MainMenuHand.State.Ready;
    }

    public void ResetState()
    {
        this.transform.SetPosition(577, -981, null);
        this.transform.SetScale(1, 1, 1);
        shardBackground.enabled = true;
        ResetSceneTint();
        this.StopAllCoroutines();
        MainMenuScene.Current.Travel();
        MainMenuScene.Current.dewOne.color = new Color(MainMenuScene.Current.dewOne.color.r, MainMenuScene.Current.dewOne.color.g, MainMenuScene.Current.dewOne.color.b, (76f / 255f));
        MainMenuScene.Current.dewTwo.color = new Color(MainMenuScene.Current.dewTwo.color.r, MainMenuScene.Current.dewTwo.color.g, MainMenuScene.Current.dewTwo.color.b, (128f / 255f));
    }

    public void ResetSceneTint()
    {
        for (int i = 0; i < shardBackgroundCollection.Count; i++)
        {
            if (MainMenuScene.Current.CurrentItem.mainMenuItemSubType == shardBackgroundCollection[i].key)
            {
                MainMenuScene.Current.tintScreen.color = shardBackgroundCollection[i].tint;
                return;
            }
        }
        MainMenuScene.Current.tintScreen.color = this.defaultTint;
    }

    public void ResetZoomOut()
    {
        base.StartCoroutine(resetZoomOut_cr());
    }

    private IEnumerator resetZoomOut_cr()
    {
        yield return new WaitForSeconds(0.3f);
        this.transform.SetPosition(-121f, -41f, null);
        this.transform.SetScale(6f, 6f, 1);
        shardBackground.enabled = false;
        MainMenuScene.Current.tintScreen.color = this.defaultTint;
        MainMenuScene.Current.tintScreen.color = new Color(MainMenuScene.Current.tintScreen.color.r, MainMenuScene.Current.tintScreen.color.g, MainMenuScene.Current.tintScreen.color.b, 0f);
        MainMenuScene.Current.dewOne.color = new Color(MainMenuScene.Current.dewOne.color.r, MainMenuScene.Current.dewOne.color.g, MainMenuScene.Current.dewOne.color.b, 0f);
        MainMenuScene.Current.dewTwo.color = new Color(MainMenuScene.Current.dewTwo.color.r, MainMenuScene.Current.dewTwo.color.g, MainMenuScene.Current.dewTwo.color.b, 0f);
        MainMenuItemSubType subType = MainMenuScene.Current.RetrievePreviousSelection;
        SetShardBackground(subType);
        MainMenuScene.Current.Retreat();
        yield return new WaitForSeconds(0.3f);
        TriggerZoomOutHand();
        yield break;
    }

    public void PlayGleamScroll()
    {
        this.gleamSprite.Play("gleam_start", SetShardBackgroundEventTarget);
    }

    public void PlayGleamSelect()
    {
        this.gleamSprite.Play("gleam_start", ClearShardBackgroundEventTarget, true);
    }

    public IEnumerator ClearDew_cr()
    {
        int time = 16;
        while (time > 0)
        {
            if (time <= 6)
                MainMenuScene.Current.tintScreen.color = new Color(MainMenuScene.Current.tintScreen.color.r, MainMenuScene.Current.tintScreen.color.g, MainMenuScene.Current.tintScreen.color.b, Mathf.Clamp((MainMenuScene.Current.tintScreen.color.a - ((76f / 255f) / 6f)), 0f, 1f));
            MainMenuScene.Current.dewOne.color = new Color(MainMenuScene.Current.dewOne.color.r, MainMenuScene.Current.dewOne.color.g, MainMenuScene.Current.dewOne.color.b, Mathf.Clamp((MainMenuScene.Current.dewOne.color.a - ((76f / 255f) / 16f)), 0f, 1f));
            MainMenuScene.Current.dewTwo.color = new Color(MainMenuScene.Current.dewTwo.color.r, MainMenuScene.Current.dewTwo.color.g, MainMenuScene.Current.dewTwo.color.b, Mathf.Clamp((MainMenuScene.Current.dewTwo.color.a - ((128f / 255f) / 16f)), 0f, 1f));
            time--;
            yield return null;
        }
        yield break;
    }

    public IEnumerator RestoreDew_cr()
    {
        int time = 16;
        while (time > 0)
        {
            if (time > 10)
                MainMenuScene.Current.tintScreen.color = new Color(MainMenuScene.Current.tintScreen.color.r, MainMenuScene.Current.tintScreen.color.g, MainMenuScene.Current.tintScreen.color.b, Mathf.Clamp((MainMenuScene.Current.tintScreen.color.a + ((76f / 255f) / 6f)), 0f, 1f));
            MainMenuScene.Current.dewOne.color = new Color(MainMenuScene.Current.dewOne.color.r, MainMenuScene.Current.dewOne.color.g, MainMenuScene.Current.dewOne.color.b, Mathf.Clamp((MainMenuScene.Current.dewOne.color.a + ((76f / 255f) / 16f)), 0f, 1f));
            MainMenuScene.Current.dewTwo.color = new Color(MainMenuScene.Current.dewTwo.color.r, MainMenuScene.Current.dewTwo.color.g, MainMenuScene.Current.dewTwo.color.b, Mathf.Clamp((MainMenuScene.Current.dewTwo.color.a + ((128f / 255f) / 16f)), 0f, 1f));
            time--;
            yield return null;
        }
        this.gleamSprite.Play("gleam_start", RestoreShardAndMenuTarget);
        yield break;
    }
}

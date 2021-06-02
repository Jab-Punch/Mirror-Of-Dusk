using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharacterSelectStarSlot : AbstractCollidableObject
{
    [SerializeField] public int characterCode;

    [Header("Components")]
    [SerializeField] public GameSpriteAnimator spriteAnimator;
    [SerializeField] public GameSpriteAnimator spriteAnimator2;
    [SerializeField] public GameMaskAnimator maskAnimator;
    [SerializeField] public AudioManagerComponent audioManagerComponent;
    [SerializeField] public SpriteRenderer characterName;

    private IEnumerator co_RevealName;
    private int highlightCount = 0;
    public bool highlighted { get; private set; }
    public State state;
    private HighlightState highlightState;

    public enum State
    {
        Off,
        On
    }

    private enum HighlightState
    {
        Off,
        On
    }

    protected override void Awake()
    {
        base.Awake();
        co_RevealName = revealName_cr();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (CharacterSelectScene.Current.state == CharacterSelectScene.State.Selecting || CharacterSelectScene.Current.state == CharacterSelectScene.State.OptionsBusy || CharacterSelectScene.Current.state == CharacterSelectScene.State.OptionsSelecting)
        {
            if (highlightState == HighlightState.On && highlightCount <= 0)
            {
                highlightState = HighlightState.Off;
                this.SetCSStarAnimation("Out");
            }
            if (highlightCount > 0)
            {
                highlighted = true;
                if (highlightState == HighlightState.Off)
                {
                    this.SetCSStarAnimation("Over");
                }
                highlightState = HighlightState.On;
            }
            else
            {
                highlighted = false;
            }
        }
        highlightCount = 0;
    }

    public void RevealName()
    {
        StartCoroutine(revealName_cr());
    }

    private IEnumerator revealName_cr()
    {
        Color nC = characterName.color;
        float alph = nC.a;
        int wFrame = 5;
        while (wFrame > 0)
        {
            yield return 0f;
            wFrame--;
        }
        while (alph < 1.0f)
        {
            alph += 0.05f;
            if (alph > 1.0f) { alph = 1.0f; }
            characterName.color = new Color(nC.r, nC.g, nC.b, alph);
            yield return 0f;
        }
        co_RevealName = null;
        yield break;
    }

    public void HaltRevealName()
    {
        if (co_RevealName != null) this.StopCoroutine(co_RevealName);
        co_RevealName = null;
        characterName.color = new Color(characterName.color.r, characterName.color.g, characterName.color.b, 1f);
    }

    private void SetCSStarAnimation(string state)
    {
        this.spriteAnimator.Play(state);
        this.spriteAnimator2.Play(state);
        this.maskAnimator.Play(state);
    }

    protected override void OnCollisionCursor(GameObject hit, CollisionPhase phase)
    {
        if (phase == CollisionPhase.Stay)
        {
            CharacterSelectCursorStatsManager stats = hit.GetComponent<CharacterSelectCursorStatsManager>();
            if (stats.Holding != -1)
            {
                stats.playerCursorFoundFlags |= (CharacterSelectCursorStatsManager.CursorFoundFlags)(1 << 1);
                stats.characterSelectedId = this.characterCode;
                highlightCount++;
            }
        } else if (phase == CollisionPhase.Exit)
        {
            //CharacterSelectCursorStatsManager stats = hit.GetComponent<CharacterSelectCursorStatsManager>();
            //stats.characterSelectedId = -1;
        }
    }
}

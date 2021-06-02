using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditShardsGUISlot : MonoBehaviour
{
    [SerializeField] EditShardsGUI rootGUI;
    [SerializeField] EditShardsGUI.PhaseState phaseState;
    [SerializeField] OptionsGUIVerticalArrowSet arrowSet;
    [SerializeField] int digitAmount;
    private int selection
    {
        get {
            switch (phaseState)
            {
                default:
                    return rootGUI.editedDefaultShardsHeld;
                case EditShardsGUI.PhaseState.Strength:
                    return rootGUI.editedDefaultShardStrength;
            }
            
        }
        set
        {
            switch (phaseState)
            {
                default:
                    rootGUI.editedDefaultShardsHeld = value;
                    if (rootGUI.editedDefaultShardsHeld > BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards)
                        rootGUI.editedDefaultShardsHeld = BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards;
                    if (rootGUI.editedDefaultShardsHeld < 0)
                        rootGUI.editedDefaultShardsHeld = 0;
                    break;
                case EditShardsGUI.PhaseState.Strength:
                    rootGUI.editedDefaultShardStrength = value;
                    if (rootGUI.editedDefaultShardStrength > BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards)
                        rootGUI.editedDefaultShardStrength = BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards;
                    if (rootGUI.editedDefaultShardStrength < 0)
                        rootGUI.editedDefaultShardStrength = 0;
                    break;
            }
        }
    }

    public CanvasGroup highlighterCanvasGroup;
    [NonSerialized] public bool holdHighlighter = false;

    public void incrementSelection()
    {
        int dAmount = digitAmount;
        if (this.phaseState == EditShardsGUI.PhaseState.Held)
        {
            if (CharacterSelectScene.Current.totalShardsLeft > 0)
            {
                if (dAmount > CharacterSelectScene.Current.totalShardsLeft)
                {
                    dAmount = CharacterSelectScene.Current.totalShardsLeft;
                }
                AudioManager.Play("menu_scroll");
                this.selection = this.selection + dAmount;
                CharacterSelectScene.Current.UpdateShardsLeft(-dAmount);
                this.UpdateArrows();
            }
        } else if (this.phaseState == EditShardsGUI.PhaseState.Strength)
        {
            if (this.selection < BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards)
            {
                AudioManager.Play("menu_scroll");
                this.selection = this.selection + dAmount;
                this.UpdateArrows();
            }
        }
    }

    public void decrementSelection()
    {
        int dAmount = digitAmount;
        if (this.phaseState == EditShardsGUI.PhaseState.Held)
        {
            if (this.selection <= 0)
                return;
            if (CharacterSelectScene.Current.totalShardsLeft < BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards)
            {
                if (dAmount > (BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards - CharacterSelectScene.Current.totalShardsLeft))
                {
                    dAmount = (BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards - CharacterSelectScene.Current.totalShardsLeft);
                }
                AudioManager.Play("menu_scroll");
                if (dAmount > this.selection)
                    dAmount = dAmount - (dAmount - this.selection);
                this.selection = this.selection - dAmount;
                CharacterSelectScene.Current.UpdateShardsLeft((dAmount));
                this.UpdateArrows();
            }
        }
        else if (this.phaseState == EditShardsGUI.PhaseState.Strength)
        {
            if (this.selection > 0)
            {
                AudioManager.Play("menu_scroll");
                this.selection = this.selection - dAmount;
                this.UpdateArrows();
            }
        }
        
    }

    private void Awake()
    {
        CharacterSelectScene.Current.OnReviewTotalShardsLeftEvent += this.OnReviewShardsLeft;
    }

    private void OnEnable()
    {
        if (this.phaseState == EditShardsGUI.PhaseState.Select)
            rootGUI.OnFadeHighlighterEvent += this.OnFadeHighlighter;
        if (this.phaseState != EditShardsGUI.PhaseState.Select)
            rootGUI.OnFadeArrowsEvent += this.OnFadeArrows;
        /*if (button.leftArrow != null || button.rightArrow != null)
        {
            rootGUI.OnUpdateArrowAnimationEvent += this.OnUpdateArrowAnimation;
        }*/
    }

    private void OnDisable()
    {
        this.holdHighlighter = false;
        if (this.phaseState == EditShardsGUI.PhaseState.Select)
            rootGUI.OnFadeHighlighterEvent -= this.OnFadeHighlighter;
        if (this.phaseState != EditShardsGUI.PhaseState.Select)
            rootGUI.OnFadeArrowsEvent -= this.OnFadeArrows;
        /*if (button.leftArrow != null || button.rightArrow != null)
        {
            rootGUI.OnUpdateArrowAnimationEvent -= this.OnUpdateArrowAnimation;
        }*/
    }

    private void OnDestroy()
    {
        if (CharacterSelectScene.Current != null)
            CharacterSelectScene.Current.OnReviewTotalShardsLeftEvent -= this.OnReviewShardsLeft;
    }

    public void OnFadeHighlighter()
    {
        if (!this.holdHighlighter)
        {
            base.StartCoroutine(fadeHighlighter_cr());
        }
    }

    private IEnumerator fadeHighlighter_cr()
    {
        float alph = this.highlighterCanvasGroup.alpha;
        while (alph > 0f)
        {
            alph -= 0.1f;
            this.highlighterCanvasGroup.alpha = Mathf.Clamp(alph, 0f, 1f);
            yield return null;
        }
        yield break;
    }

    public void InstantFadeHighlighter()
    {
        this.highlighterCanvasGroup.alpha = 0f;
    }

    /*public void InstantFadeHighlighter()
    {
        this.arrowSet.StopAllCoroutines();
        this.arrowSet.state = OptionsGUIVerticalArrowSet.State.Inactive;
        this.arrowSet.canvasGroup.alpha = 0f;
    }*/

    /*public void SummonHighlighter()
    {
        this.arrowSet.canvasGroup.alpha = 1f;
        this.UpdateHighlighter();
    }*/

    public void SummonHighlighter()
    {
        this.StopAllCoroutines();
        base.StartCoroutine(summonHighlighter_cr());
    }

    private IEnumerator summonHighlighter_cr()
    {
        float alph = this.highlighterCanvasGroup.alpha;
        while (alph < 1f)
        {
            alph += 0.1f;
            this.highlighterCanvasGroup.alpha = Mathf.Clamp(alph, 0f, 1f);
            yield return null;
        }
        yield break;
    }

    public void OnFadeArrows()
    {
        if (this.phaseState == this.rootGUI.phaseState)
        {
            this.arrowSet.StopAllCoroutines();
            this.arrowSet.state = OptionsGUIVerticalArrowSet.State.Inactive;
            this.arrowSet.canvasGroup.alpha = 0f;
        }
    }

    public void FadeAllArrows()
    {
        this.arrowSet.StopAllCoroutines();
        this.arrowSet.state = OptionsGUIVerticalArrowSet.State.Inactive;
        this.arrowSet.canvasGroup.alpha = 0f;
    }

    public void SummonArrows()
    {
        if (this.phaseState == this.rootGUI.phaseState)
        {
            this.arrowSet.canvasGroup.alpha = 1f;
            this.UpdateArrows();
        }
    }

    public void UpdateArrows()
    {
        OptionsGUIVerticalArrowSet.State readState = OptionsGUIVerticalArrowSet.State.Active;
        switch (this.phaseState)
        {
            default:
                if (BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards == 0 || (CharacterSelectScene.Current.totalShardsLeft == 0 && rootGUI.editedDefaultShardsHeld == 0))
                {
                    readState = OptionsGUIVerticalArrowSet.State.Inactive;
                }
                else if (rootGUI.editedDefaultShardsHeld >= BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards || CharacterSelectScene.Current.totalShardsLeft == 0)
                {
                    readState = OptionsGUIVerticalArrowSet.State.Max;
                }
                else if (rootGUI.editedDefaultShardsHeld <= 0)
                {
                    readState = OptionsGUIVerticalArrowSet.State.Min;
                }
                if (this.arrowSet.state != readState)
                {
                    this.arrowSet.state = readState;
                    this.arrowSet.StopAllCoroutines();
                    this.arrowSet.SetUpArrow();
                    this.arrowSet.SetDownArrow();
                }
                break;
            case EditShardsGUI.PhaseState.Strength:
                if (BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultShardStrength == 0)
                {
                    readState = OptionsGUIVerticalArrowSet.State.Inactive;
                }
                else if (rootGUI.editedDefaultShardStrength >= BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards)
                {
                    readState = OptionsGUIVerticalArrowSet.State.Max;
                }
                else if (rootGUI.editedDefaultShardStrength <= 0)
                {
                    readState = OptionsGUIVerticalArrowSet.State.Min;
                }
                if (this.arrowSet.state != readState)
                {
                    this.arrowSet.state = readState;
                    this.arrowSet.StopAllCoroutines();
                    this.arrowSet.SetUpArrow();
                    this.arrowSet.SetDownArrow();
                }
                break;
        }
    }

    private void UpdateEnabledArrows()
    {
        if (this.rootGUI.gameObject.activeSelf && this.phaseState == EditShardsGUI.PhaseState.Held)
        {
            this.UpdateArrows();
        }
    }

    private void OnReviewShardsLeft(int shards)
    {
        rootGUI.UpdateShardsLeft(shards);
        this.UpdateEnabledArrows();
    }
}

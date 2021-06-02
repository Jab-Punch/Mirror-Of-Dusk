using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rewired;

public class ButtonGuideGUI : AbstractMB
{
    [SerializeField] private TextMeshProUGUI tmpComponent;
    [SerializeField] private LocalizationHelper localizationHelper;
    [SerializeField] private TextSpriteAction[] textSpriteAction;

    [Serializable]
    public class TextSpriteAction
    {
        public int actionId;
        public int axis = 1;
        public bool isNotAction;

        public TextSpriteAction()
        {

        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        if (DEBUG_AssetLoaderManager.debugWasFound)
        {
            this.UpdateButtonGuideText();
        }
    }

    private void Start()
    {
        PlayerManager.OnPlayerJoinedEvent += this.OnLoaded;
        Localization.OnLanguageChangedEvent += this.UpdateLanguages;
    }

    private void OnDestroy()
    {
        PlayerManager.OnPlayerJoinedEvent -= this.OnLoaded;
        Localization.OnLanguageChangedEvent -= this.UpdateLanguages;
    }

    private void OnLoaded(PlayerId player)
    {
        if (player == PlayerId.PlayerOne)
        {
            if (PlayerManager.IsPlayerUsingController(player))
                this.UpdateButtonGuideText();
        }
    }

    private void UpdateLanguages()
    {
        this.UpdateButtonGuideText();
    }

    public void UpdateButtonGuideText()
    {
        if (this.localizationHelper != null && this.tmpComponent != null)
        {
            this.localizationHelper.ApplyTranslation(Localization.Find(this.localizationHelper.currentID), null);
            if (textSpriteAction.Length < 1)
                return;
            int[] spriteNum = new int[textSpriteAction.Length];
            for (int i = 0; i < textSpriteAction.Length; i++)
            {
                if ((this.textSpriteAction[i].actionId != -1))
                {
                    if (this.textSpriteAction[i].isNotAction)
                    {
                        spriteNum[i] = this.textSpriteAction[i].actionId;
                    } else
                    {
                        if (PlayerManager.IsPlayerUsingController(PlayerId.PlayerOne))
                        {
                            Controller pCont = PlayerManager.GetPlayerJoystick(PlayerId.PlayerOne);
                            ControllerMap pContMap = PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.GetMap(pCont, 2, 1);
                            if (pCont != null)
                            {
                                spriteNum[i] = UserConfigData.GetControllerGlyphId((pCont as Joystick), pContMap.GetFirstButtonMapWithAction(this.textSpriteAction[i].actionId).elementIdentifierId);
                            }
                            else
                            {
                                spriteNum[i] = UserConfigData.GetDefaultActionId(this.textSpriteAction[i].actionId, this.textSpriteAction[i].axis);
                            }
                        }
                        else
                        {
                            spriteNum[i] = UserConfigData.GetDefaultActionId(this.textSpriteAction[i].actionId, this.textSpriteAction[i].axis);
                        }
                    }
                }
            }
            string[] tSprite = new string[textSpriteAction.Length];
            for (int j = 0; j < textSpriteAction.Length; j++) {
                tSprite[j] = String.Format("<sprite index= {0}>", spriteNum[j]);
                if (this.tmpComponent.text.Contains("<"+j+">"))
                {
                    this.tmpComponent.text = this.tmpComponent.text.Replace("<" + j + ">", tSprite[j]);
                }
            }
        }
    }
}

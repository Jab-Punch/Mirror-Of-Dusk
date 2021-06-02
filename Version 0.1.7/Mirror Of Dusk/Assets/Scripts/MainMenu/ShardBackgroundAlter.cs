using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardBackgroundAlter : MonoBehaviour
{

    MenuSelector menuSelector;
    SpriteRenderer sprRender;
    DelayInputMainMenu delayInput;

    [System.Serializable]
    public class ShardBackgrounds
    {
        public string name;
        public Sprite sprite;
    }

    public ShardBackgrounds[] shardBackgrounds;
    private Dictionary<string, string> shardBackgroundList;
    public bool keepImg = false;

    // Use this for initialization
    void Start()
    {
        menuSelector = GameObject.Find("MenuSelector").GetComponent<MenuSelector>();
        delayInput = GameObject.Find("MenuSelector").GetComponent<DelayInputMainMenu>();
        sprRender = gameObject.GetComponent<SpriteRenderer>();
        shardBackgroundList = new Dictionary<string, string> {
            { "SOLO_1" , "SoloModeImage" } ,
            { "MULTIPLAYER_1" , "MultiplayerModeImage" } ,
            { "DEFAULT" , "OtherModeImage" }
        };
        if (!delayInput.checkRefresh())
        {
            keepImg = true;
            changeShardBackground();
        }
        else
        {
            sprRender.color = new Color(sprRender.color.r, sprRender.color.g, sprRender.color.b, 0);
        }
    }

    public void changeShardBackground()
    {
        string shardBackCode = menuSelector.mainMenuDict[menuSelector.currentMainMenuMode][menuSelector.currentSelector];
        bool dictFound = false;
        foreach (KeyValuePair<string, string> list in shardBackgroundList)
        {
            if (list.Key == shardBackCode)
            {
                dictFound = true;
            }
        }
        if (!dictFound)
        {
            shardBackCode = "DEFAULT";
        }
        //Sprite spr = Resources.Load<Sprite>("MainMenu/" + shardBackgroundList[shardBackCode]);
        //sprRender.sprite = spr;
        for (int i = 0; i < shardBackgrounds.Length; i++)
        {
            if (shardBackgrounds[i].name == shardBackgroundList[shardBackCode])
            {
                sprRender.sprite = shardBackgrounds[i].sprite;
                break;
            }
        }
        if (keepImg)
        {
            sprRender.color = new Color(sprRender.color.r, sprRender.color.g, sprRender.color.b, 200f / 255f);
        }
        else
        {
            sprRender.color = new Color(sprRender.color.r, sprRender.color.g, sprRender.color.b, 0);
        }
    }
}

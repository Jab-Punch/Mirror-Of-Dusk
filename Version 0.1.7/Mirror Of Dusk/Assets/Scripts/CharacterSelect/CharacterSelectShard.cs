using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectShard : MonoBehaviour
{
    private CharacterSelectManager characterSelectManager;

    private bool scriptOn = false;
    public int shardID = 0;             //Player ID of the game object
    int currentColor = 0;
    public int selectedColor = 0;       //Current color code of the character icon on the shard
    public int prevSelectedColor = 0;   //Last selected color code of the character icon on the shard before confirm

    Dictionary<int, string> playerIdentity; //For selecting the identity name of a selected character into a string
                                            //Needed for labeling specific character icon sprite names
    public class PlayerColors               //Class for current player's color settings
    {
        public float red;
        public float green;
        public float blue;
    }

    private class TempFDC   //Instance of FighterDataCollection (Data of character colors and statuses)
    {
        public string name;
        public int id;
        public bool active = true;
        public GameObject gm;
        private FighterData _fighterData;

        public TempFDC(string name, int id, bool active, GameObject gm)
        {
            this.name = name;
            this.id = id;
            this.active = active;
            this.gm = gm;
            _fighterData = gm.GetComponent<FighterData>();
        }

        public FighterData fighterData
        {
            get { return _fighterData; }
        }
    }

    Dictionary<int, PlayerColors> playerColor;                      //For organizing a character's list of available colors
    Dictionary<int, Dictionary<int, PlayerColors>> playerColorCode; //For organizing a list of available characters for their colors
    SpriteRenderer sprRenderIcon;
    SpriteRenderer sprRenderBack0;
    SpriteRenderer sprRenderBack1;
    Dictionary<int, Sprite> _largeIcon_sub;                         //List of icons sprites for a character
    Dictionary<int, Dictionary<int, Sprite>> _largeIcon;            //List of characters' collections of icon sprites

    CSPlayerData csPlayerData;
    AnimateCSShard animateCSShard;
    private FighterDataCollection fighterDataCollection;
    private TempFDC[] fighterSelectData;

    [Header("Sprite Prefabs")]
    public GameObject _sprRenderIcon;       //Main icon of character within shard
    public GameObject _sprRenderBack0;      //Background symbol of a character
    public GameObject _sprRenderBack1;      //Background color of a character

    // Use this for initialization
    void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        initializePlayerIdentity();
        initializePlayerColor();
        fighterDataCollection = characterSelectManager.fighterDataCollection.GetComponent<FighterDataCollection>();
        fighterSelectData = new TempFDC[fighterDataCollection.fighterSelectData.Length];
        for (int i = 0; i < fighterSelectData.Length; i++)
        {
            fighterSelectData[i] = new TempFDC(fighterDataCollection.fighterSelectData[i].name, fighterDataCollection.fighterSelectData[i].id, fighterDataCollection.fighterSelectData[i].active, fighterDataCollection.fighterSelectData[i].fighterData);
        }
        sprRenderIcon = _sprRenderIcon.GetComponent<SpriteRenderer>();
        sprRenderBack0 = _sprRenderBack0.GetComponent<SpriteRenderer>();
        sprRenderBack1 = _sprRenderBack1.GetComponent<SpriteRenderer>();
        csPlayerData = characterSelectManager.players[shardID].GetComponent<CSPlayerData>();
        animateCSShard = gameObject.GetComponent<AnimateCSShard>();
        _largeIcon = new Dictionary<int, Dictionary<int, Sprite>>();
        for (int i = 0; i < playerIdentity.Count; i++)
        {
            _largeIcon_sub = new Dictionary<int, Sprite>();
            for (int j = 0; j < 2; j++)
            {
                Sprite spr = Resources.Load<Sprite>("CharacterSelect/" + "LargeIcon_" + playerIdentity[i] + "_" + j.ToString());
                _largeIcon_sub.Add(j, spr);
            }
            _largeIcon.Add(i, _largeIcon_sub);
        }
    }

    void Start()
    {
        scriptOn = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    void OnEnable()
    {
        if (scriptOn)
        {
            animateCSShard.changeState(0);
            UpdateShard();
            StartCoroutine("summonPShard");
        }
    }

    public void UpdateShard()
    {
        sprRenderIcon.sprite = _largeIcon[csPlayerData.characterCode][animateCSShard.shardData[animateCSShard.shardPhase[animateCSShard.shardState][animateCSShard.shardSubFrame]].iconCode];
        for (int i = 0; i < fighterSelectData.Length; i++)
        {
            if (csPlayerData.characterCode == 0 || playerIdentity[csPlayerData.characterCode] == "Unknown")
            {
                for (int j = 0; j < fighterSelectData.Length; j++)
                {
                    if (fighterSelectData[j].name.Contains("Unknown"))
                    {
                        //FighterData fCData = fighterDataCollection.fighterSelectData[j].fighterData.GetComponent<FighterData>();
                        sprRenderIcon.sharedMaterial = fighterSelectData[j].fighterData.fighterColorData[0].selectPalettes[animateCSShard.shardData[animateCSShard.shardPhase[animateCSShard.shardState][animateCSShard.shardSubFrame]].iconCode].bustUpPalette;
                        break;
                    }
                }
                break;
            }
            //Debug.Log(fighterDataCollection.fighterSelectData[i].name + " / " + csPlayerData.selectedCharacterName[csPlayerData.characterCode]);
            if ((fighterSelectData[i].name == csPlayerData.selectedCharacter && fighterSelectData[i].active) || (fighterSelectData[i].name == csPlayerData.selectedCharacterName[csPlayerData.characterCode] && fighterSelectData[i].active))
            {
                //FighterData fCData = fighterDataCollection.fighterSelectData[i].fighterData.GetComponent<FighterData>();
                for (int j = 0; j < fighterSelectData[i].fighterData.fighterColorData.Length; j++)
                {
                    if (selectedColor == j)
                    {
                        if (fighterSelectData[i].fighterData.fighterColorData[j].selectPalettes.Length > 1)
                        {
                            if (fighterSelectData[i].fighterData.fighterColorData[j].selectPalettes[animateCSShard.shardData[animateCSShard.shardPhase[animateCSShard.shardState][animateCSShard.shardSubFrame]].iconCode].bustUpPalette != null)
                            {
                                sprRenderIcon.sharedMaterial = fighterSelectData[i].fighterData.fighterColorData[j].selectPalettes[animateCSShard.shardData[animateCSShard.shardPhase[animateCSShard.shardState][animateCSShard.shardSubFrame]].iconCode].bustUpPalette;
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }
        float pAlpha = 1.0f;
        if (csPlayerData.characterCode == 0)
        {
            pAlpha = 0.0f;
        }
        sprRenderIcon.color = new Color(sprRenderIcon.color.r, sprRenderIcon.color.g, sprRenderIcon.color.b, pAlpha);
        sprRenderBack0.color = new Color(playerColorCode[csPlayerData.characterCode][currentColor].red, playerColorCode[csPlayerData.characterCode][currentColor].green, playerColorCode[csPlayerData.characterCode][currentColor].blue, 1.0f);
        sprRenderBack1.color = new Color(playerColorCode[csPlayerData.characterCode][1].red, playerColorCode[csPlayerData.characterCode][1].green, playerColorCode[csPlayerData.characterCode][1].blue, 1.0f);
    }

    //Summon the active shard displaying selected character icons
    public IEnumerator summonPShard()
    {
        float endMoveY = -150f;
        float curPosY = gameObject.transform.position.y;
        while (curPosY < endMoveY)
        {
            curPosY += 30f;
            if (curPosY > endMoveY)
            {
                curPosY = endMoveY;
            }
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, curPosY, gameObject.transform.position.z);
            yield return null;
        }
        yield return null;
    }

    //Remove the active shard displaying selected character icons
    public IEnumerator destroyPShard()
    {
        float endMoveY = -780f;
        float curPosY = gameObject.transform.position.y;
        while (curPosY > endMoveY)
        {
            curPosY -= 30f;
            if (curPosY < endMoveY)
            {
                curPosY = endMoveY;
            }
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, curPosY, gameObject.transform.position.z);
            yield return null;
        }
        this.gameObject.SetActive(false);
        yield return null;
    }

    //For initializing all available player names assigned to their ids
    private void initializePlayerIdentity()
    {
        playerIdentity = new Dictionary<int, string>
        {
            { 0, "Unknown"},
            { 1, "Tilly"},
            { 2, "Unknown"},
            { 3, "Unknown"},
            { 4, "Unknown"},
            { 5, "Unknown"},
            { 6, "Unknown"},
            { 7, "Unknown"},
            { 8, "Unknown"},
            { 9, "Unknown"},
            { 10, "Unknown"},
            { 11, "Unknown"},
            { 12, "Unknown"}
        };
    }

    //Initialize colors of character backgrounds
    private void initializePlayerColor()
    {
        playerColorCode = new Dictionary<int, Dictionary<int, PlayerColors>>
        {
            { 0, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                } },
            { 1, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 0.6f, green = 0.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 0.82f, green = 0.0f, blue = 1.0f } }
                }
            },
            { 2, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 3, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 4, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 5, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 6, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 7, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 8, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 9, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 10, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 11, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            },
            { 12, new Dictionary<int, PlayerColors> {
                    { 0, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } },
                    { 1, new PlayerColors{ red = 1.0f, green = 1.0f, blue = 1.0f } }
                }
            }
        };
    }
}

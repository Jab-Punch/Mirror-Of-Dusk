using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayerGUI : AbstractMB
{
    private CharacterSelectPlayer player;
    public PlayerId id { get; private set; }

    [Header("Components")]
    [SerializeField] private SpriteRenderer playerNumber;
    [SerializeField] SpriteRenderer pGUICharacterName;
    [SerializeField] Text _mainGuiText;
    [SerializeField] LocalizationHelper textLocalizationHelper;
    [SerializeField] SpriteRenderer joinText;
    [SerializeField] public CharacterSelectPlayerGUIHurtbox[] characterSelectGUIHurtboxes;
    [SerializeField] public CharacterSelectPlayerGUIHurtbox[] characterSelectGUIWindows;
    [SerializeField] private RectTransform colorMenuRoot;
    [SerializeField] private RectTransform shardsMenuRoot;
    [NonSerialized] private ColorMenuGUI colorMenuGUI;
    [NonSerialized] private EditHealthGUI hpMenuGUI;
    [NonSerialized] private EditShardsGUI shardsMenuGUI;

    [Header("Sprites")]
    [SerializeField] private Sprite[] playerNumbers;
    [SerializeField] public CharacterNames[] characterNames;
    [SerializeField] public SpriteRenderer[] nameGleamPool;
    [SerializeField] private SpriteRenderer alterHP;
    [SerializeField] private SpriteRenderer alterShards;

    [Header("Prefabs")]
    [SerializeField] private ColorMenuGUI colorMenuPrefab;
    [SerializeField] private EditHealthGUI hpMenuPrefab;
    [SerializeField] private EditShardsGUI shardsMenuPrefab;

    private bool _initialized = false;
    [NonSerialized] public SummonState summonState;
    [NonSerialized] public ActionState actionState;
    [NonSerialized] public bool guiActive = false;
    private Vector2 restPosition;
    private Vector2 activePosition;
    private IEnumerator JoinFlash;
    [NonSerialized] public WindowHighlightedFlags currentHightlightedFlags = 0;
    [NonSerialized] public WindowHighlightedFlags updatedHightlightedFlags = 0;
    private int characterId = 0;
    [SerializeField] private CharacterSelectPlayerGuiGleamingName gleamingNamePrefab;
    private IEnumerator _readWindow;

    public IEnumerator ReadWindows
    {
        get
        {
            this._readWindow = ReadWindows_cr();
            return _readWindow;
        }
    }

    public string mainGuiText
    {
        get { return _mainGuiText.text; }
        set
        {
            _mainGuiText.text = value;
            int fSize = 28;
            _mainGuiText.fontSize = fSize;
            Canvas.ForceUpdateCanvases();
            if (_mainGuiText.preferredWidth > 340)
            {
                fSize = 14;
            } else if(_mainGuiText.preferredWidth > 240)
            {
                fSize = 18;
            } else if (_mainGuiText.preferredWidth > 180)
            {
                fSize = 22;
            }
            _mainGuiText.fontSize = fSize;
            //Canvas.ForceUpdateCanvases();
        }
    }

    public delegate void ChangePlayerIconStateHandler(CursorHitboxPriority chp);
    public event ChangePlayerIconStateHandler ChangePlayerIconStateEvent;

    private Dictionary<GameObject, int> pooledObjectLimit = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, SpawnObject> spawnedTestObjects = new Dictionary<GameObject, SpawnObject>();

    public class SpawnObject
    {
        public GameObject prefab;
        public int poolId;

        public SpawnObject(GameObject prefab, int poolId)
        {
            this.prefab = prefab;
            this.poolId = poolId;
        }
    }

    [Serializable]
    public class InitObject
    {
        public Vector2 position;
        public int numberId;

        public InitObject(Vector2 position, int numberId)
        {
            this.position = position;
            this.numberId = numberId;
        }
    }

    [Serializable]
    public class CharacterNames
    {
        [SerializeField] public int characterId = 0;
        [SerializeField] public int modeId = 0;
        [SerializeField] public int formId = 0;
        [SerializeField] public CharacterNameMode[] characterNameMode;

        [Serializable] public class CharacterNameMode {
            [SerializeField] public Sprite[] characterName;
        }
    }

    [Serializable]
    public class NameGleamSet
    {
        public bool active = false;
        public IEnumerator nameGleam;

        public NameGleamSet()
        {
            this.active = false;
        }

        public void SetNameGleam(IEnumerator nG)
        {
            this.nameGleam = nG;
        }
    }

    public enum WindowHighlightedFlags
    {
        MenuColor = (1 << 5),
        MenuHP = (1 << 6),
        MenuShards = (1 << 7),
        MenuConfig = (1 << 8)
    }

    public enum SummonState
    {
        Opening,
        Instant
    }

    public enum ActionState
    {
        Busy,
        Free
    }

    public static CharacterSelectPlayerGUI Create(CharacterSelectPlayer player, CharacterSelectPlayerGUI.InitObject init)
    {
        CharacterSelectPlayerGUI characterSelectPlayerGUI = UnityEngine.Object.Instantiate<CharacterSelectPlayerGUI>
            (CharacterSelectScene.Current.characterSelectPlayerGUI);
        characterSelectPlayerGUI.Init(player, init);
        return characterSelectPlayerGUI;
    }

    private void Init(CharacterSelectPlayer player, CharacterSelectPlayerGUI.InitObject init)
    {
        this.player = player;
        base.gameObject.name = "PlayerGUI_" + player.playerId.ToString();
        this.id = player.playerId;
        this.restPosition = init.position;
        this.activePosition = new Vector2(this.restPosition.x, this.restPosition.y + 580f);
        base.transform.position = this.restPosition;
        this.playerNumber.sprite = this.playerNumbers[init.numberId];
        for (int i = 0; i < characterSelectGUIHurtboxes.Length; i++)
        {
            characterSelectGUIHurtboxes[i].Init(this);
        }
        for (int i = 0; i < characterSelectGUIWindows.Length; i++)
        {
            characterSelectGUIWindows[i].Init(this);
        }
        this.colorMenuGUI = this.colorMenuPrefab.InstantiatePrefab<ColorMenuGUI>();
        this.colorMenuGUI.rectTransform.SetParent(this.colorMenuRoot, false);
        this.colorMenuGUI.Init(this.player, this);
        this.hpMenuGUI = this.hpMenuPrefab.InstantiatePrefab<EditHealthGUI>();
        this.hpMenuGUI.rectTransform.SetParent(this.colorMenuRoot, false);
        this.hpMenuGUI.Init(this.player, this);
        this.shardsMenuGUI = this.shardsMenuPrefab.InstantiatePrefab<EditShardsGUI>();
        this.shardsMenuGUI.rectTransform.SetParent(this.shardsMenuRoot, false);
        this.shardsMenuGUI.Init(this.player, this);
        _initialized = true;
        this.gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        this.CreateGleamPool(gleamingNamePrefab.gameObject, 3);
        JoinFlash = JoinFlash_cr();
    }

    private void OnEnable()
    {
        if (_initialized && !guiActive)
        {
            this.actionState = ActionState.Busy;
            pGUICharacterName.sprite = this.GetCharacterName(CharacterSelectScene.Current.playerCursors[(int)this.id].stats.characterSelectedId, 0, 0);
            pGUICharacterName.enabled = false;
            CharacterSelectScene.Current.OnCharacterSelectEvent += this.OnCharacterSelect;
            this.FindTextLocalizer("OptionMenuPollingPlayer", new LocalizationHelper.LocalizationSubtext[] { new LocalizationHelper.LocalizationSubtext("OptionMenuPlayer1", "OptionMenuPlayer", false), new LocalizationHelper.LocalizationSubtext("PlayerNumber", " PlayerNumber:", true) });
            this.mainGuiText = this.mainGuiText.Replace("PlayerNumber:", ((int)this.id + 1).ToString());
            if (summonState == SummonState.Opening)
            {
                this.StartCoroutine(moveGUIBase_cr());
            } else
            {
                this.gameObject.transform.position = this.activePosition;
                this.StartCoroutine(JoinFlash);
                this.SetGuiWindowsActive(true);
                this.actionState = ActionState.Free;
                this.guiActive = true;
            }
        }
    }

    private void OnDisable()
    {
        if (_initialized)
        {
            if (CharacterSelectScene.Current != null)
            {
                CharacterSelectScene.Current.OnCharacterSelectEvent -= this.OnCharacterSelect;
            }
        }
        this.actionState = ActionState.Busy;
        this.guiActive = false;
    }

    private void Update()
    {
        if (!_initialized)
            return;
        if (CharacterSelectScene.Current.playerData[(int)this.id].playerHealth != BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultInitialHealth)
        {
            this.alterHP.enabled = true;
        } else
        {
            this.alterHP.enabled = false;
        }
        if (CharacterSelectScene.Current.playerData[(int)this.id].playerShardsHeld != BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultInitialShards || CharacterSelectScene.Current.playerData[(int)this.id].playerShardStrength != BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultShardStrength)
        {
            this.alterShards.enabled = true;
        }
        else
        {
            this.alterShards.enabled = false;
        }
    }

    private IEnumerator moveGUIBase_cr()
    {
        yield return base.TweenPosition(this.restPosition, this.activePosition, 20f, EaseUtils.EaseType.linear);
        this.StartCoroutine(JoinFlash);
        this.SetGuiWindowsActive(true);
        this.actionState = ActionState.Free;
        this.guiActive = true;
        yield break;
    }

    public IEnumerator JoinFlash_cr()
    {
        for (; ; )
        {
            if (CharacterSelectData.Data.GetPlayerSlot((int)this.id).joinState == CharacterSelectData.PlayerSlot.JoinState.Joined || CharacterSelectScene.Current.playerModes[(int)this.id] != CharacterSelectScene.PlayerMode.Offline)
            {
                joinText.enabled = false;
                yield return null;
                continue;
            }
            joinText.enabled = true;
            yield return new WaitForSeconds(0.75f);
            joinText.enabled = false;
            yield return new WaitForSeconds(0.25f);
        }
        yield break;
    }

    private void FindTextLocalizer(string buttonText, LocalizationHelper.LocalizationSubtext[] subtext = null)
    {
        if (this.textLocalizationHelper != null)
        {
            TranslationElement translationElement = Localization.Find(buttonText);
            if (translationElement != null)
            {
                this.textLocalizationHelper.ApplyTranslation(translationElement, subtext);
                this.mainGuiText = this.mainGuiText;
                return;
            }
        }
        this.mainGuiText = buttonText;
    }

    public Sprite GetCharacterName(int characterId, int mode, int form)
    {
        for (int i = 0; i < this.characterNames.Length; i++)
        {
            if (characterId == this.characterNames[i].characterId)
            {
                return this.characterNames[i].characterNameMode[mode].characterName[form];
            }
        }
        return this.characterNames[0].characterNameMode[0].characterName[0];
    }

    public void OnCharacterSelect(int csId, int selectedId, bool click)
    {
        if (csId == (int)this.id)
        {
            pGUICharacterName.sprite = this.GetCharacterName(selectedId, 0, 0);
            if (selectedId != CharacterSelectScene.Current.playerCharacterShards[csId].characterSelectedId)
            {
                CharacterSelectScene.Current.playerCharacterShards[csId].UpdateIcons(selectedId, 0);
                if (selectedId != -1)
                {
                    AudioManager.Play("star_implode");
                    CharacterSelectScene.Current.playerCharacterShards[csId].Play("SpeedRotate1");
                }
            }
            if (selectedId != -1 || click)
            {
                pGUICharacterName.enabled = true;
                if (click)
                {
                    CharacterSelectScene.Current.playerCharacterShards[csId].Play("Select1");
                    this.SpawnGleam(this.gleamingNamePrefab, pGUICharacterName.gameObject.transform).Init(this, pGUICharacterName.sprite);
                }
            } else
            {
                pGUICharacterName.enabled = false;
            }
        }
    }

    public void DefaultCharacterSelect(int csId, int selectedId)
    {
        if (csId == (int)this.id)
        {
            pGUICharacterName.sprite = this.GetCharacterName(selectedId, 0, 0);
            pGUICharacterName.enabled = true;
        }
    }

    public void DeselectCharacter()
    {
        pGUICharacterName.sprite = this.GetCharacterName(-1, 0, 0);
        pGUICharacterName.enabled = false;
        CharacterSelectScene.Current.playerData[(int)id].characterSelectedId = -1;
    }

    private void CreateGleamPool(GameObject prefab, int initialPoolSize)
    {
        if (prefab != null)
        {
            List<GameObject> list = new List<GameObject>();
            pooledObjectLimit.Add(prefab, initialPoolSize);
            pooledObjects.Add(prefab, list);
            if (initialPoolSize > 0)
            {
                bool activeSelf = prefab.activeSelf;
                prefab.SetActive(true);
                Transform transfrm = this.pGUICharacterName.transform;
                int debugCount = 0;
                while (list.Count < initialPoolSize)
                {
                    GameObject gameObj = UnityEngine.Object.Instantiate<GameObject>(prefab);
                    gameObj.SetActive(false);
                    gameObj.transform.parent = pGUICharacterName.transform;
                    gameObj.transform.position = pGUICharacterName.transform.position;
                    list.Add(gameObj);
                    debugCount++;
                    if (debugCount > 1000)
                        break;
                }
                prefab.SetActive(activeSelf);
            }
        }
    }

    private T SpawnGleam<T>(T prefab, Transform transfrm) where T : Component
    {
        bool found = false;
        GameObject findObj = this.SpawnGleam(prefab.gameObject, transfrm, out found);
        if (found)
        {
            return findObj.GetComponent<T>();
        } else
        {
            this.RecycleGleam(findObj);
            findObj = this.SpawnGleam(prefab.gameObject, transfrm, out found);
            return findObj.GetComponent<T>();
        }
    }

    private GameObject SpawnGleam(GameObject prefab, Transform trans, out bool found)
    {
        List<GameObject> list;
        GameObject gameObj;
        Transform transfrm;
        if (this.pooledObjects.TryGetValue(prefab, out list))
        {
            gameObj = null;
            int debugCount = 0;
            int idCount = this.pooledObjectLimit[prefab] - (list.Count + 1);
            if (list.Count > 0)
            {
                while (gameObj == null && list.Count > 0)
                {
                    idCount++;
                    gameObj = list[0];
                    list.RemoveAt(0);
                    debugCount++;
                    if (debugCount > 1000)
                        break;
                }
                if (gameObj != null)
                {
                    transfrm = trans;
                    transfrm.position = trans.position;
                    gameObj.SetActive(true);
                    //this.spawnedObjects.Add(gameObj, prefab);
                    this.spawnedTestObjects.Add(gameObj, new SpawnObject(prefab, idCount));
                    found = true;
                    return gameObj;
                }
            }
            if (spawnedTestObjects.Count > 0)
            {
                GameObject usedGM = null;
                foreach (GameObject gm in spawnedTestObjects.Keys)
                {
                    //Debug.Log(gm.GetInstanceID());
                    if (spawnedTestObjects[gm].prefab == prefab && spawnedTestObjects[gm].poolId == 0)
                    {
                        usedGM = gm;
                    }
                }
                found = false;
                return usedGM;
            }
            gameObj = UnityEngine.Object.Instantiate<GameObject>(prefab);
            transfrm = gameObj.transform;
            transfrm.parent = trans;
            transfrm.position = trans.position;
            this.spawnedObjects.Add(gameObj, prefab);
            found = true;
            return gameObj;
        }
        gameObj = UnityEngine.Object.Instantiate<GameObject>(prefab);
        gameObj.transform.parent = pGUICharacterName.transform;
        transfrm = trans;
        transfrm.position = trans.position;
        found = true;
        return gameObj;
    }

    public void RecycleGleam<T>(T obj) where T : Component
    {
        this.RecycleGleam(obj.gameObject);
    }

    public void RecycleGleam(GameObject obj)
    {
        //GameObject prefab;
        SpawnObject prefab;
        if (this.spawnedTestObjects.TryGetValue(obj, out prefab))
        {
            this.RecycleGleam(obj, prefab.prefab);
        } else
        {
            UnityEngine.Object.Destroy(obj);
        }
    }

    public void RecycleGleam(GameObject obj, GameObject prefab)
    {
        this.pooledObjects[prefab].Add(obj);
        //this.spawnedObjects.Remove(obj);
        foreach (SpawnObject gm in spawnedTestObjects.Values)
        {
            if (gm.prefab == prefab)
            {
                gm.poolId -= 1;
            }
        }
        this.spawnedTestObjects.Remove(obj);
        if (obj != null)
        {
            //obj.transform.parent = this.transform;
            obj.SetActive(false);
        }
    }

    public void ChangePlayerIconStateTrigger(CursorHitboxPriority chp)
    {
        this.StopCoroutine(JoinFlash);
        this.StartCoroutine(JoinFlash);
        if (this.ChangePlayerIconStateEvent != null)
            this.ChangePlayerIconStateEvent(chp);
    }

    public void DelayInput()
    {
        this.guiActive = false;
        base.FrameDelayedCallback(new Action(this.SetGuiActive), 48);
    }

    private void SetGuiActive()
    {
        this.guiActive = true;
    }

    public void SetGuiWindowsActive(bool set)
    {
        for (int i = 0; i < characterSelectGUIWindows.Length; i++)
        {
            characterSelectGUIWindows[i].enabled = set;
        }
    }

    public void UpdateWindows()
    {
        if (this.currentHightlightedFlags != this.updatedHightlightedFlags)
        {
            this.currentHightlightedFlags = this.updatedHightlightedFlags;
            this.StopThisCoroutine(_readWindow);
            this.StartCoroutine(ReadWindows);
        }
    }

    private IEnumerator ReadWindows_cr()
    {
        for (; ; )
        {
            bool skip = false;
            for (int i = 5; i < 9; i++)
            {
                if (currentHightlightedFlags.HasFlag((WindowHighlightedFlags)(1 << i)))
                {
                    skip = true;
                    int wFrame = 60;
                    while (wFrame > 0)
                    {
                        switch (i)
                        {
                            default:
                                //this.mainGuiText = ("PLAYER " + ((int)this.id + 1).ToString());
                                this.FindTextLocalizer("OptionMenuPollingPlayer", new LocalizationHelper.LocalizationSubtext[] { new LocalizationHelper.LocalizationSubtext("OptionMenuPlayer1", "OptionMenuPlayer", false), new LocalizationHelper.LocalizationSubtext("PlayerNumber", " PlayerNumber:", true) });
                                this.mainGuiText = this.mainGuiText.Replace("PlayerNumber:", ((int)this.id + 1).ToString());
                                break;
                            case 5:
                                int foundCharId = CharacterSelectScene.Current.playerCharacterShards[(int)this.id].GetCharacterId(CharacterSelectScene.Current.playerCharacterShards[(int)this.id].characterSelectedId);
                                if (foundCharId != 0)
                                {
                                    if (CharacterSelectScene.Current.playerCharacterShards[(int)this.id].gameObject.activeSelf)
                                    {
                                        this.FindTextLocalizer(CharacterSelectScene.Current.characterIconData.data[foundCharId].normal.selectPalettes[CharacterSelectScene.Current.playerCharacterShards[(int)this.id].characterColorCode].name);
                                        this.mainGuiText = ((CharacterSelectScene.Current.playerCharacterShards[(int)this.id].characterColorCode + 1).ToString()) + ": " + this.mainGuiText;
                                    } else
                                    {
                                        this.FindTextLocalizer("usergui_color_nocolor");
                                    }
                                    //this.mainGuiText = ((CharacterSelectScene.Current.playerCharacterShards[(int)this.id].gameObject.activeSelf) ? ((CharacterSelectScene.Current.playerCharacterShards[(int)this.id].characterColorCode + 1).ToString()) + ": " + (CharacterSelectScene.Current.characterIconData.data[foundCharId].normal.selectPalettes[CharacterSelectScene.Current.playerCharacterShards[(int)this.id].characterColorCode].name) : "No Color");
                                } else
                                {
                                    this.FindTextLocalizer("usergui_color_nocolor");
                                }
                                break;
                            case 6:
                                this.mainGuiText = CharacterSelectScene.Current.playerData[(int)this.id].playerHealth.ToString() + " HP";
                                break;
                            case 7:
                                this.mainGuiText = CharacterSelectScene.Current.playerData[(int)this.id].playerShardsHeld.ToString() + " / " + BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards;
                                break;
                            case 8:
                                this.FindTextLocalizer((CharacterSelectData.Data.GetPlayerSlot((int)this.id).controllerState == CharacterSelectData.PlayerSlot.ControllerState.UsingController) ? "usergui_controller_yes" : "usergui_controller_no");
                                break;
                        }
                        yield return null;
                        wFrame--;
                    }
                }
            }
            if (!skip)
            {
                //this.mainGuiText = ("PLAYER " + ((int)this.id + 1).ToString());
                this.FindTextLocalizer("OptionMenuPollingPlayer", new LocalizationHelper.LocalizationSubtext[] { new LocalizationHelper.LocalizationSubtext("OptionMenuPlayer1", "OptionMenuPlayer", false), new LocalizationHelper.LocalizationSubtext("PlayerNumber", " PlayerNumber:", true) });
                this.mainGuiText = this.mainGuiText.Replace("PlayerNumber:", ((int)this.id + 1).ToString());
                yield return null;
            }
        }
        yield break;
    }

    private void StopThisCoroutine(IEnumerator cr)
    {
        if (cr != null)
            this.StopCoroutine(cr);
    }

    public void OpenColorMenu(CharacterSelectPlayer player, int selectedCharacterId)
    {
        this.colorMenuGUI.EnableThisGUI(player, selectedCharacterId);
    }

    public void OpenHPMenu(CharacterSelectPlayer player, int selectedCharacterId)
    {
        this.hpMenuGUI.EnableThisGUI(player, selectedCharacterId);
    }

    public void OpenShardsMenu(CharacterSelectPlayer player, int selectedCharacterId)
    {
        this.shardsMenuGUI.EnableThisGUI(player, selectedCharacterId);
    }
}

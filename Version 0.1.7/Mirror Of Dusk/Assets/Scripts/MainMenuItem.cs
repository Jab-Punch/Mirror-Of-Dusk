using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class MainMenuItem : AbstractMB
{
    private const float FLOAT_TIME = 0.3f;
    [SerializeField] float moveTime;
    public MainMenuItemType mainMenuType;

    public enum MainMenuItemType
    {
        Main,
        Sub
    }

    public MainMenuItemMainType mainMenuItemMainType;
    public MainMenuItemSubType mainMenuItemSubType;
    public MainMenuSections mainMenuItemSection;
    public MainMenuSections mainMenuPreviousSection;
    [NonSerialized] public Scenes mainMenuEnterScene = Scenes.scene_main_menu;

    [Header("Sprites")]
    public SpriteRenderer spriteInactive;
    public SpriteRenderer spriteSelected;

    [Header("Texts")]
    public TextMeshProUGUI leadText;
    [SerializeField] private AdjustTMPMaterial adjustTMPMaterialComponent;
    [SerializeField] private CanvasRenderer canvasRendererComponent;
    public Color selectedTextColor;
    public Color unselectedTextColor;
    public Color selectedTextOutlineColor;
    public Color unselectedTextOutlineColor;
    public Color selectedTextGlowColor;
    public Color unselectedTextGlowColor;


    //[HideInInspector]
    public Vector3 endPosition;

    //[HideInInspector]
    public Vector3 startPosition;

    private Vector3 _initialItemPos;
    private bool _initailItemPosInit = false;

    private Vector3 InitialItemPos
    {
        get
        {
            if (!_initailItemPosInit)
            {
                _initailItemPosInit = true;
                return _initialItemPos = this.leadText.transform.position;
            }
            return _initialItemPos;
        }
    }

    private Coroutine selectionCoroutine;
    

    public enum State
    {
        NotReady,
        Ready,
        Returning,
        Busy
    }

    public enum SelectState
    {
        NotSelected,
        Selected
    }

    public enum SpriteState
    {
        Inactive,
        Selected
    }

    public MainMenuItem.State state { get; private set; }
    private MainMenuItem.SelectState _selectState;
    public MainMenuItem.SelectState selectState {
        get
        {
            return this._selectState;
        }
        set
        {
            if (this.state == MainMenuItem.State.Ready && value == MainMenuItem.SelectState.Selected)
            {
                base.StartCoroutine(BumpSelect());
            }
            this._selectState = value;
        }
    }

    public int DisplayId
    {
        get
        {
            switch (this.mainMenuType)
            {
                //case MainMenuItemType.Main:
                    //return MainMenuItemProperties.GetDisplayId(this.mainMenuItemMainType);
                case MainMenuItemType.Sub:
                    return MainMenuItemProperties.GetDisplayId(this.mainMenuItemSubType);
                default:
                    return -1;
            }
        }
    }

    public int DescriptionId
    {
        get
        {
            switch (this.mainMenuType)
            {
                //case MainMenuItemType.Main:
                //return MainMenuItemProperties.GetDisplayId(this.mainMenuItemMainType);
                case MainMenuItemType.Sub:
                    return MainMenuItemProperties.GetDescriptionId(this.mainMenuItemSubType);
                default:
                    return -1;
            }
        }
    }

    public string DisplayName
    {
        get
        {
            switch (this.mainMenuType)
            {
                case MainMenuItemType.Main:
                    return MainMenuItemProperties.GetDisplayName(this.mainMenuItemMainType);
                case MainMenuItemType.Sub:
                    return MainMenuItemProperties.GetDisplayName(this.mainMenuItemSubType);
                default:
                    return string.Empty;
            }
        }
    }

    public float DisplayNameSize
    {
        get
        {
            switch (this.mainMenuType)
            {
                case MainMenuItemType.Main:
                    return MainMenuItemProperties.GetDisplayNameFont(this.mainMenuItemMainType);
                default:
                    return 0f;
            }
        }
    }

    public string DescName
    {
        get
        {
            switch (this.mainMenuType)
            {
                case MainMenuItemType.Sub:
                    return MainMenuItemProperties.GetDescName(this.mainMenuItemSubType);
                default:
                    return string.Empty;
            }
        }
    }

    public string Description
    {
        get
        {
            switch (this.mainMenuType)
            {
                case MainMenuItemType.Main:
                    return MainMenuItemProperties.GetDescription(this.mainMenuItemMainType);
                case MainMenuItemType.Sub:
                    return MainMenuItemProperties.GetDescription(this.mainMenuItemSubType);
                default:
                    return string.Empty;
            }
        }
    }

    public void Init()
    {
        this.startPosition = base.transform.localPosition;
        //this.endPosition = this.startPosition;
        //this.endPosition.y = this.endPosition.y + 40f;
        this.SetSprite(MainMenuItem.SpriteState.Inactive);
        if (leadText != null)
        {
            leadText.text = this.DisplayName;
            float tempFontSize = this.DisplayNameSize;
            if (tempFontSize != 0)
            {
                leadText.fontSize = tempFontSize;
            }
        }
        this.StopAllCoroutines();
        base.StartCoroutine(this.adjust_cr(base.transform.localPosition, this.endPosition, MainMenuItem.State.Ready));
    }

    private void SetSprite(MainMenuItem.SpriteState spriteState)
    {
        this.spriteInactive.enabled = false;
        this.spriteSelected.enabled = false;
        switch (spriteState)
        {
            case MainMenuItem.SpriteState.Inactive:
                this.spriteInactive.enabled = true;
                break;
            case MainMenuItem.SpriteState.Selected:
                this.spriteSelected.enabled = true;
                break;
        }
    }

    public void Select()
    {
        if (this.state != MainMenuItem.State.Ready)
        {
            return;
        }
        this.StopAllCoroutines();
        base.StartCoroutine(this.float_cr(base.transform.localPosition, this.endPosition));
    }

    public void Deselect()
    {
        if (this.state != MainMenuItem.State.Ready)
        {
            return;
        }
        this.StopAllCoroutines();
        base.StartCoroutine(this.float_cr(base.transform.localPosition, this.startPosition));
    }

    private void UpdateFloat(float value)
    {
        base.transform.localPosition = Vector3.Lerp(this.startPosition, this.endPosition, value);
    }

    public void SendItemBack()
    {
        this.state = MainMenuItem.State.Returning;
        Vector3 tempStartPos = this.endPosition;
        Vector3 tempEndPos = this.startPosition;
        //this.startPosition = base.transform.localPosition;
        //this.endPosition = tempStartPos;
        this.SetSprite(MainMenuItem.SpriteState.Inactive);
        this.StopAllCoroutines();
        base.StartCoroutine(this.adjust_cr(tempStartPos, tempEndPos, MainMenuItem.State.NotReady));
    }

    private IEnumerator adjust_cr(Vector3 start, Vector3 end, State _state)
    {
        bool reset = (_state == MainMenuItem.State.NotReady) ? true : false;
        yield return base.StartCoroutine(float_cr(start, end, reset));
        this.state = _state;
        if (_state == MainMenuItem.State.Ready)
            this.leadText.transform.position = InitialItemPos;
        yield return null;
        yield break;
    }

    private IEnumerator float_cr(Vector3 start, Vector3 end, bool reset = false)
    {
        float t = 0f;
        float time = 0.3f * (Vector3.Distance(start, end) / Vector3.Distance(this.startPosition, this.endPosition));
        while (t < moveTime)
        {
            float val = t / moveTime;
            base.transform.localPosition = Vector3.Lerp(start, end, EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, val));
            t += 1f;
            yield return null;
        }
        if (reset)
            this.selectState = MainMenuItem.SelectState.NotSelected;
        base.transform.localPosition = end;
        yield return null;
        yield break;
    }

    private void OnDestroy()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //this.Init();
        //this.endPosition = new Vector3(-400f, base.transform.localPosition.y, base.transform.localPosition.z);
        
    }
    void Update()
    {
        if (this.selectState == MainMenuItem.SelectState.Selected)
        {
            this.leadText.color = selectedTextColor;
            /*this.leadText.gameObject.GetComponent<CanvasRenderer>().GetMaterial().SetColor("_OutlineColor", selectedTextOutlineColor);
            this.leadText.gameObject.GetComponent<CanvasRenderer>().GetMaterial().SetColor("_GlowColor", selectedTextGlowColor);
            this.leadText.gameObject.GetComponent<CanvasRenderer>().GetMaterial().SetFloat("_GlowOuter", 0.5f);*/
            if (this.adjustTMPMaterialComponent != null && this.canvasRendererComponent != null)
            {
                if (this.canvasRendererComponent.materialCount > 0)
                {
                    Material material = AdjustTMPMaterialManager.TmpManager.GetTMPMaterial(adjustTMPMaterialComponent.getMaterial(Localization.language, true));
                    //this.leadText.gameObject.GetComponent<CanvasRenderer>().SetTexture(material.GetTexture("_MainTex"));
                    this.canvasRendererComponent.SetMaterial(material, 0);
                }
            }
            //this.leadText.gameObject.GetComponent<CanvasRenderer>().SetMaterial(this.gameObject.GetComponent<AdjustTMPMaterial>().);
            //this.leadText.outlineColor = selectedTextOutlineColor;
            //this.leadText.fontSharedMaterial.SetColor("_GlowColor", selectedTextGlowColor);
            //this.leadText.fontSharedMaterial.SetFloat("_GlowOuter", 0.5f);
        } else
        {
            this.leadText.color = unselectedTextColor;
            if (this.adjustTMPMaterialComponent != null && this.canvasRendererComponent != null)
            {
                if (this.canvasRendererComponent.materialCount > 0)
                {
                    Material material = AdjustTMPMaterialManager.TmpManager.GetTMPMaterial(adjustTMPMaterialComponent.getMaterial(Localization.language, false));
                    //this.leadText.gameObject.GetComponent<CanvasRenderer>().SetTexture(material.GetTexture("_MainTex"));
                    this.canvasRendererComponent.SetMaterial(material, 0);
                }
            }
            //this.leadText.outlineColor = unselectedTextOutlineColor;
            //this.leadText.fontSharedMaterial.SetColor("_GlowColor", unselectedTextGlowColor);
            //this.leadText.fontSharedMaterial.SetFloat("_GlowOuter", 0.25f);
        }
    }

    public IEnumerator BumpSelect()
    {
        this.leadText.transform.position = InitialItemPos;
        bool moveBack = false;
        bool moveFinish = false;
        int moveCount = 0;
        AudioManager.Play("menu_scroll");
        while (!moveFinish)
        {
            if (!moveBack)
            {
                this.leadText.transform.position = new Vector3(this.leadText.transform.position.x + 3, this.leadText.transform.position.y);
                moveCount++;
                if (moveCount > 5)
                {
                    moveBack = true;
                }
            }
            else
            {
                this.leadText.transform.position = new Vector3(this.leadText.transform.position.x - 3, this.leadText.transform.position.y);
                moveCount--;
                if (moveCount <= 0)
                {
                    moveFinish = true;
                }
            }

            // Yield execution of this coroutine and return to the main loop until next frame
            yield return null;
        }
        yield break;
    }


}

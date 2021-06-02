using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class MainMenuBackgroundGroup : AbstractMB
{
    public static readonly MainMenuSections[] mainMenuSections = new MainMenuSections[]
    {
        MainMenuSections.Default,
        MainMenuSections.Solo
    };

    public static MainMenuBackgroundGroup Current { get; private set; }

    public static MainMenuSections PreviousSection { get; private set; }

    public bool Initialized { get; private set; }

    public MainMenuSections CurrentSection { get; }

    private MainMenuBackgroundProperties.DefaultGroup properties;



    [Space(10f)]
    [SerializeField]
    public MainMenuSections assignedSection;
    [SerializeField]
    public MainMenuSections assignedPreviousSection;
    [SerializeField]
    public MainMenuBackgroundGroupEntity[] backgroundItem;

    public bool AssignedSectionFound {
        get {
            ActivateAssigned(false);
            return (assignedSection == MainMenuScene.Current.CurrentSection);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        //this.PartialInit();
    }

    public void Init()
    {
        this.PartialInit();
    }

    private void PartialInit()
    {
        this.properties = MainMenuBackgroundProperties.DefaultGroup.GetMode(assignedSection);
        for (int i = 0; i < backgroundItem.Length; i++)
        {
            backgroundItem[i].BackgroundInit(this.properties, i);
        }
        this.ActivateAssigned(true);
        this.Initialized = true;
        //this.properties.OnStateChange += base.zHack_OnStateChanged;
    }

    private void ActivateAssigned(bool startScene)
    {
        if (startScene)
        {
            if (assignedSection == MainMenuData.Data.CurrentSectionData.sectionPlacement)
            {
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        } else
        {
            if (assignedSection == MainMenuScene.Current.CurrentSection)
            {
                this.gameObject.SetActive(true);
            }
        }
    }

    public void UnassignSection()
    {
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        MainMenuScene.Current.OnGroupTravelEvent += this.OnTravel;
        MainMenuScene.Current.OnGroupRetreatEvent += this.OnRetreat;
    }

    /*private void OnDestroy()
    {
        MainMenuScene.Current.OnGroupTravelEvent -= this.OnTravel;
    }*/

    private void OnDisable()
    {
        if (MainMenuScene.Current != null)
        {
            MainMenuScene.Current.OnGroupTravelEvent -= this.OnTravel;
            MainMenuScene.Current.OnGroupRetreatEvent -= this.OnRetreat;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTravel()
    {
        base.StartCoroutine(startTravel_cr());
        if (MainMenuScene.Current.CurrentSection == MainMenuSections.Enter)
        {
            MainMenuScene.Current.EnterNextScene();
        } else
        {
            base.StartCoroutine(summonNextBackground_cr());
        }
    }

    private IEnumerator startTravel_cr()
    {
        for (int i = 0; i < backgroundItem.Length; i++)
        {
            backgroundItem[i].StartTravelForward(EaseUtils.EaseType.easeInOutQuart);
        }
        yield return new WaitForSeconds(0.95f);
        MainMenuScene.Current.UpdateMenuItems();
        UnassignSection();
        yield break;
    }

    public void ExecuteFinishTravel()
    {
        for (int i = 0; i < backgroundItem.Length; i++)
        {
            backgroundItem[i].SetSortingLayer = "Foreground_3";
            backgroundItem[i].FinishTravelForward(EaseUtils.EaseType.easeOutQuart);
        }
    }

    private void OnRetreat()
    {
        base.StartCoroutine(startRetreat_cr());
    }

    private IEnumerator startRetreat_cr()
    {
        for (int i = 0; i < backgroundItem.Length; i++)
        {
            backgroundItem[i].StartTravelBackward(EaseUtils.EaseType.easeInOutQuart);
        }
        yield return new WaitForSeconds(0.25f);
        MainMenuScene.Current.SummonPrevBackground();
        yield return new WaitForSeconds(0.70f);
        //MainMenuScene.Current.UpdateMenuItems();
        UnassignSection();
        yield break;
    }

    public void ExecuteFinishRetreat(MainMenuSections section)
    {
        for (int i = 0; i < backgroundItem.Length; i++)
        {
            backgroundItem[i].SetSortingLayer = "Foreground_2";
            backgroundItem[i].FinishTravelBackward(EaseUtils.EaseType.easeOutQuart, section);
        }
    }

    private IEnumerator summonNextBackground_cr()
    {
        yield return new WaitForSeconds(0.2f);
        MainMenuScene.Current.SummonNextBackground();
        yield break;
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Reflection;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager characterSelectManager = null;
    //public CharacterSelectManagerContent content;

    /*[System.Serializable]
    public class CharacterSelectTitleBase : CharacterSelectManagerContent
    {
        public GameObject characterSelectTitleBase;
        private CSMoveMenuItem csMoveMenuItem;

        public void C_Init()
        {
            Debug.Log(characterSelectTitleBase);
            if (this.characterSelectTitleBase != null)
            {
                this.csMoveMenuItem = characterSelectTitleBase.GetComponent<CSMoveMenuItem>();
            }
        }

        public CSMoveMenuItem CsMoveMenuItem
        {
            get { return this.csMoveMenuItem; }
        }
    }*/
    private GameObject _eventSystem;
    public GameObject eventSystem {
        get { return _eventSystem; }
        private set { _eventSystem = value; }
    }
    [SerializeField] public GameObject entryBlackScreen;
    [SerializeField] public GameObject csTitleBase;
    [SerializeField] public GameObject csRulesBase;
    [SerializeField] public GameObject csBackBase;
    [SerializeField] public GameObject csControl;
    [SerializeField] public GameObject gameData;
    [SerializeField] public GameObject[] csStar;
    [SerializeField] public GameObject activePlayers;
    [SerializeField] public GameObject[] players;
    private GameObject[] _csCursors = new GameObject[4];
    public GameObject[] csCursors {
        get { return _csCursors; }
        private set { _csCursors = value; } 
    }
    private GameObject[] _csCursorStars = new GameObject[4];
    public GameObject[] csCursorStars
    {
        get { return _csCursorStars; }
        private set { _csCursorStars = value; }
    }
    [SerializeField] public GameObject musicPlayer;
    [SerializeField] public GameObject sfxPlayer;
    [SerializeField] public GameObject csPlayerGUI;
    [SerializeField] public GameObject csShards;
    [SerializeField] public GameObject rulesMenuScreen;
    [SerializeField] public GameObject shardSettingsRulesScreen;
    [SerializeField] public GameObject selectUserMenuScreens;
    [SerializeField] public GameObject selectUserNewNameScreens;
    [SerializeField] public GameObject selectUserDeleteNameScreens;
    [SerializeField] public GameObject colorMenuScreens;
    [SerializeField] public GameObject hpMenuScreens;
    [SerializeField] public GameObject shardsMenuScreens;
    [SerializeField] public GameObject fighterDataCollection;


    private void Awake()
    {
        if (characterSelectManager == null)
        {
            characterSelectManager = this;
        }
        else if (characterSelectManager != null)
        {
            Destroy(gameObject);
        }
        characterSelectManager.eventSystem = characterSelectManager.gameObject;

        //for (int i = 0; i < CharacterSelectManagerContent.)
        //SetUp();
    }

    /*private void Start()
    {
        Binder defaultBinder = System.Type.DefaultBinder;
        foreach (CharacterSelectManagerContent ex in GetAllEntities())
        {
            ex.C_Init();
        }
    }

    public List<object> GetAllEntities()
    {
        var cd = System.AppDomain.CurrentDomain;
        return cd.GetAssemblies().SelectMany(x => x.GetTypes())
             .Where(x => typeof(CharacterSelectManagerContent).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
             .Select(x => System.Activator.CreateInstance(x)).ToList();
    }*/

}




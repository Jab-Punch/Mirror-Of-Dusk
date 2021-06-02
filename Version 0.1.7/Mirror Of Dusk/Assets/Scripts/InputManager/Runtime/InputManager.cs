using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using UnityEngine.Profiling;

//public delegate bool ScanHandler(ScanResult result);
public delegate void RemoteUpdateDelegate(PlayerID playerID);

public partial class InputManager : MonoBehaviour {

    [SerializeField]
    private List<ControlScheme> m_controlSchemes = new List<ControlScheme>();
    [SerializeField]
    private string m_playerOneDefault;
    [SerializeField]
    private string m_playerTwoDefault;
    [SerializeField]
    private string m_playerThreeDefault;
    [SerializeField]
    private string m_playerFourDefault;
    /*[SerializeField]
    private bool m_ignoreTimeScale = true;*/

    private ControlScheme m_playerOneScheme;
    private ControlScheme m_playerTwoScheme;
    private ControlScheme m_playerThreeScheme;
    private ControlScheme m_playerFourScheme;
    //private ScanService m_scanService;
    private Action<PlayerID> m_playerControlsChangedHandler;
    private Action m_controlSchemesChangedHandler;
    private Action m_loadedHandler;
    private Action m_savedHandler;
    private Action m_beforeUpdateHandler;
    private Action m_afterUpdateHandler;
    private RemoteUpdateDelegate m_remoteUpdateHandler;
    private static InputManager m_instance;

    //private Dictionary<Type, IInputService> m_services;
    private Dictionary<string, ControlScheme> m_schemeLookup;
    private Dictionary<string, ControlScheme> m_schemeLookupByID;
    //private Dictionary<string, Dictionary<string, InputAction>> m_actionLookup;

    public List<ControlScheme> ControlSchemes
    {
        get { return m_controlSchemes; }
    }

    public string PlayerOneDefault
    {
        get { return m_playerOneDefault; }
        set { m_playerOneDefault = value; }
    }

    public string PlayerTwoDefault
    {
        get { return m_playerTwoDefault; }
        set { m_playerTwoDefault = value; }
    }

    public string PlayerThreeDefault
    {
        get { return m_playerThreeDefault; }
        set { m_playerThreeDefault = value; }
    }

    public string PlayerFourDefault
    {
        get { return m_playerFourDefault; }
        set { m_playerFourDefault = value; }
    }

    /*public bool IgnoreTimescale
    {
        get { return m_ignoreTimescale; }
        set { m_ignoreTimescale = value; }
    }*/

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;

            m_schemeLookup = new Dictionary<string, ControlScheme>();
            m_schemeLookupByID = new Dictionary<string, ControlScheme>();

            AddDefaultServices();
            Initialize();
        } else
        {
            Debug.LogWarning("You have multiple InputManager instances in the scene!", gameObject);
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (m_instance == this)
        {
            m_instance = null;
        }

        m_playerControlsChangedHandler = null;
        m_controlSchemesChangedHandler = null;
        m_loadedHandler = null;
        m_savedHandler = null;
        m_remoteUpdateHandler = null;

        /*foreach (var entry in m_services)
        {
            entry.Value.ShutDown();
        }*/

        //m_services.Clear();
    }

    private void AddDefaultServices()
    {

    }

    private void Initialize()
    {
        m_schemeLookup.Clear();

        m_playerOneScheme = null;
        m_playerTwoScheme = null;
        m_playerThreeScheme = null;
        m_playerFourScheme = null;

        if (m_controlSchemes.Count == 0)
            return;

        PopulateLookupTables();

        if (!string.IsNullOrEmpty(m_playerOneDefault) && m_schemeLookupByID.ContainsKey(m_playerOneDefault))
        {
            m_playerOneScheme = m_schemeLookupByID[m_playerOneDefault];
        }
        else
        {
            if (m_controlSchemes.Count > 0)
                m_playerOneScheme = m_controlSchemes[0];
        }

        if (!string.IsNullOrEmpty(m_playerTwoDefault) && m_schemeLookupByID.ContainsKey(m_playerTwoDefault))
        {
            m_playerTwoScheme = m_schemeLookupByID[m_playerTwoDefault];
        }

        if (!string.IsNullOrEmpty(m_playerThreeDefault) && m_schemeLookupByID.ContainsKey(m_playerThreeDefault))
        {
            m_playerThreeScheme = m_schemeLookupByID[m_playerThreeDefault];
        }

        if (!string.IsNullOrEmpty(m_playerFourDefault) && m_schemeLookupByID.ContainsKey(m_playerFourDefault))
        {
            m_playerFourScheme = m_schemeLookupByID[m_playerFourDefault];
        }

        foreach (ControlScheme scheme in m_controlSchemes)
        {
            scheme.Initialize();
        }

        Input.ResetInputAxes();
    }

    private void PopulateLookupTables()
    {
        m_schemeLookup.Clear();
        m_schemeLookupByID.Clear();
        foreach(ControlScheme scheme in m_controlSchemes)
        {
            m_schemeLookup[scheme.Name] = scheme;
            m_schemeLookupByID[scheme.UniqueID] = scheme;
        }
    }
	
	// Update is called once per frame
	void Update () {
        Profiler.BeginSample("OnBeforeUpdate", this);

        if (m_beforeUpdateHandler != null)
            m_beforeUpdateHandler();

        Profiler.EndSample();

        Profiler.BeginSample("Update", this);
        UpdateControlScheme(m_playerOneScheme, PlayerID.One);
        UpdateControlScheme(m_playerTwoScheme, PlayerID.Two);
        UpdateControlScheme(m_playerThreeScheme, PlayerID.Three);
        UpdateControlScheme(m_playerFourScheme, PlayerID.Four);
        //UpdateScanService();

        Profiler.EndSample();

        Profiler.BeginSample("OnAfterUpdate", this);

        if (m_afterUpdateHandler != null)
            m_afterUpdateHandler();

        Profiler.EndSample();
    }

    private void UpdateControlScheme(ControlScheme scheme, PlayerID playerID)
    {
        if (scheme != null)
        {
            float deltaTime = Time.unscaledDeltaTime;
            scheme.Update(deltaTime);

            if (m_remoteUpdateHandler != null)
                m_remoteUpdateHandler(playerID);
        }
    }

    private void SetControlSchemeByPlayerID(PlayerID playerID, ControlScheme scheme)
    {
        if (playerID == PlayerID.One)
            m_playerOneScheme = scheme;
        else if (playerID == PlayerID.Two)
            m_playerTwoScheme = scheme;
        else if (playerID == PlayerID.Three)
            m_playerThreeScheme = scheme;
        else if (playerID == PlayerID.Four)
            m_playerFourScheme = scheme;
    }

    private ControlScheme GetControlSchemeByPlayerID(PlayerID playerID)
    {
        if (playerID == PlayerID.One)
            return m_playerOneScheme;
        else if (playerID == PlayerID.Two)
            return m_playerTwoScheme;
        else if (playerID == PlayerID.Three)
            return m_playerThreeScheme;
        else if (playerID == PlayerID.Four)
            return m_playerFourScheme;
        else
            return null;
    }

    private PlayerID? IsControlSchemeInUse(string name)
    {
        if (m_playerOneScheme != null && m_playerOneScheme.Name == name)
            return PlayerID.One;
        if (m_playerTwoScheme != null && m_playerTwoScheme.Name == name)
            return PlayerID.Two;
        if (m_playerThreeScheme != null && m_playerThreeScheme.Name == name)
            return PlayerID.Three;
        if (m_playerFourScheme != null && m_playerFourScheme.Name == name)
            return PlayerID.Four;

        return null;
    }

    private void OnInitializeAfterScriptReload()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("You have multiple InputManager instances in the scene!", gameObject);
        }
        else if (m_instance == null)
        {
            m_instance = this;
            m_schemeLookup = new Dictionary<string, ControlScheme>();
            /*m_actionLookup = new Dictionary<string, Dictionary<string, InputAction>>();*/

            Initialize();
        }
    }

    private void RaisePlayerControlsChangedEvent(PlayerID playerID)
    {
        if (m_playerControlsChangedHandler != null)
            m_playerControlsChangedHandler(playerID);
    }

    private void RaiseControlSchemesChangedEvent()
    {
        if (m_controlSchemesChangedHandler != null)
            m_controlSchemesChangedHandler();
    }

    private void RaiseLoadedEvent()
    {
        if (m_loadedHandler != null)
            m_loadedHandler();
    }

    private void RaiseSavedEvent()
    {
        if (m_savedHandler != null)
            m_savedHandler();
    }

    #region [Static Interface]
    public static event Action<PlayerID> PlayerControlsChanged
    {
        add { if (m_instance != null) m_instance.m_playerControlsChangedHandler += value; }
        remove { if (m_instance != null) m_instance.m_playerControlsChangedHandler -= value; }
    }

    public static event Action ControlSchemesChanged
    {
        add { if (m_instance != null) m_instance.m_controlSchemesChangedHandler += value; }
        remove { if (m_instance != null) m_instance.m_controlSchemesChangedHandler -= value; }
    }

    public static event Action Loaded
    {
        add { if (m_instance != null) m_instance.m_loadedHandler += value; }
        remove { if (m_instance != null) m_instance.m_loadedHandler -= value; }
    }

    public static event Action Saved
    {
        add { if (m_instance != null) m_instance.m_savedHandler += value; }
        remove { if (m_instance != null) m_instance.m_savedHandler -= value; }
    }

    public static event Action BeforeUpdate
    {
        add { if (m_instance != null) m_instance.m_beforeUpdateHandler += value; }
        remove { if (m_instance != null) m_instance.m_beforeUpdateHandler -= value; }
    }

    public static event Action AfterUpdate
    {
        add { if (m_instance != null) m_instance.m_afterUpdateHandler += value; }
        remove { if (m_instance != null) m_instance.m_afterUpdateHandler -= value; }
    }

    public static event RemoteUpdateDelegate RemoteUpdate
    {
        add { if (m_instance != null) m_instance.m_remoteUpdateHandler += value; }
        remove { if (m_instance != null) m_instance.m_remoteUpdateHandler -= value; }
    }

    public static bool Exists
    {
        get { return m_instance != null; }
    }

    public static ControlScheme PlayerOneControlScheme
    {
        get { return m_instance.m_playerOneScheme; }
    }

    public static ControlScheme PlayerTwoControlScheme
    {
        get { return m_instance.m_playerTwoScheme; }
    }

    public static ControlScheme PlayerThreeControlScheme
    {
        get { return m_instance.m_playerThreeScheme; }
    }

    public static ControlScheme PlayerFourControlScheme
    {
        get { return m_instance.m_playerFourScheme; }
    }

    public static bool AnyInput()
    {
        return AnyInput(m_instance.m_playerOneScheme) || AnyInput(m_instance.m_playerTwoScheme) ||
            AnyInput(m_instance.m_playerThreeScheme) || AnyInput(m_instance.m_playerFourScheme);
    }

    public static bool AnyInput(PlayerID playerID)
    {
        return AnyInput(m_instance.GetControlSchemeByPlayerID(playerID));
    }

    public static bool AnyInput(string schemeName)
    {
        ControlScheme scheme;
        if (m_instance.m_schemeLookup.TryGetValue(schemeName, out scheme))
            return scheme.AnyInput;

        return false;
    }

    public static bool AnyInput(ControlScheme scheme)
    {
        if (scheme != null)
            return scheme.AnyInput;

        return false;
    }

    public static void Reinitialize()
    {
        m_instance.RaiseControlSchemesChangedEvent();
        m_instance.Initialize();
    }

    public static void SetControlScheme(string name, PlayerID playerID)
    {
        PlayerID? playerWhoUsesControlScheme = m_instance.IsControlSchemeInUse(name);

        if (playerWhoUsesControlScheme.HasValue && playerWhoUsesControlScheme.Value != playerID)
        {
            Debug.LogErrorFormat("The control scheme named \'{0}\' is already being used by player {1}", name, playerWhoUsesControlScheme.Value.ToString());
            return;
        }

        if (playerWhoUsesControlScheme.HasValue && playerWhoUsesControlScheme.Value == playerID)
            return;

        ControlScheme controlScheme = null;
        if (m_instance.m_schemeLookup.TryGetValue(name, out controlScheme))
        {
            controlScheme.Reset();
            m_instance.SetControlSchemeByPlayerID(playerID, controlScheme);
            m_instance.RaisePlayerControlsChangedEvent(playerID);
        } else
        {
            Debug.LogError(string.Format("A control scheme named \'{0}\' does not exist", name));
        }
    }

    public static ControlScheme GetControlScheme(string name)
    {
        ControlScheme scheme = null;
        if (m_instance.m_schemeLookup.TryGetValue(name, out scheme))
            return scheme;

        return null;
    }

    public static ControlScheme GetControlScheme(PlayerID playerID)
    {
        return m_instance.GetControlSchemeByPlayerID(playerID);
    }

    public static ControlScheme CreateControlScheme(string name)
    {
        if (m_instance.m_schemeLookup.ContainsKey(name))
        {
            Debug.LogError(string.Format("A control scheme named \'{0}\' already exists", name));
            return null;
        }

        ControlScheme scheme = new ControlScheme(name);
        m_instance.m_controlSchemes.Add(scheme);
        m_instance.m_schemeLookup[name] = scheme;

        return scheme;
    }

    public static bool DeleteControlScheme(string name)
    {
        ControlScheme scheme = GetControlScheme(name);
        if (scheme == null)
            return false;

        m_instance.m_schemeLookup.Remove(name);
        m_instance.m_controlSchemes.Remove(scheme);
        if (m_instance.m_playerOneScheme.Name == scheme.Name)
            m_instance.m_playerOneScheme = null;
        if (m_instance.m_playerTwoScheme.Name == scheme.Name)
            m_instance.m_playerTwoScheme = null;
        if (m_instance.m_playerThreeScheme.Name == scheme.Name)
            m_instance.m_playerThreeScheme = null;
        if (m_instance.m_playerFourScheme.Name == scheme.Name)
            m_instance.m_playerFourScheme = null;

        return true;
    }

    public static void Save()
    {
        string filename = Application.persistentDataPath + "/input_config.xml";
        //Save(new InputSaverXML(filename));
    }

    public static void Load()
    {
        #if UNITY_WINRT && !UNITY_EDITOR
        string filename = Application.persistentDataPath + "/input_config.xml";
        if (UnityEngine.Windows.File.Exists(filename))
        {
            //Load(new InputLoaderXML(filename));
        }
        #else
        string filename = Application.persistentDataPath + "/input_config.xml";
        if (System.IO.File.Exists(filename))
        {
            //Load(new InputLoaderXML(filename));
        }
        #endif
    }

    public static void Load(string filename)
    {
        //Load(new InputLoaderXML(filename));
    }

    #if UNITY_EDITOR
    [DidReloadScripts(0)]
    private static void OnScriptReload()
    {
        if (EditorApplication.isPlaying)
        {
            InputManager[] inputManagers = FindObjectsOfType<InputManager>();
            for (int i = 0; i < inputManagers.Length; i++)
            {
                inputManagers[i].OnInitializeAfterScriptReload();
            }
        }
    }
    #endif

#endregion
}

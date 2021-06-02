using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class GameData : MonoBehaviour
{

    [System.Serializable]
    public class PlayersOn
    {
        public int player;
        public bool active;
    }
    public PlayersOn[] playersOn;
    [SerializeField] public int modeStyle = 0;
    [SerializeField] public int modeReflection = 0;
    [SerializeField] public int defaultStockCount = 1;
    [SerializeField] public int timerSetting = 0;
    [SerializeField] public int defaultInitialHealth = 3000;
    [SerializeField] public int defaultInitialShards = 50;
    [SerializeField] public int defaultTotalShards = 200;
    [SerializeField] public int defaultShardStrength = 150;
    [SerializeField] public float damageRatio = 1.0f;
    [SerializeField] public float barrierRatio = 1.0f;

    /*public enum ControllerTypeSetting
    {
        Default,
        A,
        B,
        C
    }*/

    private int _idCounter = 0;
    public int idCounter
    {
        get { return _idCounter; }
        set { _idCounter = value; }
    }

    [System.Serializable]
    public class UserNameReference
    {
        public string Name;
        public int id;
        public ControllerTypeSetting controllerTypeSetting;

        public UserNameReference(int id, string name)
        {
            this.Name = name;
            this.id = id;
            controllerTypeSetting = ControllerTypeSetting.Default;
        }

        public UserNameReference(int id, string name, ControllerTypeSetting cts)
        {
            this.Name = name;
            this.id = id;
            controllerTypeSetting = cts;
        }
    }
    public UserNameReference[] userNameReference;

    private void Awake()
    {
        LoadGame();
        //playersOn = new bool[4];
        idCounter = (userNameReference.Length > 0) ? userNameReference.Length : 0;
    }

    public void UpdateIdCounter(int i)
    {
        idCounter += i;
    }

    // Use this for initialization
    /*void Start()
    {
        //UpdateGameData(ref modeStyle);
    }*/

    // Update is called once per frame
    /*void Update()
    {
        //Debug.Log("Mode Style:"+modeStyle);
    }*/

    public void UpdateGameData(string mode)
    {
        Debug.Log("Mode Style:" + mode);
    }

    public void UpdateGameData(int mode)
    {
        Debug.Log("Mode Style:" + mode);
    }

    private Save CreateSaveGameObject()
    {
        Save save = new Save();
        
        for (int i = 0; i < save.userNameReference.Length; i++)
        {
            if (i == 0 && i < userNameReference.Length)
            {
                if (userNameReference[i] == null)
                {
                    save.userNameReference[i] = new Save.UserNameReference(0, "Default", ControllerTypeSetting.Default);
                    break;
                }
            }
            if (i < userNameReference.Length)
            {
                if (userNameReference[i] != null)
                {
                    save.userNameReference[i] = new Save.UserNameReference(userNameReference[i].id, userNameReference[i].Name, userNameReference[i].controllerTypeSetting);
                }
                else
                {
                    save.userNameReference[i] = null;
                }
            }
            else
            {
                save.userNameReference[i] = null;
            }
        }

        return save;
    }

    private Save CreateDefaultSave()
    {
        Save save = new Save();

        save.userNameReference[0] = new Save.UserNameReference(0, "Default", ControllerTypeSetting.Default);

        return save;
    }

    public void SaveGame()
    {
        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
    }

    public void LoadGame()
    {
        Debug.Log(Application.persistentDataPath);
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            for (int i = 0; i < save.userNameReference.Length; i++)
            {
                if (save.userNameReference[i] == null)
                {
                    userNameReference = new UserNameReference[i];
                    break;
                }
                if (i == save.userNameReference.Length-1)
                {
                    userNameReference = new UserNameReference[i + 1];
                    break;
                }
            }

            for (int i = 0; i < save.userNameReference.Length; i++)
            {
                if (save.userNameReference[i] != null)
                {
                    if (i < userNameReference.Length)
                    {
                        userNameReference[i] = new UserNameReference(save.userNameReference[i].id, save.userNameReference[i].Name, save.userNameReference[i].controllerTypeSetting);
                    }
                }
                else
                {
                    if (i < userNameReference.Length)
                    {
                        userNameReference[i] = null;
                    }
                }
            }
        } else
        {
            Save save = CreateDefaultSave();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
            bf.Serialize(file, save);
            file.Close();

            bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            save = (Save)bf.Deserialize(file);
            file.Close();
            
            for (int i = 0; i < save.userNameReference.Length; i++)
            {
                if (save.userNameReference[i] == null)
                {
                    userNameReference = new UserNameReference[i];
                    break;
                }
                if (i == save.userNameReference.Length - 1)
                {
                    userNameReference = new UserNameReference[i + 1];
                    break;
                }
            }

            for (int i = 0; i < save.userNameReference.Length; i++)
            {
                if (save.userNameReference[i] != null)
                {
                    if (i < userNameReference.Length)
                    {
                        userNameReference[i] = new UserNameReference(save.userNameReference[i].id, save.userNameReference[i].Name, save.userNameReference[i].controllerTypeSetting);
                    }
                }
                else
                {
                    if (i < userNameReference.Length)
                    {
                        userNameReference[i] = null;
                    }
                }
            }
        }
    }
}

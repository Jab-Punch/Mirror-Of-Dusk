using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuEvent : UnityEvent<System.Object> { }

public class MenuEventManager : MonoBehaviour
{

    private Dictionary<string, MenuEvent> eventDictionary;

    //private static MenuEventManager menuEventManager;

    /*public static MenuEventManager instance
    {
        get
        {
            if (!menuEventManager)
            {
                menuEventManager = FindObjectOfType(typeof(MenuEventManager)) as MenuEventManager;

                if (!menuEventManager)
                {
                    Debug.LogError("There needs to be one active MenuEventManager script on a GameObject in your scene.");
                }
                else
                {
                    menuEventManager.Init();
                }
            }
            return menuEventManager;
        }
    }*/

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, MenuEvent>();
        }
    }

    public void StartListening(string eventName, UnityAction<System.Object> listener)
    {
        MenuEvent thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new MenuEvent();
            thisEvent.AddListener(listener);
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<System.Object> listener)
    {
        if (this == null) return;
        MenuEvent thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, System.Object arg0 = null)
    {
        MenuEvent thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(arg0);
        }
    }
}

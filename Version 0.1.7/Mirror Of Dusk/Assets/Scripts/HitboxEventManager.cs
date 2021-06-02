using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitboxEvent : UnityEvent<System.Object, System.Object, System.Object> { }

public class HitboxEventManager : MonoBehaviour {

    private Dictionary<string, HitboxEvent> eventDictionary;

    private static HitboxEventManager hitboxEventManager;

    public static HitboxEventManager instance
    {
        get
        {
            if (!hitboxEventManager)
            {
                hitboxEventManager = FindObjectOfType(typeof(HitboxEventManager)) as HitboxEventManager;

                if (!hitboxEventManager)
                {
                    Debug.LogError("There needs to be one active HitboxEventManager script on a GameObject in your scene.");
                }
                else
                {
                    hitboxEventManager.Init();
                }
            }
            return hitboxEventManager;
        }
    }

    private void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, HitboxEvent>();
        }
    }
    
    public static void StartListening(string eventName, UnityAction<System.Object, System.Object, System.Object> listener)
    {
        HitboxEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        } else
        {
            thisEvent = new HitboxEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<System.Object, System.Object, System.Object> listener)
    {
        if (hitboxEventManager == null) return;
        HitboxEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, System.Object arg0 = null, System.Object arg1 = null, System.Object arg2 = null)
    {
        HitboxEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(arg0, arg1, arg2);
        }
    }
}

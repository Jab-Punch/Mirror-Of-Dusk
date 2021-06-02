using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Create a one-input generic derivation of UnityEvent that takes a boxed "object"
// as input. The input will need to be "unboxed" at the other end of the line
// either with knowledge of its original type or using its GetType() method.
public class UnityObjectEvent : UnityEvent<GameObject> { }

// Event management script for creating/removing listeners, and triggering.
public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityObjectEvent> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                // If there isn't one to be found, log an error:
                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else // If one was found, assume it wasn't initialised and initialise it.
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {

            eventDictionary = new Dictionary<string, UnityObjectEvent>();
        }
    }

    // Start listening to an event
    public static void StartListening(string eventName, UnityAction<GameObject> listener)
    {
        UnityObjectEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityObjectEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    // Stop listening to an event
    public static void StopListening(string eventName, UnityAction<GameObject> listener)
    {
        if (eventManager == null) return; // In case we've already destroyed our eventManager, avoid exceptions.
        UnityObjectEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    // Trigger an event
    public static void TriggerEvent(string eventName, GameObject argument)
    {
        UnityObjectEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(argument); // Run all listener functions associated with this event.
        }
    }
}


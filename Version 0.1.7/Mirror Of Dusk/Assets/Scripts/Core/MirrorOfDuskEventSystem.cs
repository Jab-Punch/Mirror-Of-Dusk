using System;
using UnityEngine;

public class MirrorOfDuskEventSystem : MonoBehaviour
{
    private const string PATH = "EventSystems/MirrorOfDuskEventSystem";

    private static MirrorOfDuskEventSystem _instance;

    private static void Init()
    {
        if (MirrorOfDuskEventSystem._instance != null)
        {
            return;
        }
        MirrorOfDuskEventSystem._instance = (UnityEngine.Object.Instantiate(Resources.Load("EventSystems/MirrorOfDuskEventSystem")) as GameObject).GetComponent<MirrorOfDuskEventSystem>();
    }

    private void Awake()
    {
        base.useGUILayout = false;
        if (MirrorOfDuskEventSystem._instance == null)
        {
            MirrorOfDuskEventSystem._instance = this;
            base.gameObject.name = base.gameObject.name.Replace("(Clone)", string.Empty);
            return;
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }
}

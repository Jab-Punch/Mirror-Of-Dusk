using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCollisionUpdateManager : MonoBehaviour
{
    public static GameCollisionUpdateManager Instance;

    private void Awake()
    {
        base.useGUILayout = false;
        if (GameCollisionUpdateManager.Instance == null)
        {
            GameCollisionUpdateManager.Instance = this;
            return;
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }

    private void OnDestroy()
    {
        if (GameCollisionUpdateManager.Instance == this)
        {
            GameCollisionUpdateManager.Instance = null;
        }
    }
    
    private void Update()
    {
        if (!ExperimentHitboxManager.initialized)
        {
            return;
        }
        ExperimentHitboxManager.Update();
    }
}

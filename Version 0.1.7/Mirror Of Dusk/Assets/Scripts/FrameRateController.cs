using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateController : MonoBehaviour {

    [SerializeField] public bool vSync = true;
    [SerializeField] public int target = 60;

	// Use this for initialization
	void Awake () {
        if (vSync)
        {
            QualitySettings.vSyncCount = 1;
        } else
        {
            QualitySettings.vSyncCount = 0;
        }
        Application.targetFrameRate = target;
	}
	
	// Update is called once per frame
	void Update () {
		if (Application.targetFrameRate != target)
        {
            Application.targetFrameRate = target;
        }
	}
}

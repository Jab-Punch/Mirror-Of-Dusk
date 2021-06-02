using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlot : MonoBehaviour {
    

    protected GameObject rootMenu;

    private void Awake()
    {
        rootMenu = gameObject.transform.root.gameObject;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

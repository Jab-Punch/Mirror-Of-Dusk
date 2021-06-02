using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayInputMainMenu : DelayInput {

    //Required Components
    MenuInput menuInput;
    MenuSelector menuSelector;
    MoveMenuTrees moveMenuTrees;
    TextColorAlter textColorAlter;

    private bool refreshMenu = false;
    private bool buildHand = false;
    private bool leavingMenu = false;
    private bool moveTheTrees = false;
    private bool treeDirection = false;

    // Use this for initialization
    void Start () {
        menuSelector = gameObject.GetComponent<MenuSelector>();
        menuInput = gameObject.GetComponent<MenuInput>();
        moveMenuTrees = gameObject.GetComponent<MoveMenuTrees>();
        textColorAlter = gameObject.GetComponent<TextColorAlter>();
    }
	
	// Update is called once per frame
	void Update () {
        delayTime--;
        if (delayTime <= 70)
        {
            if (moveTheTrees)
            {
                moveMenuTrees.nowMovingTrees(treeDirection);
                moveTheTrees = false;
            }
        }
        if (delayTime <= 50)
        {
            if (buildHand)
            {
                menuSelector.BuildHand();
                buildHand = false;
            }
        }
        if (delayTime <= 40)
        {
            if (refreshMenu)
            {
                menuSelector.BuildMenuChoices(menuSelector.currentMainMenuMode);
                textColorAlter.StopCoroutine("textAlterColor");
                textColorAlter.StartCoroutine("textAlterColor");
                refreshMenu = false;
            }
        }
        if (delayTime <= 0)
        {
            delayTime = 0f;
            if (!leavingMenu)
            {
                menuInput.enableMenuInput = true;
            }
        }
    }

    //Call this to refresh the menu:
    public void refreshTheMenu()
    {
        refreshMenu = true;
    }

    //Call this confirm is the menu has been refreshed:
    public bool checkRefresh()
    {
        if (refreshMenu)
        {
            return true;
        }
        return false;
    }

    //Call this to allow reconstruction of the "Hand holding shard" sprite:
    public void buildTheHand()
    {
        buildHand = true;
    }

    //Call this to set the status to "Now leaving the menu":
    public void nowLeaving()
    {
        leavingMenu = true;
    }

    //Call this to enable moving of all trees:
    public void startMovingTrees(bool tD)
    {
        treeDirection = tD;
        moveTheTrees = true;
    }
}

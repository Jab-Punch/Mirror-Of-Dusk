using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class SelectUserDeleteNameScreen : YesNoMenuScreen
{
    [Header("Manager Prefabs")]
    public GameObject _nameTextField;

    protected override void Awake()
    {
        menuEventManagerInstance = gameObject.GetComponent<MenuEventManager>();
        setUpMenu();
        initializeHorizontalScreen();
        initializeDeleteUserScreen();
    }

    private void initializeDeleteUserScreen()
    {
        c_playerId = playerId;
        csPlayerGUI = characterSelectManager.csPlayerGUI.transform.GetChild(playerId).GetComponent<CSPlayerGUI>();
        nameTextField = _nameTextField.GetComponent<TextMeshProUGUI>();
    }

    public override void forceCloseMenu()
    {
        resetScrollFrames();
        (selectedRootMenu as SelectUserMenuScreen).unlockMenu();
        gameObject.SetActive(false);
    }

    public void UpdateNameText(string _name)
    {
        nameTextField.text = _name;
    }

    public void DeleteUser()
    {
        (selectedRootMenu as SelectUserMenuScreen).DeleteUser();
        confirmOption();
    }
}

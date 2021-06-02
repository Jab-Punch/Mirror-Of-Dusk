using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class ControlScheme {

    [SerializeField]
    private string m_name;
    [SerializeField]
    private string m_description;
    [SerializeField]
    private bool m_isExpanded;
    [SerializeField]
    private string m_uniqueID;
    /*[SerializeField]
    private List<InputAction> m_actions;*/

    public bool IsExpanded
    {
        get { return m_isExpanded; }
        set { m_isExpanded = value; }
    }

    public string UniqueID
    {
        get { return m_uniqueID; }
        set { m_uniqueID = value; }
    }

    public string Name
    {
        get { return m_name; }
        set
        {
            m_name = value;
            if (Application.isPlaying)
            {
                Debug.LogWarning("You should not change the name of a control scheme at runtime");
            }
        }
    }

    public string Description
    {
        get { return m_description; }
        set { m_description = value; }
    }

    public bool AnyInput
    {
        get
        {
            /*foreach(var action in m_actions)
            {
                if (action.AnyInput)
                {
                    return true;
                }
            }*/

            return false;
        }
    }

    public ControlScheme() : this("New Scheme") { }

    public ControlScheme(string name)
    {
        //m_actions = new List<InputAction>();
        m_name = name;
        m_description = "";
        m_isExpanded = false;
        m_uniqueID = GenerateUniqueID();
    }

    public void Initialize()
    {
        /*foreach(var action in m_actions)
        {
            action.Initialize();
        }*/
    }

    public void Update(float deltaTime)
    {

    }

    public void Reset()
    {

    }

    public static ControlScheme Duplicate(ControlScheme source)
    {
        return Duplicate(source.Name, source);
    }

    public static ControlScheme Duplicate(string name, ControlScheme source)
    {
        ControlScheme duplicate = new ControlScheme();
        duplicate.m_name = name;
        duplicate.m_description = source.m_description;
        duplicate.m_uniqueID = GenerateUniqueID();
        /*duplicate.m_actions = new List<InputAction>();
        foreach (var action in source.m_actions)
        {
            duplicate.m_actions.Add(InputAction.Duplicate(action));
        }*/

        return duplicate;
    }

    private static string GenerateUniqueID()
    {
        return Guid.NewGuid().ToString("N");
    }
}

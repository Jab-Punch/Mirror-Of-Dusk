using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    [System.Serializable]
    public class UserNameReference
    {
        public string Name;
        public int id;
        public ControllerTypeSetting controllerTypeSetting;

        public UserNameReference(int id, string name)
        {
            this.Name = name;
            this.id = id;
            controllerTypeSetting = ControllerTypeSetting.Default;
        }

        public UserNameReference(int id, string name, ControllerTypeSetting cts)
        {
            this.Name = name;
            this.id = id;
            controllerTypeSetting = cts;
        }
    }

    public UserNameReference[] userNameReference = new UserNameReference[50];
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxCollection : MonoBehaviour {

    public static HurtboxCollection instance = null;
    public List<HurtboxOwner> hurtboxes { get; private set; }

    public class HurtboxOwner
    {
        public GameObject owner;
        public Hurtbox hurtbox;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        hurtboxes = new List<HurtboxOwner>();
    }

    public void AddToHurtboxCollection(GameObject gm, Hurtbox hbox)
    {
        HurtboxOwner ho = new HurtboxOwner { owner = gm, hurtbox = hbox };
        hurtboxes.Add(ho);
    }
}

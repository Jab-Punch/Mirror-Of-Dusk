using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollection : MonoBehaviour {

    public static HitboxCollection instance = null;
    public List<HitboxOwner> hitboxes { get; private set; }

    public class HitboxOwner
    {
        public GameObject owner;
        public Hitbox hitbox;
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
        hitboxes = new List<HitboxOwner>();
    }

    public void AddToHitboxCollection(GameObject gm, Hitbox hbox)
    {
        HitboxOwner ho = new HitboxOwner { owner = gm, hitbox = hbox };
        hitboxes.Add(ho);
    }
}

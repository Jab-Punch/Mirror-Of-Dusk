using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    HitboxCollection hitboxCollection;

    [System.Serializable]
    public class HitboxData
    {
        public string name;
        public int duration;
        public int playerId;

        public string shape;
    }

    public HitboxData hitboxData;

    public bool loop { get; private set; }
    public bool collideOkay { get; private set; }

    void Awake()
    {
        hitboxCollection = HitboxCollection.instance;
        hitboxCollection.AddToHitboxCollection(gameObject.transform.root.gameObject, this);
        loop = false;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        PlayBox(hitboxData);
    }

    void OnDisable()
    {
        collideOkay = false;
    }

    // Update is called once per frame
    void Update () {
        collideOkay = true;
    }

    private IEnumerator PlayBox(HitboxData hbData)
    {
        int timer = 0;
        //float delay = 1f / (float)animation.fps;
        int delay = hbData.duration;
        if (hbData.duration == 0)
        {
            loop = true;
        }

        while (timer < delay || loop)
        {
            //timer += Time.deltaTime;
            timer += 1;
            yield return 0f;
        }
        if (timer >= delay)
        {
            this.enabled = false;
            yield return null;
        }

    }
}

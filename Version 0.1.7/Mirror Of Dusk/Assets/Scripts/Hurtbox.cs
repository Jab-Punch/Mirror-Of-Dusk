using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

    HurtboxCollection hurtboxCollection;
    private IHitboxResponder _hitResponder = null;
    private IHurtboxResponder _hurtResponder = null;


    [System.Serializable]
    public class HurtboxData
    {
        public string name;
        public int duration;

        public string shape;
        public string frame_name;
        private SpriteAnimator current_object;

        public SpriteAnimator Current_object
        {
            get { return current_object; }
            set { current_object = value; }
        }
    }

    public HurtboxData hurtboxData;

    public bool permanent = false;
    public bool loop { get; private set; }
    public bool collideOkay { get; private set; }
    public bool linkAnim { get; private set; }

    void Awake()
    {
        hurtboxCollection = HurtboxCollection.instance;
        hurtboxCollection.AddToHurtboxCollection(gameObject.transform.root.gameObject, this);
        loop = false;
        linkAnim = false;
        if (!permanent)
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        StartCoroutine(PlayBox(hurtboxData));
    }

    void OnDisable()
    {
        linkAnim = false;
        collideOkay = false;
    }
	
	// Update is called once per frame
	void Update () {
        collideOkay = true;
	}

    private IEnumerator PlayBox(HurtboxData hbData)
    {
        int timer = 0;
        //float delay = 1f / (float)animation.fps;
        int delay = hbData.duration;
        if (hbData.duration == 0)
        {
            loop = true;
        }

        while ((timer < delay || loop) && ((linkAnim && hurtboxData.Current_object.IsPlaying(hurtboxData.frame_name)) || !linkAnim))
        {
            //timer += Time.deltaTime;
            timer += 1;
            yield return 0f;
        }
        if (timer >= delay)
        {
            gameObject.SetActive(false);
        }

        yield return null;
    }

    public void checkCollision (GameObject gm, Hitbox _hitbox, float x1, float y1)
    {
        if (collideOkay)
        {
            if (hurtboxData.shape == "Circle")
            {
                float rx1 = gm.transform.position.x - (gm.transform.localScale.x / 2);
                float rx2 = gm.transform.localScale.x;
                float ry1 = gm.transform.position.y - (gm.transform.localScale.y / 2);
                float ry2 = gm.transform.localScale.y;
                float cx = gameObject.transform.position.x;
                float cy = gameObject.transform.position.y;
                float cr = gameObject.transform.localScale.x / 2;

                float nearestX = System.Math.Max(rx1, System.Math.Min(cx, rx1 + rx2));
                float nearestY = System.Math.Max(ry1, System.Math.Min(cy, ry1 + ry2));

                float deltaX = cx - nearestX;
                float deltaY = cy - nearestY;

                if ((deltaX * deltaX + deltaY * deltaY) < (cr * cr))
                {
                    //Debug.Log("Responding: " + _responder);
                    _hitResponder?.collisionDetected(_hitbox, this);
                    _hurtResponder?.collisionDetected(_hitbox, this);
                    //return true;
                }
            } else if (hurtboxData.shape == "Square" || hurtboxData.shape == "Rectangle")
            {
                float rx1 = gm.transform.position.x - (gm.transform.localScale.x / 2);
                float rx2 = gm.transform.localScale.x;
                float ry1 = gm.transform.position.y - (gm.transform.localScale.y / 2);
                float ry2 = gm.transform.localScale.y;

                float x2 = gameObject.transform.position.x - (gameObject.transform.localScale.x / 2);
                float y2 = gameObject.transform.position.y - (gameObject.transform.localScale.y / 2);

                /*Debug.Log(x1);
                Debug.Log(y1);
                Debug.Log(gameObject.name + " : " + x2);
                Debug.Log(gameObject.name + " : " + y2);*/

                if ((rx1 + rx2) >= x2 && rx1 <= (x2 + gameObject.transform.localScale.x) && (ry1 + ry2) >= y2 && ry1 <= (y2 + gameObject.transform.localScale.y))
                {
                    _hitResponder?.collisionDetected(_hitbox, this);
                    _hurtResponder?.collisionDetected(_hitbox, this);
                    //return true;
                }
            }
        }
        //return false;
    }

    public void linkAnimation(GameObject gm, string name, bool link)
    {
        if (link)
        {
            linkAnim = true;
            hurtboxData.frame_name = name;
            hurtboxData.Current_object = gm.GetComponent<SpriteAnimator>();
        }
    }

    public void useHitResponder(IHitboxResponder responder)
    {
        _hitResponder = responder;
    }

    public void useHurtResponder(IHurtboxResponder responder)
    {
        _hurtResponder = responder;
    }

}

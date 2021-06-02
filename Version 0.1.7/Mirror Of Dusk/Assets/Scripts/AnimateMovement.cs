using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateMovement : MonoBehaviour {

    [SerializeField] public bool animateOn = true;
    [SerializeField] public bool resetOn = true;
    [SerializeField] bool loopOn = false;
    [SerializeField] public Vector3[] animSequence;
    private Vector3 initPos;
    [SerializeField] public int[] animSpeed;
    [SerializeField] public bool vanish = false;

    private bool animDone = false;

    int frame = 0;
    int renderFrame = 0;

    // Use this for initialization
    void Start()
    {
        initPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        frame = 0;
        renderFrame = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (animateOn)
        {
            if (animSequence != null && animSpeed != null)
            {
                if (!animDone)
                {
                    frame++;
                    if (frame >= animSpeed[renderFrame])
                    {
                        renderFrame++;
                        if (renderFrame >= animSequence.Length)
                        {
                            if (loopOn)
                            {
                                renderFrame = 0;
                            }
                            else
                            {
                                animDone = true;
                            }
                        }
                        frame = 0;
                    }
                    if (!animDone)
                    {
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + animSequence[renderFrame].x, gameObject.transform.position.y + animSequence[renderFrame].y, gameObject.transform.position.z + animSequence[renderFrame].z);
                    }
                }
            }
        } else
        {
            if (resetOn)
            {
                gameObject.transform.position = initPos;
            }
        }
    }

    public void OnEnable()
    {
        /*Color sprColor = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(sprColor.r, sprColor.g, sprColor.b, 1.0f);*/
    }

    public void OnDisable()
    {
        /*Color sprColor = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(sprColor.r, sprColor.g, sprColor.b, 0f);*/
    }

    public void setAnimation(Vector3[] nAnimSeq, int[] nAnimSpeed, bool loop)
    {
        animSequence = nAnimSeq;
        animSpeed = nAnimSpeed;
        loopOn = loop;
        frame = 0;
        renderFrame = 0;
    }
}

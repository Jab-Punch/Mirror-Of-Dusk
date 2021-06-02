using System;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingSprite : AbstractPausableComponent
{
    public ScrollingSprite.Axis axis;

    [SerializeField] bool negativeDirection;
    [SerializeField] private bool onLeft;
    [SerializeField] private bool isRotated;
    [SerializeField] private Material spriteMaterial;

    [Range(0f, 4000f)]
    public float speed;

    [Range(1f, 60f)]
    public int frameSpeed = 1;
    private int currentSpeedFrame = 0; 

    [SerializeField] private float offset;

    [SerializeField]
    [Range(1f, 10f)]
    private int count = 1;

    [NonSerialized]
    public float playbackSpeed = 1f;

    protected float size;
    protected Vector3 pos;
    private float startY;
    protected int direction;

    public enum Axis
    {
        X,
        Y
    }

    public List<SpriteRenderer> copyRenderers { get; private set; }
    
    protected virtual void Start()
    {
        /*Material mater;
        mater = new Material(Shader.Find("Sprites/Default"));*/
        this.copyRenderers = new List<SpriteRenderer>();
        /*Debug.Log(base.transform.GetComponent<SpriteRenderer>().material);
        Debug.Log(mater);*/
        this.direction = ((!this.negativeDirection) ? 1 : -1);
        SpriteRenderer component = base.transform.GetComponent<SpriteRenderer>();
        this.copyRenderers.Add(component);
        //Debug.Log(component.material);
        /*if (mater != null)
        {
            component.material = new Material(Shader.Find("Sprites/Default"));
        }
        Debug.Log(component.material);*/
        this.size = ((this.axis != ScrollingSprite.Axis.X) ? component.sprite.bounds.size.y : component.sprite.bounds.size.x) - this.offset;
        for (int i = 0; i < this.count; i++)
        {
            GameObject gameObject = new GameObject(base.gameObject.name + " Copy");
            gameObject.transform.parent = base.transform;
            gameObject.transform.ResetLocalTransforms();
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerID = component.sortingLayerID;
            spriteRenderer.sortingOrder = component.sortingOrder;
            spriteRenderer.renderingLayerMask = component.renderingLayerMask;
            spriteRenderer.sprite = component.sprite;
            //spriteRenderer.material = component.material;
            spriteRenderer.color = component.color;
            spriteRenderer.maskInteraction = component.maskInteraction;
            this.copyRenderers.Add(spriteRenderer);
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
            gameObject2.transform.parent = base.transform;
            gameObject2.transform.ResetLocalTransforms();
            this.copyRenderers.Add(gameObject2.GetComponent<SpriteRenderer>());
            if (this.axis == ScrollingSprite.Axis.X)
            {
                gameObject.transform.SetLocalPosition(new float?((float)this.direction * (this.size + this.size * (float)i)), new float?(0f), new float?(0f));
                gameObject2.transform.SetLocalPosition(new float?((float)this.direction * -(this.size + this.size * (float)i)), new float?(0f), new float?(0f));
            }
            else
            {
                gameObject.transform.SetLocalPosition(new float?(0f), new float?(this.size + this.size * (float)i), new float?(0f));
                gameObject2.transform.SetLocalPosition(new float?(0f), new float?(-(this.size + this.size * (float)i)), new float?(0f));
            }
        }
        this.startY = base.transform.localPosition.y;
        currentSpeedFrame = 0;
    }
    
    void Update()
    {
        this.pos = base.transform.localPosition;
        currentSpeedFrame++;
        if (currentSpeedFrame >= frameSpeed)
        {
            if (this.axis == ScrollingSprite.Axis.X)
            {
                if (this.pos.x <= -this.size)
                {
                    this.pos.x = this.pos.x + this.size;
                    if (this.isRotated)
                    {
                        this.pos.y = this.startY;
                    }
                }
                if (this.pos.x >= this.size)
                {
                    this.pos.x = this.pos.x - this.size;
                }
                if (!this.isRotated)
                {
                    //this.pos.x = this.pos.x - (float)((!this.negativeDirection) ? 1 : -1) * this.speed * MirrorOfDuskTime.Delta * this.playbackSpeed;
                    this.pos.x = this.pos.x - (float)((!this.negativeDirection) ? 1 : -1) * this.speed * 1f * this.playbackSpeed;
                }
            }
            else
            {
                if (this.pos.y <= -this.size)
                {
                    this.pos.y = this.pos.y + this.size;
                }
                if (this.pos.y >= this.size)
                {
                    this.pos.y = this.pos.y - this.size;
                }
                if (!this.isRotated)
                {
                    //this.pos.y = this.pos.y - (float)((!this.negativeDirection) ? 1 : -1) * this.speed * MirrorOfDuskTime.Delta * this.playbackSpeed;
                    this.pos.y = this.pos.y - (float)((!this.negativeDirection) ? 1 : -1) * this.speed * 1f * this.playbackSpeed;
                }
            }
            if (this.isRotated)
            {
                //this.pos -= base.transform.right * this.speed * MirrorOfDuskTime.Delta;
                this.pos -= base.transform.right * this.speed * 1f;
            }
            base.transform.localPosition = this.pos;
            currentSpeedFrame = 0;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.copyRenderers = null;
    }
}

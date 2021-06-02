using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AbstractCollidableObject : AbstractPausableComponent
{
    [SerializeField] public ColliderType colliderType;
    [SerializeField] public List<ColliderType> opposingColliderTypes;
    [SerializeField] public List<GameCollider> colliderList;

    private List<int> triggersFound;
    private List<int> staysFound;

    public List<int> TriggersFound
    {
        get { return triggersFound; }
    }

    public List<int> StaysFound
    {
        get { return staysFound; }
    }

    protected override void Awake()
    {
        base.Awake();
        if (ExperimentHitboxManager.collidableObjects != null)
            ExperimentHitboxManager.collidableObjects[colliderType].Add(this);
        triggersFound = new List<int>();
        staysFound = new List<int>();
    }

    protected virtual void OnEnable()
    {
        for (int i = staysFound.Count - 1; i >= 0; i--)
        {
            if (!triggersFound.Contains(staysFound[i]))
                staysFound.RemoveAt(i);
        }
        /*if (!triggerFound)
            stayFound = false;
        triggerFound = false;*/
        if (triggersFound.Count > 0)
        {
            for (int i = triggersFound.Count - 1; i >= 0; i--)
            {
                triggersFound.RemoveAt(i);
            }
        }
    }

    protected virtual void Update()
    {
        for (int i = staysFound.Count - 1; i >= 0; i--)
        {
            if (!triggersFound.Contains(staysFound[i]))
                staysFound.RemoveAt(i);
        }
        /*if (!triggerFound)
            stayFound = false;
        triggerFound = false;*/
        if (triggersFound.Count > 0)
        {
            for (int i = triggersFound.Count - 1; i >= 0; i--)
            {
                triggersFound.RemoveAt(i);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (ExperimentHitboxManager.collidableObjects != null)
            ExperimentHitboxManager.collidableObjects[colliderType].Remove(this);
    }

    public virtual void ReviewCollisions(AbstractCollidableObject oppCollider)
    {
        if (!base.gameObject.activeSelf)
        {
            return;
        }
        int oppId = oppCollider.GetInstanceID();
        if (this.colliderType == oppCollider.colliderType && triggersFound.Contains(oppId))
            return;
        for (int i = 0; i < colliderList.Count; i++)
        {
            for (int l = 0; l < oppCollider.colliderList.Count; l++)
            {
                if (colliderList[i].CheckCollision(oppCollider.colliderList[l]))
                {
                    if (!staysFound.Contains(oppId))
                    {
                        this.OnHitboxTriggerEnter(oppCollider);
                        oppCollider.OnHitboxCollisionEnter(this);
                    }
                    /*if (!stayFound)
                    {
                        this.OnHitboxTriggerEnter(oppCollider);
                        oppCollider.OnHitboxCollisionEnter(this);
                    }*/
                    triggersFound.Add(oppCollider.GetInstanceID());
                    //triggerFound = true;
                    this.OnHitboxTriggerStay(oppCollider);
                    oppCollider.OnHitboxCollisionStay(this);
                    if (!staysFound.Contains(oppId))
                        staysFound.Add(oppId);
                    //stayFound = true;
                    if (this.colliderType == oppCollider.colliderType)
                    {
                        oppCollider.TriggersFound.Add(this.GetInstanceID());
                    }
                    return;
                }
            }
        }
        if (staysFound.Contains(oppId))
        {
            this.OnHitboxTriggerExit(oppCollider);
            oppCollider.OnHitboxCollisionExit(this);
        }
        if (staysFound.Contains(oppId))
            staysFound.Remove(oppId);
    }

    public virtual void OnHitboxTriggerEnter(AbstractCollidableObject oppCollider)
    {
        this.checkCollision(oppCollider, CollisionPhase.Enter);
    }

    public virtual void OnHitboxCollisionEnter(AbstractCollidableObject oppCollider)
    {
        this.checkCollision(oppCollider, CollisionPhase.Enter);
    }

    public virtual void OnHitboxTriggerStay(AbstractCollidableObject oppCollider)
    {
        this.checkCollision(oppCollider, CollisionPhase.Stay);
    }

    public virtual void OnHitboxCollisionStay(AbstractCollidableObject oppCollider)
    {
        this.checkCollision(oppCollider, CollisionPhase.Stay);
    }

    public virtual void OnHitboxTriggerExit(AbstractCollidableObject oppCollider)
    {
        this.checkCollision(oppCollider, CollisionPhase.Exit);
    }

    public virtual void OnHitboxCollisionExit(AbstractCollidableObject oppCollider)
    {
        this.checkCollision(oppCollider, CollisionPhase.Exit);
    }

    protected virtual void checkCollision(AbstractCollidableObject oppCollider, CollisionPhase phase)
    {
        ColliderType colType = oppCollider.colliderType;
        GameObject hit = oppCollider.gameObject;
        this.OnCollision(hit, phase);
        if (colType == ColliderType.Wall)
        {
            this.OnCollisionWalls(hit, phase);
        }
        else if (colType == ColliderType.Ground)
        {
            this.OnCollisionGround(hit, phase);
        }
        else if (colType == ColliderType.Character_Hurt)
        {
            this.OnCollisionCharacter(hit, phase);
            oppCollider.OnCollisionCharacter(this.gameObject, phase);
        }
        else if (colType == ColliderType.Projectile_Hurt)
        {
            this.OnCollisionProjectile(hit, phase);
            oppCollider.OnCollisionProjectile(this.gameObject, phase);
        }
        else if (colType == ColliderType.Background_Hurt)
        {
            this.OnCollisionBackground(hit, phase);
            oppCollider.OnCollisionBackground(this.gameObject, phase);
        }
        else if (colType == ColliderType.Cursor_Hurt)
        {
            this.OnCollisionCursor(hit, phase);
            oppCollider.OnCollisionCursor(this.gameObject, phase);
        }
        else if (colType == ColliderType.Other)
        {
            this.OnCollisionOther(hit, phase);
        }
    }

    protected virtual void OnCollision(GameObject hit, CollisionPhase phase) { }
    protected virtual void OnCollisionWalls(GameObject hit, CollisionPhase phase) { }
    protected virtual void OnCollisionGround(GameObject hit, CollisionPhase phase) { }
    protected virtual void OnCollisionCharacter(GameObject hit, CollisionPhase phase) { }
    protected virtual void OnCollisionProjectile(GameObject hit, CollisionPhase phase) { }
    protected virtual void OnCollisionBackground(GameObject hit, CollisionPhase phase) { }
    protected virtual void OnCollisionCursor(GameObject hit, CollisionPhase phase) { }
    protected virtual void OnCollisionOther(GameObject hit, CollisionPhase phase) { }
}

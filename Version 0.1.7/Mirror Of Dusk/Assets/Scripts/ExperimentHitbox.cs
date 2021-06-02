using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentHitbox : MonoBehaviour
{
    [SerializeField] public ExperimentHitboxManager.BoxType hitboxType;
    private float posX;
    private float posY;
    [SerializeField] private float locX;
    [SerializeField] private float locY;
    Collider[] colliders;
    Vector3 position;
    Vector3 boxSize;
    Quaternion rotation;
    //int mask = 1;
    LayerMask mask;
    int uCount = 0;
    int fCount = 0;

    private Vector3 currentPosition
    {
        get { return this.gameObject.transform.position; }
    }

    private void Awake()
    {
        if (ExperimentHitboxManager.hitboxes != null)
            ExperimentHitboxManager.hitboxes.Add(this);
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {
        if (ExperimentHitboxManager.hitboxes != null)
            ExperimentHitboxManager.hitboxes.Remove(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        rotation = new Quaternion();
        mask = LayerMask.GetMask("Default");
    }

    // Update is called once per frame
    void Update()
    {
        position = this.gameObject.transform.position;
        boxSize = new Vector3(position.x / 2, position.y / 2, position.z / 2);
        
        
        colliders = Physics.OverlapBox(position, boxSize, rotation, mask);
        if (colliders.Length > 0)
        {
            Debug.Log("U:" + uCount++);
        }
    }

    private void FixedUpdate()
    {
        position = this.gameObject.transform.position;
        boxSize = new Vector3(position.x / 2, position.y / 2, position.z / 2);
        colliders = Physics.OverlapBox(position, boxSize, rotation, mask);
        if (colliders.Length > 0)
        {
            Debug.Log("F:" + fCount++);
        }
    }

    public void CheckCollision()
    {
        for (int i = 0; i < ExperimentHitboxManager.hurtboxes.Count; i++)
        {
            ExperimentHurtbox _hurtbox = ExperimentHitboxManager.hurtboxes[i];
            float rx1 = _hurtbox.currentPosition.x - (_hurtbox.locX / 2);
            float rx2 = _hurtbox.locX;
            float ry1 = _hurtbox.currentPosition.y - (_hurtbox.locY / 2);
            float ry2 = _hurtbox.locY;

            float x2 = this.currentPosition.x - (this.locX / 2);
            float y2 = this.currentPosition.y - (this.locY / 2);

            if ((rx1 + rx2) >= x2 && rx1 <= (x2 + this.locX) && (ry1 + ry2) >= y2 && ry1 <= (y2 + this.locY))
            {
                /*_hitResponder?.collisionDetected(_hitbox, this);
                _hurtResponder?.collisionDetected(_hitbox, this);*/
                Debug.Log("Hit");
                //return true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(locX * 2, locY * 2, 2)); // Because size is halfExtents

    }
}

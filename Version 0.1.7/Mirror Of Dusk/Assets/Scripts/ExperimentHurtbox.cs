using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentHurtbox : MonoBehaviour
{
    [SerializeField] public ExperimentHitboxManager.BoxType hurtboxType;
    [SerializeField] public float locX;
    [SerializeField] public float locY;

    public Vector3 currentPosition
    {
        get { return this.gameObject.transform.position; }
    }

    private void Awake()
    {
        if (ExperimentHitboxManager.hurtboxes != null)
            ExperimentHitboxManager.hurtboxes.Add(this);
    }

    private void OnDestroy()
    {
        if (ExperimentHitboxManager.hurtboxes != null)
            ExperimentHitboxManager.hurtboxes.Remove(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

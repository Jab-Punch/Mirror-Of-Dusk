using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class AbstractPlayerController : AbstractPausableComponent
{
    private NewPlayerInput _input;
    private PlayerStatsManager _stats;
    //private PlayerCameraController _cameraController;

    private bool _isUnconscious;

    private BoxCollider2D _collider;

    /*public virtual Vector3 center
    {
        get
        {
            if (base.transform == null)
            {
                return Vector3.zero;
            }
            return base.transform.position + this.collider.offset;
        }
    }*/

    public bool IsKOed
    {
        get
        {
            return this._isUnconscious;// && this.stats.Health <= 0;
        }
    }

    protected override void Awake()
    {
        AbstractPlayerController[] array = UnityEngine.Object.FindObjectsOfType<AbstractPlayerController>();
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].name.Contains("PlayerTwo") || array[i].name.Contains("PlayerThree") || array[i].name.Contains("PlayerFour"))
            {
                UnityEngine.Object.Destroy(array[i].gameObject);
            }
        }
        base.Awake();
        //if (Level.Current == null || !Level.Current.PlayersCreated)
        //{
            if (base.gameObject != null)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
            return;
        //}
    }
}

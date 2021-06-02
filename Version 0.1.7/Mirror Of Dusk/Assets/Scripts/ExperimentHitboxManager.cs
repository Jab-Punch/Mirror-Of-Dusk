using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ExperimentHitboxManager
{
    public static bool initialized = false;
    public static List<ExperimentHitbox> hitboxes;
    public static List<ExperimentHurtbox> hurtboxes;
    public static Dictionary<ColliderType, List<AbstractCollidableObject>> collidableObjects;
    private static List<ColliderType> hitColliders;

    public enum BoxType
    {
        Default,
        CharacterSelect
    }

    public static void Awake()
    {
        ExperimentHitboxManager.hitboxes = new List<ExperimentHitbox>();
        ExperimentHitboxManager.hurtboxes = new List<ExperimentHurtbox>();
        ExperimentHitboxManager.collidableObjects = new Dictionary<ColliderType, List<AbstractCollidableObject>>();
        for (int i = 0; i < Enum.GetNames(typeof(ColliderType)).Length; i++)
        {
            collidableObjects.Add((ColliderType)i, new List<AbstractCollidableObject>());
        }
        hitColliders = new List<ColliderType>()
        {
            ColliderType.Default,
            ColliderType.Character,
            ColliderType.Character_Hit,
            ColliderType.Projectile,
            ColliderType.Projectile_Hit,
            ColliderType.Background,
            ColliderType.Background_Hit,
            ColliderType.Other,
            ColliderType.Other_Hit,
            ColliderType.Cursor_Hit
        };
        ExperimentHitboxManager.initialized = true;
    }

    public static void Update()
    {
        /*for (int i = 0; i < ExperimentHitboxManager.hitboxes.Count; i++)
        {
            hitboxes[i].CheckCollision();
        }
        for (int i = 0; i < ExperimentHitboxManager.collidableObjects.Count; i++)
        {
            hitboxes[i].CheckCollision();
        }*/
        foreach (ColliderType col in hitColliders)
        {
            for (int i = 0; i < collidableObjects[col].Count; i++)
            {
                for (int j = 0; j < collidableObjects[col][i].opposingColliderTypes.Count; j++)
                {
                    for (int k = 0; k < collidableObjects[collidableObjects[col][i].opposingColliderTypes[j]].Count; k++)
                    {
                        collidableObjects[col][i].ReviewCollisions(collidableObjects[collidableObjects[col][i].opposingColliderTypes[j]][k]);
                    }
                }
            }
        }
    }


}

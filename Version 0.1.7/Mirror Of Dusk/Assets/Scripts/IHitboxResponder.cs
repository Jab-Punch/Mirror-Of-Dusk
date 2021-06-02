using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxResponder {

    void collisionDetected(Hitbox hitbox, Hurtbox hurtbox);
}

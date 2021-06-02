using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurtboxResponder
{

    void collisionDetected(Hitbox hitbox, Hurtbox hurtbox);
}

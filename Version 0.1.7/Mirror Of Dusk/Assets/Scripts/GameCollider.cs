using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCollider : MonoBehaviour
{
    [SerializeField] public BoxShape boxShape;
    private float posX;
    private float posY;
    [SerializeField] public float offsetX;
    [SerializeField] public float offsetY;
    [SerializeField] public float sizeX;
    [SerializeField] public float sizeY;

    private Vector3 CurrentPosition
    {
        get { return this.gameObject.transform.position; }
    }

    public enum BoxShape
    {
        Square,
        Circle
    }

    public bool CheckCollision(GameCollider collider)
    {
        if (this.boxShape == BoxShape.Square)
        {
            if (collider.boxShape == BoxShape.Square)
            {
                float rx1 = (collider.CurrentPosition.x + collider.offsetX) - (collider.sizeX / 2);
                float rx2 = collider.sizeX;
                float ry1 = (collider.CurrentPosition.y + collider.offsetY) - (collider.sizeY / 2);
                float ry2 = collider.sizeY;

                float x2 = (this.CurrentPosition.x + this.offsetX) - (this.sizeX / 2);
                float y2 = (this.CurrentPosition.y + this.offsetY) - (this.sizeY / 2);

                if ((rx1 + rx2) >= x2 && rx1 <= (x2 + this.sizeX) && (ry1 + ry2) >= y2 && ry1 <= (y2 + this.sizeY))
                {
                    return true;
                }
            } else if (collider.boxShape == BoxShape.Circle)
            {
                float rcx = (collider.CurrentPosition.x + collider.offsetX);
                float rcy = (collider.CurrentPosition.y + collider.offsetY);
                float rcr = collider.sizeX / 2;

                float x2 = (this.CurrentPosition.x + this.offsetX) - (this.sizeX / 2);
                float y2 = (this.CurrentPosition.y + this.offsetY) - (this.sizeY / 2);

                float nearestX = System.Math.Max(x2, System.Math.Min(rcx, x2 + this.sizeX));
                float nearestY = System.Math.Max(y2, System.Math.Min(rcy, y2 + this.sizeY));

                float deltaX = rcx - nearestX;
                float deltaY = rcy - nearestY;

                if ((deltaX * deltaX + deltaY * deltaY) < (rcr * rcr))
                {
                    return true;
                }
            }
        } else if (this.boxShape == BoxShape.Circle)
        {
            if (collider.boxShape == BoxShape.Square)
            {
                float rx1 = (collider.CurrentPosition.x + collider.offsetX) - (collider.sizeX / 2);
                float rx2 = collider.sizeX;
                float ry1 = (collider.CurrentPosition.y + collider.offsetY) - (collider.sizeY / 2);
                float ry2 = collider.sizeY;

                float cx = (this.CurrentPosition.x + this.offsetX);
                float cy = (this.CurrentPosition.y + this.offsetY);
                float cr = this.sizeX / 2;

                float nearestX = System.Math.Max(rx1, System.Math.Min(cx, rx1 + rx2));
                float nearestY = System.Math.Max(ry1, System.Math.Min(cy, ry1 + ry2));

                float deltaX = cx - nearestX;
                float deltaY = cy - nearestY;

                if ((deltaX * deltaX + deltaY * deltaY) < (cr * cr))
                {
                    return true;
                }
            } else if (collider.boxShape == BoxShape.Circle)
            {
                float rcx = (collider.CurrentPosition.x + collider.offsetX);
                float rcy = (collider.CurrentPosition.y + collider.offsetY);
                float rcr = collider.sizeX / 2;

                float cx = (this.CurrentPosition.x + this.offsetX);
                float cy = (this.CurrentPosition.y + this.offsetY);
                float cr = this.sizeX / 2;

                float distX = cx - rcx;
                float distY = cy - rcy;
                float distance = (float)Math.Sqrt( (distX * distX) + (distY * distY));

                if (distance < (cr + rcr))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        if (this.boxShape == BoxShape.Square)
        {
            Gizmos.DrawWireCube(new Vector3(offsetX, offsetY, 0), new Vector3(sizeX, sizeY, 2));
        }
        else if (this.boxShape == BoxShape.Circle)
        {
            Gizmos.DrawWireSphere(new Vector3(offsetX, offsetY, 0), (sizeX / 2));
        }
    }
}

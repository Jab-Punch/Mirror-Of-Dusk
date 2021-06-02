using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VectorPath
{
    [SerializeField]
    private List<Vector3> _points = new List<Vector3>
    {
        new Vector2(-100f, 0f),
        new Vector2(100f, 0f)
    };

    [SerializeField] private bool _closed;
    private float _distance = -1f;

    private List<VectorPath.Node> __infoNodes;

    public struct Node
    {
        public float x;
        public float y;
        public float z;
        public float distance;

        public Node(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.distance = 0f;
        }

        public Vector3 position
        {
            get { return new Vector3(this.x, this.y, this.z); }
        }

        public static List<VectorPath.Node> NewList(List<Vector3> oldList)
        {
            List<VectorPath.Node> list = new List<VectorPath.Node>(oldList.Count);
            for (int i = 0; i < oldList.Count; i++)
            {
                list.Add(new VectorPath.Node(oldList[i]));
            }
            return list;
        }

        public static implicit operator VectorPath.Node(Vector2 v)
        {
            return new VectorPath.Node(v);
        }

        public static implicit operator Vector2(VectorPath.Node t)
        {
            return new Vector2(t.x, t.y);
        }

        public static implicit operator VectorPath.Node(Vector3 v)
        {
            return new VectorPath.Node(v);
        }

        public static implicit operator Vector3(VectorPath.Node t)
        {
            return new Vector3(t.x, t.y, t.z);
        }
    }

    public static Vector3 Lerp(VectorPath path, float t)
    {
        if (path == null || path._points.Count < 1)
        {
            return Vector3.zero;
        }
        if (path._points.Count == 1)
        {
            return path._points[0];
        }
        if (path._points.Count == 2)
        {
            return Vector3.Lerp(path._points[0], path._points[1], t);
        }
        Vector3 vector = default(Vector3);
        int num = 0;
        if (path.Distance < 0f)
        {
            path.Calculate();
        }
        for (int i = 0; i < path.infoNodes.Count - 1; i++)
        {
            num = i;
            if (path.infoNodes[i + 1].distance > t)
            {
                break;
            }
        }
        Vector3 vector2 = path.infoNodes[num];
        Vector3 vector3 = path.infoNodes[num + 1];
        float distance = path.infoNodes[num].distance;
        float distance2 = path.infoNodes[num + 1].distance;
        float t2 = (t - distance) / (distance2 - distance);
        return Vector3.Lerp(path.infoNodes[num], path.infoNodes[num + 1], t2);
    }

    public List<Vector3> Points
    {
        get
        {
            return this._points;
        }
    }

    public bool Closed
    {
        get
        {
            return this._closed;
        }
        set
        {
            this._closed = value;
            this.Calculate();
        }
    }

    public float Distance
    {
        get
        {
            if (this._distance < 0f)
            {
                this.Calculate();
            }
            return this._distance;
        }
    }

    public List<VectorPath.Node> infoNodes
    {
        get
        {
            if (this.__infoNodes == null)
                this.Calculate();
            return this.__infoNodes;
        }
    }

    private void Calculate()
    {
        this.__infoNodes = VectorPath.Node.NewList(this._points);
        if (this._closed)
        {
            this.infoNodes.Add(new VectorPath.Node(this._points[0]));
        }
        this._distance = 0f;
        for (int i = 1; i < this.infoNodes.Count; i++)
        {
            this._distance += Vector3.Distance(this.infoNodes[i - 1], this.infoNodes[i]);
            Debug.Log(this.infoNodes[i - 1]);
            Debug.Log(this.infoNodes[i]);
        }
        float num = 0f;
        for (int j = 1; j < this.infoNodes.Count; j++)
        {
            num += Vector3.Distance(this.infoNodes[j - 1], this.infoNodes[j]);
            VectorPath.Node value = this.infoNodes[j];
            value.distance = num / this._distance;
            this.infoNodes[j] = value;
        }
    }

    public Vector3 Lerp(float t)
    {
        return VectorPath.Lerp(this, t);
    }

    public Vector2 GetClosestPoint(Vector2 originalPosition, Vector2 positionToCheck, bool moveX, bool moveY)
    {
        Vector2 result = originalPosition;
        float num = float.MaxValue;
        Vector2 vector = Vector2.zero;
        Vector2 vector2 = Vector2.zero;
        Vector2 vector3 = Vector2.zero;
        Vector2 vector4 = Vector2.zero;
        Vector2 a = Vector2.zero;
        for (int i = 0; i < this.Points.Count - 1; i++)
        {
            vector2 = this.Points[i];
            vector3 = this.Points[i + 1];
            vector4 = positionToCheck - vector2;
            a = vector3 - vector2;
            if (moveX)
            {
                float num2 = vector4.x / a.x;
                if (num2 < 0f)
                {
                    vector = vector2;
                }
                else if (num2 > 1f)
                {
                    vector = vector3;
                }
                else
                {
                    vector = vector2 + a * num2;
                }
                float num3 = Vector2.Distance(positionToCheck, vector);
                if (num3 <= num)
                {
                    num = num3;
                    result = vector;
                }
            }
            if (moveY)
            {
                float num2 = vector4.y / a.y;
                if (num2 < 0f)
                {
                    vector = vector2;
                }
                else if (num2 > 1f)
                {
                    vector = vector3;
                }
                else
                {
                    vector = vector2 + a * num2;
                }
                float num3 = Vector2.Distance(positionToCheck, vector);
                if (num3 <= num)
                {
                    num = num3;
                    result = vector;
                }
            }
        }
        return result;
    }

    public float GetClosestNormalizedPoint(Vector2 originalPosition, Vector2 positionToCheck, bool moveX, bool moveY)
    {
        Vector2 b = originalPosition;
        float num = float.MaxValue;
        Vector2 vector = Vector2.zero;
        VectorPath.Node node = Vector2.zero;
        VectorPath.Node node2 = Vector2.zero;
        Vector2 vector2 = Vector2.zero;
        Vector2 a = Vector2.zero;
        VectorPath.Node node3 = Vector2.zero;
        VectorPath.Node node4 = Vector2.zero;
        for (int i = 0; i < this.Points.Count - 1; i++)
        {
            node = this.infoNodes[i];
            node2 = this.infoNodes[i + 1];
            vector2 = positionToCheck - (Vector2)node.position;
            a = node2.position - node.position;
            if (moveX)
            {
                float num2 = vector2.x / a.x;
                if (num2 < 0f)
                {
                    vector = node;
                }
                else if (num2 > 1f)
                {
                    vector = node2;
                }
                else
                {
                    vector = node + a * num2;
                }
                float num3 = Vector2.Distance(positionToCheck, vector);
                if (num3 <= num)
                {
                    num = num3;
                    b = vector;
                    node3 = node;
                    node4 = node2;
                }
            }
            if (moveY)
            {
                float num2 = vector2.y / a.y;
                if (num2 < 0f)
                {
                    vector = node;
                }
                else if (num2 > 1f)
                {
                    vector = node2;
                }
                else
                {
                    vector = node + a * num2;
                }
                float num3 = Vector2.Distance(positionToCheck, vector);
                if (num3 <= num)
                {
                    num = num3;
                    b = vector;
                    node3 = node;
                    node4 = node2;
                }
            }
        }
        float num4 = Vector2.Distance(node3.position, node4.position);
        float num5 = Vector2.Distance(node3.position, b);
        return Mathf.Lerp(node3.distance, node4.distance, num5 / num4);
    }
}

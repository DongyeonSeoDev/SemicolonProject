using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGizmos : MonoBehaviour
{
    public Color color;
    public float radious;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radious);
    }
}

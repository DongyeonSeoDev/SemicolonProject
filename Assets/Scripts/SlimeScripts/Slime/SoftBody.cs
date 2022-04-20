using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public abstract class SoftBody : MonoBehaviour
{
    #region Constants
    protected const float splineOffset = 0.5f;
    protected readonly float radius = 0f; // 각 바디포인트 사이의 거리
    #endregion
    [SerializeField]
    protected SpriteShapeController spriteShapeController = null;
    [SerializeField]
    protected List<Transform> points = new List<Transform>();
    public virtual void UpdateVerticies()
    {
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AvoidCloseCheckCollider : MonoBehaviour
{
    protected MiddlePoint middlePoint = null;

    [SerializeField]
    protected LayerMask checkLayer;

    [SerializeField]
    protected float distanceToMiddle = 2f;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!SlimeGameManager.Instance.playerHitCheckDict.ContainsKey(collision.gameObject) && checkLayer.CompareGameObjectLayer(collision.gameObject))
        {
            SlimeGameManager.Instance.playerHitCheckDict.Add(collision.gameObject, false);
        }
    }
    protected void Update()
    {
        List<GameObject> removeList = new List<GameObject>();

        foreach (var item in SlimeGameManager.Instance.playerHitCheckDict)
        {
            if (!item.Key.activeSelf)
            {
                bool hitCheck = item.Value;

                removeList.Add(item.Key);

                if (hitCheck) // ���� ȸ�Ǹ� �������� ����
                {
                    Debug.Log("ȸ�ǽ���!");

                    continue;
                }

                // ȸ�� ����
                Debug.Log("ȸ��!");

                if (SlimeGameManager.Instance.Player.PlayerState.IsInMomentom)
                {
                    EventManager.TriggerEvent("OnAvoidInMomentom");
                }
            }
        }

        foreach (var item in removeList)
        {
            SlimeGameManager.Instance.playerHitCheckDict.Remove(item);
        }
    }

    public void SetMiddlePoint(MiddlePoint middlePoint)
    {
        this.middlePoint = middlePoint;
    }
}

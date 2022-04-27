using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoverCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SlimeGameManager.Instance.Player.CoveredObjectList.Add(collision.gameObject);

        if(collision.gameObject.GetComponent<Enemy.DrainTutorialEnemy>() != null)
        {
            CutsceneManager.Instance.PlayCutscene("TutorialCutscene1");
            //SlimeGameManager.Instance.CurrentPlayerBody.GetComponent<PlayerDrain>().DoDrainByTuto();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        SlimeGameManager.Instance.Player.CoveredObjectList.Remove(collision.gameObject);
    }
}

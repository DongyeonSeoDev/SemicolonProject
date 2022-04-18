using UnityEngine;
using System.Collections.Generic;

public class StageGround : MonoBehaviour
{
    public Collider2D camStageCollider;

    public Transform playerSpawnPoint;

    public Transform objSpawnPos;

    //[SerializeField] private bool autoInsertStageDoors;
    public StageDoor[] stageDoors;

    //[SerializeField] private bool autoInsertPlants;
    public Pick[] plants;

    public Pair<Transform, List<MapParticleEffect>> imprecMapEffects;

    /*  private void Awake()
      {
          AutoSetStageObjs();
      }*/

    /* [ContextMenu("AutoSetStageObjs")]
     public void AutoSetStageObjs()
     {
         if (autoInsertStageDoors)
             stageDoors = GetComponentsInChildren<StageDoor>();
         if (autoInsertPlants)
             plants = GetComponentsInChildren<Pick>();
     }*/

    private void Awake()
    {
        if(imprecMapEffects.first)
        {
            imprecMapEffects.second = imprecMapEffects.first.GetComponentsInChildren<MapParticleEffect>().FindAll(x => x.effectType == MapEffectType.IMPRECATION1);
        }
       
    }

    private void OnEnable()
    {
        for(int i = 0; i < plants.Length; i++)
        {
            plants[i].gameObject.SetActive(true);
        }
    }

    public void OpenDoors()
    {
        for(int i=0; i<stageDoors.Length; i++)
        {
            stageDoors[i].Open();
        }
    }

    public void CloseDoor()
    {
        for (int i = 0; i < stageDoors.Length; i++)
        {
            stageDoors[i].Close();
        }
    }

    public void StageLightActive(bool on)
    {
        stageDoors.ForEach(x => x.DoorLightActive(on));
    }

    public void SetMapEffects()  //나중에 리펙토링 필요할듯. 일단 임시로 ㄱ
    {
       /*bool active;

        if (imprecMapEffects.first != null)
        {
            active = StageManager.Instance.CurrentAreaType == AreaType.IMPRECATION;

            for (int i = 0; i < imprecMapEffects.second.Count; i++)
            {
                imprecMapEffects.second[i].gameObject.SetActive(active);
            }   
        }*/
    }

    public StageDoor GetOpposeDoor(DoorDirType type)
    {
        DoorDirType target = Global.ReverseDoorDir(type);
        for (int i=0; i<stageDoors.Length; i++)
        {
            if (stageDoors[i].dirType == target)
            {
                stageDoors[i].gameObject.SetActive(true);
                stageDoors[i].Pass();
                return stageDoors[i];
            }
        }
        Debug.Log("해당 방향의 반대 방향의 문이 해당 스테이지에 없음");
        return null;
    }
}

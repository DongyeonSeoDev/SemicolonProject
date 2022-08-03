using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class StageGround : MonoBehaviour
{
    public Collider2D camStageCollider;

    public Transform playerSpawnPoint, lobbySpawnPoint;

    public Transform objSpawnPos;

    //[SerializeField] private bool autoInsertStageDoors;
    public StageDoor[] stageDoors;

    //[SerializeField] private bool autoInsertPlants;
    public Pick[] plants;

    #region 적 이동 관련 변수
    [Header("적이 나오는 스테이지면 True")]
    public bool isEnemyStage;

    [Header("적이 지나갈 수 없는 타일맵 넣기 (벽 제외)")]
    public Tilemap[] noPassTilemap;

    [Header("적이 지나갈 수 있는 스테이지 최대 위치, 최소 위치 넣기")]
    public Vector2Int limitMinPosition;
    public Vector2Int limitMaxPosition;
    #endregion

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

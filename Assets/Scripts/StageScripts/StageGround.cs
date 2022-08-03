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

    #region �� �̵� ���� ����
    [Header("���� ������ ���������� True")]
    public bool isEnemyStage;

    [Header("���� ������ �� ���� Ÿ�ϸ� �ֱ� (�� ����)")]
    public Tilemap[] noPassTilemap;

    [Header("���� ������ �� �ִ� �������� �ִ� ��ġ, �ּ� ��ġ �ֱ�")]
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
        Debug.Log("�ش� ������ �ݴ� ������ ���� �ش� ���������� ����");
        return null;
    }
}

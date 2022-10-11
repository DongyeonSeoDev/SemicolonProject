using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private List<NextStageIcon> icons = new List<NextStageIcon>();
    private Dictionary<DoorDirType, NextStageIcon> iconDict = new Dictionary<DoorDirType, NextStageIcon>();

    private void Start()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            iconDict.Add(icons[i].doorDirType, icons[i]);
        }

        StageManager.Instance.StageClearEvent += StageClear;
        EventManager.StartListening("ExitCurrentMap", () =>
        {
            for (int i = 0; i < icons.Count; i++)
            {
                icons[i].gameObject.SetActive(false);
            }
        });
    }

    private void StageClear()
    {
        if (StageManager.Instance.CurrentAreaType == AreaType.START || StageManager.Instance.CurrentAreaType == AreaType.LOBBY) return;

        foreach(StageDoor door in StageManager.Instance.CurrentStageGround.stageDoors)
        {
            if(door.gameObject.activeSelf && !door.IsExitDoor && door.nextStageData)
            {
                iconDict[door.dirType].gameObject.SetActive(true);
                iconDict[door.dirType].SetData(door.nextStageData);
            }
            else
            {
                iconDict[door.dirType].gameObject.SetActive(false);
            }
        }
    }
}

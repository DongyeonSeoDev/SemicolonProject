using System.Collections.Generic;
using UnityEngine;

public class NoDamagedMission : Mission
{
    private int curDamagedCnt;
    private int damagedLimCnt;

    public override void End(bool breakDoor = false)
    {
        
    }

    public override void SetLv(DifficultyLevel lv)
    {
        switch (lv)
        {
            case DifficultyLevel.EASY:
                damagedLimCnt = 3;
                break;
            case DifficultyLevel.NORMAL:
                damagedLimCnt = 1;
                break;
            case DifficultyLevel.HARD:
                damagedLimCnt = 0;
                break;
        }
        missionName = (damagedLimCnt + 1).ToString() + "ȸ �̻� �ǰݴ����� �ʰ� Ŭ�����ϼ���";
    }

    public override void Start()
    {
        curDamagedCnt = 0;
        isEnd = false;
    }

    public override void Update()
    {
        
    }
}

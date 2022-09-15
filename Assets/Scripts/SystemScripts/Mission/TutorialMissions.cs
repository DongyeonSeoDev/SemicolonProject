using UnityEngine;
using Enemy;
using System.Collections.Generic;

//Tutorial Missions
public class AbsorptionTutoMission : Mission
{
    private float checkTime;
    private List<Enemy.Enemy> curEnemyList;

    public AbsorptionTutoMission(string title) : base(title)
    {
        
    }

    public override void End(bool breakDoor = false)
    {
        TutorialManager.Instance.GetSkill2();
        EnemyManager.Instance.isOnlyAbsorption = false;
        EventManager.StopListening("TryAbsorbMob", (System.Action<bool>)TryDrain);
    }

    public override void Start()
    {
        checkTime = Time.time + 1f;
        EnemyManager.Instance.isOnlyAbsorption = true;
        EventManager.StartListening("TryAbsorbMob", (System.Action<bool>)TryDrain);
        base.Start();
    }

    private void TryDrain(bool suc)
    {
        if(!suc)
        {
            TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle("Tuto_FailAbsorb"));
        }
    }

    public override void Update()
    {
        if(checkTime < Time.time)
        {
            if(curEnemyList == null) //바로 적 데이터로 설정되지 않고 약간의 딜레이 후에 설정되기 때문에 이 조건문을 거침
            {
                if (EnemyManager.Instance.enemyDictionary.ContainsKey("Stage0-05"))
                {
                    curEnemyList = EnemyManager.Instance.enemyDictionary["Stage0-05"];
                }
            }

            checkTime = Time.time + 1f;

            if(curEnemyList != null)
            {
                for (int i = 0; i < curEnemyList.Count; i++)
                {
                    curEnemyList[i].GetDamage(2, false, false, false,
                        curEnemyList[i].transform.position, curEnemyList[i].transform.position - Global.GetSlimePos.position);
                }
            }
            
        }
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = lv;
    }
}

public class AbsorptionTutoMission2 : Mission
{

    public AbsorptionTutoMission2(string title) : base(title)
    {

    }

    public override void End(bool breakDoor = false)
    {
        TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle("Tuto_StageClear7"));
        TutorialManager.Instance.GetSkill1();
        EnemyManager.Instance.isOnlyAbsorption = false;
        EventManager.StopListening("TryAbsorbMob", (System.Action<bool>)TryDrain);
        //StageManager.Instance.StageClear();
    }

    public override void Start()
    {
        EnemyManager.Instance.isOnlyAbsorption = true;
        EventManager.StartListening("TryAbsorbMob", (System.Action<bool>)TryDrain);
        base.Start();
    }

    public override void Update()
    {
        
    }

    private void TryDrain(bool suc)
    {
        if (!suc)
        {
            TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle("Tuto_FailAbsorb2"));
        }
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = lv;
    }
}
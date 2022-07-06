using UnityEngine;
using Enemy;
using System.Collections.Generic;

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
        //StageManager.Instance.StageClear();
    }

    public override void Start()
    {
        checkTime = Time.time + 1f;
        EnemyManager.Instance.isOnlyAbsorption = true;
        EventManager.StartListening("TryAbsorbMob", (System.Action<bool>)TryDrain);
    }

    private void TryDrain(bool suc)
    {
        if(!suc)
        {
            TalkManager.Instance.SetSubtitle("����� ���� ü���� ���� ���� ������", 0.15f, 2f);
        }
    }

    public override void Update()
    {
        if(checkTime < Time.time)
        {
            if(curEnemyList == null) //�ٷ� �� �����ͷ� �������� �ʰ� �ణ�� ������ �Ŀ� �����Ǳ� ������ �� ���ǹ��� ��ħ
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
}

public class AbsorptionTutoMission2 : Mission
{

    public AbsorptionTutoMission2(string title) : base(title)
    {

    }

    public override void End(bool breakDoor = false)
    {
        TalkManager.Instance.SetSubtitle("���� �� �������� ����̾�!", 0.15f, 1.5f);
        TutorialManager.Instance.GetSkill1();
        EnemyManager.Instance.isOnlyAbsorption = false;
        EventManager.StopListening("TryAbsorbMob", (System.Action<bool>)TryDrain);
        //StageManager.Instance.StageClear();
    }

    public override void Start()
    {
        EnemyManager.Instance.isOnlyAbsorption = true;
        EventManager.StartListening("TryAbsorbMob", (System.Action<bool>)TryDrain);
    }

    public override void Update()
    {
        
    }

    private void TryDrain(bool suc)
    {
        if (!suc)
        {
            TalkManager.Instance.SetSubtitle("�ѹ� �� �������� ����� ���� ü���� ���� ���� ������", 0.15f, 1.5f);
        }
    }
}
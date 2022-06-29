using UnityEngine;
using Enemy;

public class AbsorptionTutoMission : Mission
{
    private float checkTime;

    public AbsorptionTutoMission(string title) : base(title)
    {
        
    }

    public override void End(bool breakDoor = false)
    {
        StageManager.Instance.StageClear();
        KeyActionManager.Instance.GetElement(InitGainType.SKILL2);
        EnemyManager.Instance.isOnlyAbsorption = false;
    }

    public override void Start()
    {
        checkTime = Time.time + 1f;
        EnemyManager.Instance.isOnlyAbsorption = true;
    }

    public override void Update()
    {
        if(checkTime < Time.time)
        {
            checkTime = Time.time + 1f;
            //적 체력 감소
        }
    }
}

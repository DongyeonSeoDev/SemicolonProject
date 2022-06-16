using UnityEngine;
public class NormalMission : Mission
{
    public NormalMission(short id, string title) : base(id, title)
    {

    }
    public override void End(bool breakDoor = false)
    {
        EventManager.StopListening("StageClear", Clear);
    }

    public override void Start()
    {
        isEnd = false;
        EventManager.StartListening("StageClear", Clear);
    }

    public override void Update()
    {
        
    }

    private void Clear()
    {
        isEnd = true;
    }
}

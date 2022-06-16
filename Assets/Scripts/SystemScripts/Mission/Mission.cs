
public abstract class Mission 
{
    public bool isEnd;
    public MissionType missionType;
    public string missionName;

    public Mission() { }
    public Mission(string title)
    {
        missionName = title;    
    }

    public abstract void Start();
    public abstract void Update();
    public abstract void End(bool breakDoor = false);  //문을 부수고 지나가서 End가 호출된건지
}

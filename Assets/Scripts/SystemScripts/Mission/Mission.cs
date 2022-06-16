
public abstract class Mission 
{
    public bool isEnd;
    public short id;
    public string missionName;

    public Mission() { }
    public Mission(short id, string title)
    {
        missionName = title;    
        this.id = id;
    }

    public abstract void Start();
    public abstract void Update();
    public abstract void End(bool breakDoor = false);  //문을 부수고 지나가서 End가 호출된건지
}

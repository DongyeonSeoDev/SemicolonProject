public class StageManager : MonoSingleton<StageManager>
{
    private bool isStageClear = true;
    
    public bool IsStageClear
    {
        get { return isStageClear; }
        set { isStageClear = value; }
    }
}

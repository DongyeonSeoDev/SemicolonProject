
public class InteractionHandler 
{
    public static bool canTransformEnemy = true; //플레이어가 다른 몬스터로 변신 가능한지
    public static bool canUseQuikSlot = true;  //퀵슬롯 사용 가능한지

    public static void Reset()
    {
        canTransformEnemy = true; 
        canUseQuikSlot = true;  
    }
}

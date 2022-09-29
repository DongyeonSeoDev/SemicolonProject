
public class InteractionHandler 
{
    public static bool canTransformEnemy = true; //플레이어가 다른 몬스터로 변신 가능한지
    public static bool canUseQuikSlot = true;  //퀵슬롯 사용 가능한지
    public static bool canInteractObj = true;  //오브젝트와 상호작용 가능한지
    public static bool isOpenWarningWindow = false; //경고 확인창 떠있는지
    public static bool showHeadAssimBar = true; //머리 위에 뜨는 동화율 바 보여줄지

    public static void Reset()
    {
        canTransformEnemy = true; 
        canUseQuikSlot = true;
        canInteractObj = true;
        isOpenWarningWindow = false;
        showHeadAssimBar = true;
    }
}

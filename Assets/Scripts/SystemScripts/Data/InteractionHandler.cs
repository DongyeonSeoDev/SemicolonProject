
public class InteractionHandler 
{
    public static bool canTransformEnemy = true; //�÷��̾ �ٸ� ���ͷ� ���� ��������
    public static bool canUseQuikSlot = true;  //������ ��� ��������
    public static bool isOpenWarningWindow = false; //��� Ȯ��â ���ִ���

    public static void Reset()
    {
        canTransformEnemy = true; 
        canUseQuikSlot = true;
        isOpenWarningWindow = false;
    }
}


public class InteractionHandler 
{
    public static bool canTransformEnemy = true; //�÷��̾ �ٸ� ���ͷ� ���� ��������
    public static bool canUseQuikSlot = true;  //������ ��� ��������
    public static bool canInteractObj = true;  //������Ʈ�� ��ȣ�ۿ� ��������
    public static bool isOpenWarningWindow = false; //��� Ȯ��â ���ִ���
    public static bool showHeadAssimBar = true; //�Ӹ� ���� �ߴ� ��ȭ�� �� ��������

    public static void Reset()
    {
        canTransformEnemy = true; 
        canUseQuikSlot = true;
        canInteractObj = true;
        isOpenWarningWindow = false;
        showHeadAssimBar = true;
    }
}

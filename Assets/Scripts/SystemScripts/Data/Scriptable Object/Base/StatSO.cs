using UnityEngine;

public abstract class StatSO : ScriptableObject
{
    public ushort statId;  //���� ���̵�
    public bool statValueDecimal;  //������ ���� UI�� ǥ���� �� �Ҽ��� ǥ������
    public int maxStatLv; // ���� �ִ� ����
    public float upStatValue; // Ư�� ���� ����Ʈ�� �������� �� ���� ������ ��
    public string statName; //�� ������ �̸� (�� : ü��, ���ݷ�, ����) 
    public Sprite statSpr;  //���� ������
}

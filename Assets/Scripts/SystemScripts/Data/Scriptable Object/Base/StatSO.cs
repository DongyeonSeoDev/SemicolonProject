using UnityEngine;

public abstract class StatSO : ScriptableObject
{
    public ushort statId;  //���� ���̵�
    public int maxStatLv; // ���� �ִ� ����
    public float upStatValue; // Ư�� ���� ����Ʈ�� �������� �� ���� ������ ��
    public string statName; //�� ������ �̸� (�� : ü��, ���ݷ�, ����) 
    public Sprite statSpr;  //���� ������
}

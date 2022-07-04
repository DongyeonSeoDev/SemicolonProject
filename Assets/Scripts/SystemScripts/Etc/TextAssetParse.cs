using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AssetParse<K,V,T> where T : class  
{
    #region Singleton
    protected static object _lock = new object();
    protected static T instance;

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = Activator.CreateInstance(typeof(T)) as T;
                }

                return instance;
            }
        }
    }
    #endregion

    protected Dictionary<K, V> dictionary;

    protected abstract string Path { get; }

    public abstract void CreateData();
    public abstract V GetValue(K key);
}

//�������� ���̵�� �������� �̸�(Ÿ��Ʋ ȭ�鿡 ���) ������
public class StageInfoTextAsset : AssetParse<string, string, StageInfoTextAsset> //�޸��� ������ �� ���ڵ��� UTF-8��
{
    protected override string Path => "System/TextAssets/StageInfo";

    public override void CreateData()
    {
        dictionary = new Dictionary<string, string>();
        string[] strs = Resources.Load<TextAsset>(Path).text.Split('\n');
        for(int i= 0; i < strs.Length; i++)
        {
            string[] strs2 = strs[i].Split(' ');
            dictionary.Add(strs2[0], strs2[1]);
        }
    }

    public override string GetValue(string key)
    {
        if(dictionary == null)
        {
            CreateData();
        }

        if(dictionary.ContainsKey(key))
            return dictionary[key];

        Debug.Log("Not Exist Key : " + key);
        return "NONE";
    }
}

//���� ���ȷ����� Ű�� ��������� �ش� ������ ����Ű�����ؼ� �ʿ��� ����� ��ȯ��
public class UpStatInfoTextAsset : AssetParse<int, int, UpStatInfoTextAsset>
{
    protected override string Path => "System/TextAssets/StatUpValue";

    private int lastCost;  //���� ��� ��� (������ ������� ���ĺ��ʹ� ���� ���� �ö� ������)
    private int lastKey;  //������ �ε���

    public override void CreateData()
    {
        dictionary = new Dictionary<int, int>();
        string[] strs = Resources.Load<TextAsset>(Path).text.Split('\n');
        for (int i = 0; i < strs.Length; i++)
        {
            dictionary.Add(i+1, int.Parse(strs[i]));  //���� ������ 1�̸� �� ������ �����͸� ������
        }
        lastCost = dictionary[strs.Length];  //i+1�� �����Ƿ� ������ ����� ���̸� Ű�� �ؼ� ������ �� ����
        lastKey = strs.Length;  //�׷��� lastKey�� �ᱹ ���̿� ���� 
    }

    public override int GetValue(int key)
    {
        if (dictionary == null)
        {
            CreateData();
        }

        if (dictionary.ContainsKey(key))
            return dictionary[key];

        if (key > lastKey)
            return lastCost;

        if (key == 0)
            return 10;

        Debug.Log("Not Exist Key : " + key);
        return 999;
    }
}

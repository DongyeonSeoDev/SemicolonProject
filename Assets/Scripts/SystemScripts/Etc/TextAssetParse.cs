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

//스테이지 아이디로 스테이지 이름(타이틀 화면에 띄울) 가져옴
public class StageInfoTextAsset : AssetParse<string, string, StageInfoTextAsset> //메모장 저장할 때 인코딩은 UTF-8로
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

//현재 스탯레벨을 키로 집어넣으면 해당 스탯을 업시키기위해서 필요한 비용이 반환됨
public class UpStatInfoTextAsset : AssetParse<int, int, UpStatInfoTextAsset>
{
    protected override string Path => "System/TextAssets/StatUpValue";

    private int lastCost;  //가장 비싼 비용 (마지막 비용으로 이후부터는 스탯 레벨 올라도 동일함)
    private int lastKey;  //마지막 인덱스

    public override void CreateData()
    {
        dictionary = new Dictionary<int, int>();
        string[] strs = Resources.Load<TextAsset>(Path).text.Split('\n');
        for (int i = 0; i < strs.Length; i++)
        {
            dictionary.Add(i+1, int.Parse(strs[i]));  //스탯 레벨이 1이면 맨 윗줄의 데이터를 가져옴
        }
        lastCost = dictionary[strs.Length];  //i+1씩 했으므로 마지막 비용은 길이를 키로 해서 가져올 수 있음
        lastKey = strs.Length;  //그래서 lastKey는 결국 길이와 같다 
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







#region 주석
//슬라임 대사 정보를 가져온다
/*public class SlimeDialogTextAsset : AssetParse<string, SubtitleData, SlimeDialogTextAsset>
{
    protected override string Path => "System/TextAssets/SlimeDialogsData";

    public override void CreateData()
    {
        dictionary = new Dictionary<string, SubtitleData>();
        string[] strs1 = Resources.Load<TextAsset>(Path).text.Split('$'); //대사 한 세트가 $로 분리되어 있음

        for(int i = 0; i < strs1.Length; i++)
        {
            string[] strs2 = strs1[i].Split('\n');  //대사 한 세트는 \n로 구분되어 있으며 0번째는 아이디
            Debug.Log(strs2.Length);
            SubtitleData data = new SubtitleData();
            data.subData = new SingleSubtitleData[strs2.Length - 2];  //0번째는 아이디고 마지막 데이터와 다음 $사이의 개행 때문에 길이에서 두 개 뺌
            for(int j = 1; j < strs2.Length-1; j++)
            {
                string[] strs3 = strs2[j].Split('|');  //각 대사에 대한 정보는 |로 구분
                int idx = j - 1;
                Debug.Log(idx);
                data.subData[idx] = new SingleSubtitleData();
                Debug.Log(strs3[0]);
                data.subData[idx].dialog = strs3[0];  //대사
                data.subData[idx].secondPerLit = float.Parse(strs3[1]);  //글자당 초
                data.subData[idx].duration = float.Parse(strs3[2]);  //텍스트 지속시간
                data.subData[idx].nextLogInterval = float.Parse(strs3[3]);  //다음 텍스트 딜레이
                data.subData[idx].endActionId = strs3[4];  //텍스트 끝나고 이벤트 키
            }
            
            dictionary.Add(strs2[0].Trim(), data);
        }
    }

    public override SubtitleData GetValue(string key)
    {
        if(dictionary == null)
        {
            CreateData();
        }

        if(dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }
        else
        {
            Debug.LogWarning("존재하지 않는 대사 키 : " + key);
            return new SubtitleData();
        }
    }
}*/
#endregion
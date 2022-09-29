using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;

public class GameRecord : MonoBehaviour
{
    public TextMeshProUGUI gameResultTMP;  //모험 성공/실패

    public VertexGradient clearVG, failVG;

    public Text playTimeTxt;  //플레이 타임
    public Text KillTxt;  //적 죽인 횟수 (보스 포함)
    public Text charCountTxt;  //얻은 특성 수 (판매한 것도 포함)
    public Text expTxt;  //획득한 총 스탯포인트 경험치 (몹 처치와 미션 완료 보상)

    public Text restStatPointTxt;

    private float playTime;
    private int killCount;
    private int charCount;
    private float exp;

    private Dictionary<ushort, bool> checkCharDic = new Dictionary<ushort, bool>();

    private void Awake()
    {
        Restart();
    }

    private void Update()
    {
        playTime += Time.deltaTime;
    }

    public void KillEnemy() => killCount++;
    public void GetStatPointExp(float value) => exp += value;

    public void CheckGetChar(ushort id)
    {
        if (!checkCharDic[id])
        {
            checkCharDic[id] = true;
            charCount++;
        }
    }

    public void Restart()
    {
        playTime = 0f;
        killCount = 0;
        charCount = 0;
        exp = 0f;

        for (ushort i = NGlobal.CStatStartID; i <= NGlobal.CStatEndID; i += NGlobal.StatIDOffset)
        {
            checkCharDic[i] = false;
        }
    }

    private void Record()
    {
        int s = (int)playTime % 60;
        int m = ((int)playTime / 60) % 60;
        int h = (int)playTime / 3600;

        if(h > 99) 
        {
            playTimeTxt.text = "00:00:00";
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(h.ToString("00"));
            sb.Append(":");
            sb.Append(m.ToString("00"));
            sb.Append(":");
            sb.Append(s.ToString("00"));
            playTimeTxt.text = sb.ToString();
        }

        KillTxt.text = killCount.ToString();
        charCountTxt.text = charCount.ToString();
        expTxt.text = ((int)exp).ToString();

        restStatPointTxt.text = GameManager.Instance.savedData.userInfo.playerStat.currentStatPoint.ToString();
    }

    public void EndGame(bool clear)
    {
        gameResultTMP.text = clear ? "모험 성공" : "모험 실패";
        gameResultTMP.colorGradient = clear ? clearVG : failVG;

        Record();

        UIManager.Instance.OnUIInteractSetActive(UIType.ENDGAME, true, true);
    }
}

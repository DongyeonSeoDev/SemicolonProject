using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileControl : MonoSingleton<PlayerProjectileControl>
{
    private Dictionary<int, List<PlayerProjectile>> projectileListDict = new Dictionary<int, List<PlayerProjectile>>();
    private Dictionary<int, int> totalNumDict = new Dictionary<int, int>();
    private Dictionary<int, int> onAttackEnemyDict = new Dictionary<int, int>();

    private List<int> removeList = new List<int>();

    private void Update()
    {
        CheckProjectiles();
    }
    private void CheckProjectiles()
    {
        int key = 0;
        foreach(var item in projectileListDict)
        {
            key = item.Key;
            if(totalNumDict[key] >= item.Value.Count)
            {

                if(onAttackEnemyDict[key] > 0)
                {
                    EventManager.TriggerEvent("OnEnemyAttack");
                }
                else
                {
                    EventManager.TriggerEvent("OnAttackMiss");
                }

                removeList.Add(key);
            }
        }

        RemoveDict();
    }
    private void RemoveDict()
    {
        int key = 0;
        for (int i = 0; i < removeList.Count; i++)
        {
            key = removeList[i];

            projectileListDict.Remove(key);
            totalNumDict.Remove(key);
            onAttackEnemyDict.Remove(key);
        }

        removeList.Clear();
    }
    public void AddListDict(int key, PlayerProjectile playerProjectile)
    {
        if(projectileListDict.ContainsKey(key))
        {
            projectileListDict[key].Add(playerProjectile);
        }
        else
        {
            List<PlayerProjectile> list = new List<PlayerProjectile>();
            list.Add(playerProjectile);

            projectileListDict.Add(key, list);
            totalNumDict.Add(key, 0);
            onAttackEnemyDict.Add(key, 0);
        }
    }
    private void TotalNumUp(int key)
    {
        totalNumDict[key]++;
    }
    public void OnEnemyAttack(int key)
    {
        TotalNumUp(key);
        onAttackEnemyDict[key]++;
    }
    public void OnMissAttack(int key)
    {
        TotalNumUp(key);
    }
}

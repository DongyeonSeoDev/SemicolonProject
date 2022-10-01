using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public struct EnemySpawnEffectPosition
    {
        public Type enemyType;
        public Vector2 spawnPosition;
        public Vector3 spawnScale;
    }

    public class EnemySpawn : MonoSingleton<EnemySpawn>
    {
#if UNITY_EDITOR
        public List<EnemySpawnData> enemySpawnData = new List<EnemySpawnData>();
        public Queue<EnemySpawnData> addSpawnDataQueue = new Queue<EnemySpawnData>();
        public Queue<EnemySpawnData> removeSpawnDataQueue = new Queue<EnemySpawnData>();
#endif

        public List<EnemySpawnEffectPosition> effectPositionList = new List<EnemySpawnEffectPosition>();

        public Dictionary<string, List<Enemy>> enemyDictionary = new Dictionary<string, List<Enemy>>();

        public Vector3 effectRotationEuler = new Vector3(75f, 0f, 0f);
        private Quaternion effectQuaternion;

        private Dictionary<Type, Vector2> enemySpawnEffectPositionDic = new Dictionary<Type, Vector2>();
        private Dictionary<Type, Vector3> enemySpawnEffectScaleDic = new Dictionary<Type, Vector3>();

        private void Start()
        {
            effectQuaternion = Quaternion.Euler(effectRotationEuler);
            enemyDictionary = EnemyManager.Instance.enemyDictionary;
            enemySpawnEffectPositionDic = effectPositionList.ToDictionary(x => x.enemyType, x => x.spawnPosition);
            enemySpawnEffectScaleDic = effectPositionList.ToDictionary(x => x.enemyType, x => x.spawnScale);

            PlayerRespawnEvent();

            EventManager.StartListening("AfterPlayerRespawn", PlayerRespawnEvent);
            EventManager.StartListening("SpawnEnemy", SpawnEnemy);
            EventManager.StartListening("PlayerDead", StopAllCoroutines);
            EventManager.StartListening("ExitCurrentMap", StopAllCoroutines);

            CSVEnemySpawn.Instance.GetData();
        }

        private void PlayerRespawnEvent()
        {
            EnemyManager.Instance.PlayerDeadEvent();
        }

        private void SpawnEnemy(string stageId)
        {
            var spawnData = CSVEnemySpawn.Instance.enemySpawnDatas;

            if (!spawnData.ContainsKey(stageId))
            {
                Debug.LogError("잘못된 stageId 입니다. SpawnEnemy 실행 실패 : " + stageId);
            }

            for (int i = 0; i < spawnData[stageId].Count; i++)
            {
                EnemySpawnData data = spawnData[stageId][i];

                if (enemySpawnEffectPositionDic.ContainsKey(data.enemyId))
                {
                    EnemyPoolData effect = EnemyPoolManager.Instance.GetPoolObject(Type.EnemySpawnEffect, (Vector2)data.position + enemySpawnEffectPositionDic[data.enemyId]);
                    effect.transform.localScale = enemySpawnEffectScaleDic[data.enemyId];
                    effect.transform.rotation = effectQuaternion;

                    effect.GetComponent<EnemySpawnEffect>().Play();
                }
            }

            for (int i = 0; i < spawnData[stageId].Count; i++) // 보스 라면 대기 없이 바로 소환후 움직임
            {
                if (spawnData[stageId][i].enemyId == Type.Boss_SkeletonKing_50 || spawnData[stageId][i].enemyId == Type.Boss_Centipede)
                {
                    Spawn(spawnData[stageId], stageId);
                    Move(stageId);

                    switch (spawnData[stageId][i].enemyId) //Type과 EnemyType에서 보스는 다르게 적혀있어서 이렇게 함
                    {
                        case Type.Boss_SkeletonKing_50:
                            MonsterCollection.Instance.CheckRecordedMonsters(EnemyType.Boss_SkeletonKing_01.ToString());
                            break;
                        case Type.Boss_Centipede:
                            MonsterCollection.Instance.CheckRecordedMonsters(EnemyType.Boss_Centipede_02.ToString());
                            break;
                    }

                    return;
                }
            }

            StartCoroutine(SpawnCoroutine(spawnData[stageId], stageId));
        }

        private IEnumerator SpawnCoroutine(List<EnemySpawnData> spawnList, string stageId)
        {
            yield return new WaitForSeconds(2.5f);
            Spawn(spawnList, stageId);
            yield return new WaitForSeconds(1f);
            MonsterCollection.Instance.CheckRecordedMonsters(spawnList);
            for (int i = 0; i < enemyDictionary[stageId].Count; i++)
            {
                Move(stageId);
            }
        }

        private void Spawn(List<EnemySpawnData> spawnData, string stageId)
        {
            Debug.Log(spawnData[0].enemyId);
            for (int i = 0; i < spawnData.Count; i++)
            {
                EnemyPoolData enemy = EnemyPoolManager.Instance.GetPoolObject(spawnData[i].enemyId, spawnData[i].position);

                if (enemyDictionary.ContainsKey(stageId))
                {
                    enemyDictionary[stageId].Add(enemy.GetComponent<Enemy>());
                }
                else
                {
                    enemyDictionary.Add(stageId, new List<Enemy>());
                    enemyDictionary[stageId].Add(enemy.GetComponent<Enemy>());
                }

                enemy.GetComponent<Enemy>().SetColor(1f);
            }

            EnemyManager.Instance.enemyCount += spawnData.Count;
            EventManager.TriggerEvent("EnemySpawnAfter");
        }

        private void Move(string stageId)
        {
            for (int i = 0; i < enemyDictionary[stageId].Count; i++)
            {
                enemyDictionary[stageId][i].MoveEnemy();
            }
        }
    }
}
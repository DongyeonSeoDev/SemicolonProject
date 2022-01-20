using System.Collections.Generic;
using UnityEngine;

namespace Water
{
    public class PoolManager
    {
        public static Dictionary<string, ObjPool> poolDic = new Dictionary<string, ObjPool>();

        public static void CreatePool(GameObject prefab, Transform parent, int count, string key = "")
        {
            string k = key == "" ? prefab.name : key;
            if (!poolDic.ContainsKey(k))
            {
                poolDic.Add(k, new ObjPool(prefab, parent, count));
            }
            else
            {
                Debug.Log("풀 생성 실패 : " + k);
            }
        }

        public static GameObject GetItem(string key) => poolDic[key].GetItem();

        public static void ClearPool()
        {
            poolDic.Clear();
        }
    }

    public class ObjPool
    {
        private Queue<GameObject> queue = new Queue<GameObject>();
        private Transform parent;
        private GameObject prefab;

        public ObjPool(GameObject prefab, Transform parent, int count)
        {
            this.prefab = prefab;
            this.parent = parent;
            for (int i = 0; i < count; i++)
            {
                GameObject o = GameObject.Instantiate(prefab, parent);
                o.SetActive(false);
                queue.Enqueue(o);
            }
        }

        public GameObject GetItem()
        {
            GameObject o = queue.Peek();
            if (o.activeSelf)
            {
                o = GameObject.Instantiate(prefab, parent);
            }
            else
            {
                o = queue.Dequeue();
                o.SetActive(true);
            }

            queue.Enqueue(o);
            return o;
        }

    }
    
}
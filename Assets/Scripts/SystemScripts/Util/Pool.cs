using System.Collections.Generic;
using UnityEngine;
using System;

namespace Water
{
    public static class PoolManager
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
        public static T GetItem<T>(string key) => poolDic[key].GetItem<T>();

        public static void PoolObjSetActiveFalse(string key, Action<GameObject> action = null)
        {
            if (poolDic.ContainsKey(key))
            {
                if(action == null) poolDic[key].PoolSetActiveFalse();
                else poolDic[key].PoolSetActiveFalse(action);
            }
                
        }
        public static void PoolObjSetActiveFalse(string key, Func<GameObject,bool> func)
        {
            if (poolDic.ContainsKey(key))
            {
                poolDic[key].PoolSetActiveFalse(func);
            }

        }
        public static void PoolObjSetActiveFalse<T>(string key, Func<T, bool> func)
        {
            if (poolDic.ContainsKey(key))
            {
                poolDic[key].PoolSetActiveFalse<T>(func);
            }

        }
        /*public static void PoolObjSetActiveFalse(string[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (poolDic.ContainsKey(keys[i]))
                    poolDic[keys[i]].PoolSetActiveFalse();
            }
        }*/

        public static bool IsContainKey(string key) => poolDic.ContainsKey(key);

        public static void ClearPool(string key, bool destroy = false)
        {
            if (poolDic.ContainsKey(key))
            {
                if(destroy)
                {
                    poolDic[key].DestroyAllObj();
                }
                poolDic.Remove(key);
            }
        }

        public static void ClearAllPool(bool destroy = false)
        {
            if (destroy)
            {
                foreach (string key in poolDic.Keys)
                    poolDic[key].DestroyAllObj();
            }

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

        public T GetItem<T>()
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
            return o.GetComponent<T>();
        }

        public void PoolSetActiveFalse()
        {
            queue.ForEach(x => { if (x.activeSelf) x.SetActive(false); });
        }

        public void PoolSetActiveFalse(Action<GameObject> action)
        {
            queue.ForEach(x => { 
                if (x.activeSelf)
                {
                    action(x);
                    x.SetActive(false);
                }
            });
        }

        public void PoolSetActiveFalse(Func<GameObject,bool> func)
        {
            queue.ForEach(x => {
                if (x.activeSelf && func(x))
                {
                    x.SetActive(false);
                }
            });
        }

        public void PoolSetActiveFalse<T>(Func<T, bool> func)
        {
            queue.ForEach(x => {
                if (x.activeSelf && func(x.GetComponent<T>()))
                {
                    x.SetActive(false);
                }
            });
        }

        public void DestroyAllObj()
        {
            while(queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();
                GameObject.Destroy(obj);
            }
        }
    }
    
}
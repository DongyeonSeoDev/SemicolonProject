using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePoolManager : MonoBehaviour
{
    private static SlimePoolManager instance = null;
    public static SlimePoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                FindObjectOfType<SlimePoolManager>();

                if (instance == null)
                {
                    GameObject temp = new GameObject("SlimePoolManager");

                    instance = temp.AddComponent<SlimePoolManager>();
                }
            }

            return instance;
        }
    }

    private Dictionary<string, Queue<GameObject>> dictionary = new Dictionary<string, Queue<GameObject>>();

    public void AddObject(GameObject targetObject)
    {
        string key = targetObject.name;

        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, new Queue<GameObject>());
        }

        dictionary[key].Enqueue(targetObject);
    }
    public (GameObject, bool) Find(GameObject targetObject)
    {
        string key = targetObject.name + "(Clone)";

        if (dictionary.ContainsKey(key))
        {
            if (dictionary[key].Count > 0)
            {
                GameObject obj = dictionary[key].Dequeue();

                return (obj, true);
            }

            return (null, false);
        }

        return (null, false);
    }
}

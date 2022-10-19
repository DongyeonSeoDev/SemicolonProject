using UnityEngine;

public class ObjectSave : MonoBehaviour
{
    [SerializeField] protected string objKey;

    public virtual void Awake()
    {
        if (string.IsNullOrEmpty(objKey)) objKey = name;

        StoredData.SetGameObjectKey(objKey, gameObject);
    }
}

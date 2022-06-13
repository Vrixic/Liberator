using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static Dictionary<string, ObjectPool> m_ObjectPools =  new Dictionary<string, ObjectPool>();

    private static ObjectPoolManager m_ObjectPoolManager;
    public static ObjectPoolManager Instance
    {
        get { return m_ObjectPoolManager; }
        private set { m_ObjectPoolManager = value; }
    }

    private void Awake()
    {
        m_ObjectPoolManager = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public string CreateObjectPool(PoolableObject objectPrefab, int size)
    {
        if (!m_ObjectPools.ContainsKey(objectPrefab.name + " Pool"))
        {
            // Create a new object pool
            ObjectPool pool = new ObjectPool(objectPrefab, size);
            m_ObjectPools.Add(objectPrefab.name + " Pool", pool);
        }

        return objectPrefab.name + " Pool";
    }

    public PoolableObject SpawnObject(string poolName)
    {
       return m_ObjectPools[poolName].SpawnObject();
    }
    public List<PoolableObject> GetObjectsInPool(string poolName)
    {
        return m_ObjectPools[poolName].GetAllObjectsInPool();
    }

    //public void DisableAllInPool(string poolName)
    //{
    //    m_ObjectPools[poolName].DisableObjInPool();
    //}
}

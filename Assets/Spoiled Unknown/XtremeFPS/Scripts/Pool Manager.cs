using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XtremeFPS.PoolingSystem
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Pool Manager")]
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }
    
        [System.Serializable]
        public class ObjectPoolItem
        {
            public GameObject objectToPool;
            public int amountToPool;
            public bool shouldExpand;
            public bool shouldRecycle;
        }
    
        public List<ObjectPoolItem> itemsToPool;
        private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
        private Dictionary<GameObject, GameObject> prefabParents = new Dictionary<GameObject, GameObject>();
        private int lastRecycledIndex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            InitializeObjectPools();
        }
    
        private void InitializeObjectPools()
        {
            foreach (ObjectPoolItem item in itemsToPool)
            {
                GameObject parentGameObject = new GameObject(item.objectToPool.name + " Pool");
                parentGameObject.transform.parent = transform;

                List<GameObject> objectPool = new List<GameObject>();
                for (int i = 0; i < item.amountToPool; i++)
                {
                    GameObject obj = Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    obj.transform.parent = parentGameObject.transform;
                    objectPool.Add(obj);
                }
                pooledObjects.Add(item.objectToPool, objectPool);
                prefabParents.Add(item.objectToPool, parentGameObject);
            }
        }

        public GameObject GetPooledObject(GameObject objectToPool, Vector3 position, Quaternion rotation)
        {
            if (pooledObjects.ContainsKey(objectToPool))
            {
                List<GameObject> objectPool = pooledObjects[objectToPool];
                foreach (GameObject obj in objectPool)
                {
                    if (!obj.activeInHierarchy)
                    {
                        obj.transform.SetPositionAndRotation(position, rotation);
                        obj.SetActive(true);
                        return obj;
                    }
                }

                ObjectPoolItem item = FindObjectPoolItem(objectToPool);
                if (item != null && item.shouldExpand)
                {
                    GameObject newObj = Instantiate(objectToPool, position, rotation);
                    GameObject parentGameObject = prefabParents[objectToPool];
                    newObj.transform.parent = parentGameObject.transform;
                    newObj.SetActive(true);
                    objectPool.Add(newObj);
                    return newObj;
                }

                if (item != null && item.shouldRecycle)
                {
                    // Cycle through the pool to recycle objects
                    for (int i = lastRecycledIndex + 1; i < objectPool.Count; i++)
                    {
                        GameObject recycledObj = objectPool[i];
                        if (recycledObj.activeInHierarchy)
                        {
                            recycledObj.transform.SetPositionAndRotation(position, rotation);
                            recycledObj.SetActive(true);
                            lastRecycledIndex = i;
                            return recycledObj;
                        }
                    }
                    // If no inactive objects are found, loop back to the beginning of the pool
                    for (int i = 0; i < lastRecycledIndex; i++)
                    {
                        GameObject recycledObj = objectPool[i];
                        if (recycledObj.activeInHierarchy)
                        {
                            recycledObj.transform.SetPositionAndRotation(position, rotation);
                            recycledObj.SetActive(true);
                            lastRecycledIndex = i;
                            return recycledObj;
                        }
                    }
                }
            }
            return null;
        }


        private ObjectPoolItem FindObjectPoolItem(GameObject objectToPool)
        {
            foreach (ObjectPoolItem item in itemsToPool)
            {
                if (item.objectToPool == objectToPool)
                {
                    return item;
                }
            }
            return null;
        }

        public void ReturnObjectToPool(GameObject obj)
        {
            bool foundInPool = false;

            foreach (var objectPool in pooledObjects.Values)
            {
                if (objectPool.Contains(obj))
                {
                    obj.SetActive(false);
                    foundInPool = true;
                    break;
                }
            }

            if (!foundInPool)
            {
                Debug.LogWarning("The object to return to pool is not managed by the object pool system.");
            }
        }
    }
}
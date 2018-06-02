using System.Collections.Generic;
using UnityEngine;
// ReSharper disable CheckNamespace

namespace EzPool
{
    [DisallowMultipleComponent]
    public class EzPoolManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject pooledPrefab = null;

        // Max number of objects that can be pooled.
        [SerializeField]
        private int maxPoolCount = 1;

        // Number of prefab instantiations to be made on start.
        [SerializeField]
        private uint prePoolCount = 0;

        // List of active objects (Enabled in scene and being used).
        [SerializeField]
        private List<GameObject> activeObjects;

        // List of inactive objects (Disabled in scene and available to unpool).
        [SerializeField]
        private List<GameObject> inactiveObjects;


        private void Start()
        {
            activeObjects = new List<GameObject>(maxPoolCount);
            inactiveObjects = new List<GameObject>(maxPoolCount);

            if(pooledPrefab != null)
            {
                PrewarmPool();
            }
        }

        /// <summary>
        /// Gets the next available inactive object reference in the pool. If none are available, a new object will be added
        /// to the pool if the current pool count hasn't exceeded the max pool count allowed.
        /// </summary>
        /// <returns></returns>
        public GameObject GetAvailable()
        {
            var inactiveCount = inactiveObjects.Count;

            if (inactiveCount == 0)
            {
                var obj = CreateNew();

                if (obj == null)
                {
                    return null;
                }

                activeObjects.Add(obj);
                obj.SetActive(true);
                return obj;
            }
            else
            {
                var obj = inactiveObjects[inactiveCount - 1];
                obj.SetActive(true);
                activeObjects.Add(obj);
                inactiveObjects.RemoveAt(inactiveCount - 1);
                return obj;
            }
        }

        /// <summary>
        /// Puts given object back into the inactive pool ready for use again later on. The object given must exist inside the pool for this manager.
        /// </summary>
        /// <param name="_obj"></param>
        public void PoolObject(GameObject _obj)
        {
            if (_obj == null) return;

            if (activeObjects.Contains(_obj))
            {
                _obj.SetActive(false);
                inactiveObjects.Add(_obj);
                activeObjects.Remove(_obj);
            }
        }

        /// <summary>
        /// Returns the number of total pooled objects (Both active and inactive).
        /// </summary>
        /// <returns></returns>
        public int GetPoolCount()
        {
            return activeObjects.Count + inactiveObjects.Count;
        }

        /// <summary>
        /// Clears the pool manager.
        /// </summary>
        /// <param name="_destroySceneObjects">Should the objects in the scene also be destroyed?</param>
        public void ClearPool(bool _destroySceneObjects)
        {
            if (_destroySceneObjects)
            {
                for (var i = activeObjects.Count; i > 0; i--)
                {
                    Destroy(activeObjects[i]);
                }

                for (var i = inactiveObjects.Count; i > 0; i--)
                {
                    Destroy(inactiveObjects[i]);
                }
            }
          
            activeObjects.Clear();
            inactiveObjects.Clear();
        }

        private GameObject CreateNew()
        {
            if (GetPoolCount() == maxPoolCount)
            {
                //Debug.LogWarning(string.Format("Failed to create new instance as the pool has reached its assigned limit of {0}.", maxPoolCount));
                return null;
            }

            var obj = Instantiate(pooledPrefab, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);

            return obj;
        }

        private void PrewarmPool()
        {
            if (prePoolCount > 0)
            {
                for (var i = 0; i < prePoolCount; i++)
                {
                    var obj = CreateNew();

                    if (obj == null)
                    {
                        break;
                    }

                    inactiveObjects.Add(obj);
                }
            }
        }

        //private void TestUnPool()
        //{
        //    for(var i = 0; i < maxPoolCount; i++)
        //    {
        //        GetAvailable();
        //    }
        //}

        //private void TestPool()
        //{
        //    for(var i = activeObjects.Count - 1; i >= 0; i--)
        //    {
        //        PoolObject(activeObjects[i]);
        //    }
        //}

        //private void Update()
        //{
        //    if(Input.GetKeyDown(KeyCode.Q))
        //    {
        //        var obj = GetAvailable();
        //        if (obj == null) return;
        //        obj.transform.position = Vector3.one * Random.Range(4.0f, 12.0f);
        //    }

        //    if(Input.GetKeyDown(KeyCode.E))
        //    {
        //        PoolObject(activeObjects.Count > 0 ? activeObjects[0] : null);
        //    }

        //    if(Input.GetKeyDown(KeyCode.W))
        //    {
        //        TestUnPool();
        //    }

        //    if(Input.GetKeyDown(KeyCode.S))
        //    {
        //        TestPool();
        //    }
        //}
    }
}


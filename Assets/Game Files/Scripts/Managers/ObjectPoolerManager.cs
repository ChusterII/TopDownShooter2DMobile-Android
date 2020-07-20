using System;
using System.Collections.Generic;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core;
using WarKiwiCode.Game_Files.Scripts.Projectiles;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class ObjectPoolerManager : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            [Tooltip("Max size of objects to create. If RECYCLE is set to false, it'll stop spawning once it reaches this number")]
            public int size;
            [Tooltip("If true, this pool will recycle objects once it reaches it's SIZE.")]
            public bool recycle;
        }
    
        [SerializeField] private List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> _poolDictionary;

        #region Singleton

        public static ObjectPoolerManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        // Start is called before the first frame update
        private void Start()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                _poolDictionary.Add(pool.tag, objectPool);
            }
        }
        
        public GameObject Spawn(string objectTag, Vector3 position, Quaternion rotation, SpawnAreaName areaName)
        {
            if (!_poolDictionary.ContainsKey(objectTag))
            {
                // Pool doesn't exist
                return null;
            }

            // Find the pool that corresponds to the spawned object.
            Pool nextObjectToSpawnPool = pools.Find(pool => pool.tag.Equals(objectTag));

            if (nextObjectToSpawnPool.recycle)
            {
                // Spawn the object
                GameObject objectToSpawn = SpawnObjectFromPool(objectTag, position, rotation, areaName);
                
                // Place the object inside the queue
                _poolDictionary[objectTag].Enqueue(objectToSpawn);

                return objectToSpawn;
            }

            if (_poolDictionary[objectTag].Count > 0)
            {
                // Spawn the object
                GameObject objectToSpawn = SpawnObjectFromPool(objectTag, position, rotation, areaName);

                return objectToSpawn;
            }

            // Pool empty
            return null;
        }

        private GameObject SpawnObjectFromPool(string objectTag, Vector3 position, Quaternion rotation, SpawnAreaName areaName)
        {
            GameObject objectToSpawn = _poolDictionary[objectTag].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            if (areaName != SpawnAreaName.None)
            {
                // Check if it's an enemy and pass the area name
                CheckForISpawnable(objectToSpawn, areaName);
            }

            // Check if it's an object with the OnSpawn Method
            CheckForIPooledObject(objectToSpawn);

            return objectToSpawn;
        }

        public void Despawn(string objectTag, GameObject objectToDespawn)
        {
            _poolDictionary[objectTag].Enqueue(objectToDespawn);
        }

        private static void CheckForIPooledObject(GameObject obj)
        {
            IPooledObject pooledObject = obj.GetComponent<IPooledObject>();
            pooledObject?.OnSpawn();
        }

        private static void CheckForISpawnable(GameObject obj, SpawnAreaName areaName)
        {
            // TODO: Revisar si se necesita eventualmente
            ISpawnable pooledObject = obj.GetComponent<ISpawnable>();
            pooledObject?.SetSpawnArea(areaName);
        }
        
        
    }
}

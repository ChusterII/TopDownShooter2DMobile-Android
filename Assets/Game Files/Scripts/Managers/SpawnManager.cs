using System;
using System.Collections.Generic;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core;
using WarKiwiCode.Game_Files.Scripts.Difficulty;
using Random = UnityEngine.Random;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public enum SpawnAreaName
    {
        Top,
        TopLeft,
        TopRight,
        Bottom,
        BottomLeft,
        BottomRight,
        None
    }

    [System.Serializable]
    public class SpawnArea
    {
        public SpawnAreaName spawnAreaName;
        public Collider2D spawnArea;
    }

    [System.Serializable]
    public class SpawnInfo
    {
        public SpawnAreaName spawnAreaName;
        public Vector2 spawnPosition;

        public SpawnInfo(SpawnAreaName areaName, Vector2 position)
        {
            spawnAreaName = areaName;
            spawnPosition = position;
        }
    }
    
    public class SpawnManager : MonoBehaviour
    {
        [Header("Spawning areas")]
        [SerializeField] private SpawnArea[] topSpawnAreas;
        [SerializeField] private SpawnArea[] bottomSpawnAreas;
        [Space(5f)]
        [Header("Top flanking targets")]
        [SerializeField] private Transform topLeftFlankTarget;
        [SerializeField] private Transform topRightFlankTarget;
        [SerializeField] private Transform topLeftFlankPosition;
        [SerializeField] private Transform topRightFlankPosition;
        [Header("Bottom flanking targets")]
        [SerializeField] private Transform bottomLeftFlankTarget;
        [SerializeField] private Transform bottomRightFlankTarget;
        [SerializeField] private Transform bottomLeftFlankPosition;
        [SerializeField] private Transform bottomRightFlankPosition;
        

        private ObjectPoolerManager _objectPooler;
        private delegate SpawnInfo SpawnDelegate(string enemyName);
        
        public static SpawnManager instance;
        
        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            _objectPooler = ObjectPoolerManager.instance;
        }

        /// <summary>
        /// Spawns enemies. It should spawn 1 enemy on each half of the screen at random positions, every X seconds (GameManager).
        /// </summary>
        public void SpawnEnemies(MeleeEnemySpawnData[] meleeSpawnData, RangedEnemySpawnData[] rangedSpawnData)
        {
            // Choose enemy
            string topEnemy = SelectEnemyToSpawn(meleeSpawnData, rangedSpawnData);
            string bottomEnemy = SelectEnemyToSpawn(meleeSpawnData, rangedSpawnData);
            
            SpawnEnemy(topEnemy, SpawnEnemyTop);
            SpawnEnemy(topEnemy, SpawnEnemyBottom);

        }

        private static string SelectEnemyToSpawn(MeleeEnemySpawnData[] meleeSpawnData, RangedEnemySpawnData[] rangedSpawnData)
        {
            // Select enemy to spawn
            string enemyName;
            
            // Check if any of the arrays are empty (filled with 0's)
            bool isMeleeEmpty = Array.TrueForAll(meleeSpawnData, data => data.spawnChance == 0);
            bool isRangedEmpty = Array.TrueForAll(rangedSpawnData, data => data.spawnChance == 0);

            // If neither is empty, then randomly choose an enemy from either list
            if (!isMeleeEmpty && !isRangedEmpty)
            {
                enemyName = Random.Range(0, 2) == 0 ? FindSuitableMeleeEnemy(meleeSpawnData) : FindSuitableRangedEnemy(rangedSpawnData);
            }
            else 
            {
                // If melee is empty, then get an enemy from Ranged and viceversa.
                enemyName = isMeleeEmpty ? FindSuitableRangedEnemy(rangedSpawnData) : FindSuitableMeleeEnemy(meleeSpawnData);
            }

            return enemyName;
        }

        private static string FindSuitableRangedEnemy(IList<RangedEnemySpawnData> rangedSpawnData)
        {
            bool foundSuitableEnemy = false;
            string enemyName = "";
            while (!foundSuitableEnemy)
            {
                int randomIndex = Random.Range(0, rangedSpawnData.Count);
                if (rangedSpawnData[randomIndex].spawnChance > 0)
                {
                    switch (rangedSpawnData[randomIndex].enemyType)
                    {
                        case RangedEnemyType.PistolRanged:
                            enemyName = "EnemyPistolRanged";
                            break;
                        case RangedEnemyType.SmgRanged:
                            enemyName = "EnemySmgRanged";
                            break;
                        case RangedEnemyType.RpgRanged:
                            enemyName = "EnemyRpgRanged";
                            break;
                        default:
                            enemyName = "EnemyPistolRanged";
                            break;
                    }

                    foundSuitableEnemy = true;
                }
            }

            return enemyName;
        }

        private static string FindSuitableMeleeEnemy(IList<MeleeEnemySpawnData> meleeSpawnData)
        {
            bool foundSuitableEnemy = false;
            string enemyName = "";
            while (!foundSuitableEnemy)
            {
                int randomIndex = Random.Range(0, meleeSpawnData.Count);
                if (meleeSpawnData[randomIndex].spawnChance > 0)
                {
                    switch (meleeSpawnData[randomIndex].enemyType)
                    {
                        case MeleeEnemyType.SlowMelee:
                            enemyName = "EnemySlowMelee";
                            break;
                        case MeleeEnemyType.NormalMelee:
                            enemyName = "EnemyNormalMelee";
                            break;
                        case MeleeEnemyType.FastMelee:
                            enemyName = "EnemyFastMelee";
                            break;
                        default:
                            enemyName = "EnemySlowMelee";
                            break;
                    }

                    foundSuitableEnemy = true;
                }
            }

            return enemyName;
        }



        private void SpawnEnemy(string enemyName, SpawnDelegate spawnDelegate)
        {
            // Get the corresponding spawn information
            SpawnInfo spawnInfo = spawnDelegate(enemyName);
                
            // Spawn the enemy object
            GameObject spawnedEnemy = _objectPooler.Spawn(enemyName, spawnInfo.spawnPosition, Quaternion.identity, spawnInfo.spawnAreaName);

        }

        private SpawnInfo SpawnEnemyTop(string enemyName)
        {
            return GetSpawningPosition(topSpawnAreas);
        }
        
        private SpawnInfo SpawnEnemyBottom(string enemyName)
        {
            return GetSpawningPosition(bottomSpawnAreas);
        }

        private static SpawnInfo GetSpawningPosition(IReadOnlyList<SpawnArea> spawnAreas)
        {
            // Choose an area to spawn the enemy
            int randomArea = Random.Range(0, spawnAreas.Count);
            
            // Get the boundaries of the area
            Bounds bounds = spawnAreas[randomArea].spawnArea.bounds;
            
            // Grab the area's name
            SpawnAreaName areaName = spawnAreas[randomArea].spawnAreaName;
                
            // Get a spawning point inside the boundaries
            float xValue = Random.Range(bounds.min.x, bounds.max.x);
            float yValue = Random.Range(bounds.min.y, bounds.max.y);
            
            // Build and return the class
            return new SpawnInfo(areaName, new Vector2(xValue, yValue));
        }

        public FlankInfo GetFlankInfo(Vector2 position)
        {
            if (position.x >= 0)
            {
                return position.y >= 0
                    ? new FlankInfo(topRightFlankTarget.position, topRightFlankPosition.position)
                    : new FlankInfo(bottomRightFlankTarget.position, bottomRightFlankPosition.position);
            }
            return position.y >= 0
                ? new FlankInfo(topLeftFlankTarget.position, topLeftFlankPosition.position)
                : new FlankInfo(bottomLeftFlankTarget.position, bottomLeftFlankPosition.position);
        }


    }

    
}

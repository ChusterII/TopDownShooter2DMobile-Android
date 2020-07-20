using System;
using System.Collections.Generic;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core;
using WarKiwiCode.Game_Files.Scripts.Difficulty;
using WarKiwiCode.Game_Files.Scripts.Utilities;
using Random = UnityEngine.Random;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
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
        [Header("")]
        

        private ObjectPoolerManager _objectPooler;
        private Dictionary<MeleeEnemyType, List<string>> _meleeNamesDictionary = new Dictionary<MeleeEnemyType, List<string>>();
        private delegate SpawnPositionAndArea SpawnDelegate();
        private delegate string[] NameDelegate(MeleeEnemyType enemyType);
        
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
        public void SpawnEnemies(SpawnData spawnData)
        {
            // Choose enemy
            string topEnemy = SelectEnemyToSpawn(spawnData);
            string bottomEnemy = SelectEnemyToSpawn(spawnData);
            
            // Spawn the enemies
            SpawnEnemy(topEnemy, SpawnEnemyTop);
            SpawnEnemy(bottomEnemy, SpawnEnemyBottom);

        }

        private static string SelectEnemyToSpawn(SpawnData spawnData)
        {
            // Select enemy to spawn
            string enemyName;
            
            // Check if any of the arrays are empty (filled with 0's)
            bool isMeleeEmpty = Array.TrueForAll(spawnData.MeleeData, data => data.spawnChance == 0);
            bool isRangedEmpty = Array.TrueForAll(spawnData.RangedData, data => data.spawnChance == 0);

            
            // TODO: CHANGE PARAMETERS OF FINDSUITABLEXXXXENEMY TO SPAWNDATA.
            // If neither is empty, then randomly choose an enemy from either list
            if (!isMeleeEmpty && !isRangedEmpty)
            {
                enemyName = Random.Range(0, 2) == 0
                    ? FindSuitableMeleeEnemy(spawnData.MeleeData, spawnData.MeleeMovementProbability)
                    : FindSuitableRangedEnemy(spawnData.RangedData, spawnData.RangedMovementProbability);
            }
            else 
            {
                // If melee is empty, then get an enemy from Ranged and viceversa.
                enemyName = isMeleeEmpty
                    ? FindSuitableRangedEnemy(spawnData.RangedData, spawnData.RangedMovementProbability)
                    : FindSuitableMeleeEnemy(spawnData.MeleeData, spawnData.MeleeMovementProbability);
            }
            return enemyName;
        }

        private static string FindSuitableRangedEnemy(IList<RangedEnemySpawnData> rangedSpawnData, RangedMovementProbability rangedMovementProbability)
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

        private static string FindSuitableMeleeEnemy(IList<MeleeEnemySpawnData> meleeSpawnData, MeleeMovementProbability meleeMovementProbability)
        {
            bool foundSuitableEnemy = false;
            string enemyName = "";
            MeleeMovement meleeMovement;
            while (!foundSuitableEnemy)
            {
                int randomIndex = Random.Range(0, meleeSpawnData.Count);
                if (meleeSpawnData[randomIndex].spawnChance > 0)
                {
                    switch (meleeSpawnData[randomIndex].enemyType)
                    {
                        case MeleeEnemyType.SlowMelee:
                            enemyName = GetSlowMeleeEnemyNameFromMovementProbability(meleeMovementProbability, MeleeEnemyType.SlowMelee, GetSlowMeleeEnemyNames);
                            //enemyName = "EnemySlowRushMelee";
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
            //print(enemyName);
            return enemyName;
        }
        
        
        
        // TODO: Hacer que esto sirva para otros tipos de enemigos (solo sirve pa slow) - YA CASI!
        private static string GetSlowMeleeEnemyNameFromMovementProbability(
            MeleeMovementProbability meleeMovementProbability, MeleeEnemyType enemyType, NameDelegate nameDelegate)
        {
            string enemyName;
            MeleeMovement meleeMovement = meleeMovementProbability.meleeProbabilities.Find(movement =>
                movement.enemyType == enemyType);
            float randomValue = Random.value;

           if (randomValue < meleeMovement.rushChance)
            {
                // Slow rushing enemy
                //enemyName = "EnemySlowRushMelee";
                enemyName = nameDelegate(enemyType)[0];
            }
            else
            {
                float increment = meleeMovement.flankChance + meleeMovement.rushChance;
                enemyName = randomValue < increment ? "EnemySlowFlankMelee" : "EnemySlowZigzagMelee";
            }

            return enemyName;
        }

        private static string[] GetSlowMeleeEnemyNames(MeleeEnemyType enemyType)
        {
            return new[] {"EnemySlowRushMelee", "EnemySlowFlankMelee", "EnemySlowZigzagMelee"};
        }


        private void SpawnEnemy(string enemyName, SpawnDelegate spawnDelegate)
        {
            // Get the corresponding spawn information
            SpawnPositionAndArea spawnPositionAndArea = spawnDelegate();
                
            // Spawn the enemy object
            _objectPooler.Spawn(enemyName, spawnPositionAndArea.spawnPosition, Quaternion.identity, spawnPositionAndArea.spawnAreaName);

        }

        private SpawnPositionAndArea SpawnEnemyTop()
        {
            return GetSpawningPosition(topSpawnAreas);
        }
        
        private SpawnPositionAndArea SpawnEnemyBottom()
        {
            return GetSpawningPosition(bottomSpawnAreas);
        }

        private static SpawnPositionAndArea GetSpawningPosition(IReadOnlyList<SpawnArea> spawnAreas)
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
            return new SpawnPositionAndArea(areaName, new Vector2(xValue, yValue));
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

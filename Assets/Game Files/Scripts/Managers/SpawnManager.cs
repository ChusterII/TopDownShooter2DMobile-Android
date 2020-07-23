using System;
using System.Collections.Generic;
using UnityEngine;
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

        private ObjectPoolerManager _objectPooler;
        private delegate SpawnPositionAndArea SpawnDelegate();

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
            //SpawnEnemy(bottomEnemy, SpawnEnemyBottom);
        }

        private static string SelectEnemyToSpawn(SpawnData spawnData)
        {
            // Select enemy to spawn
            string enemyName;
            
            // Check if any of the arrays are empty (filled with 0's)
            bool isMeleeEmpty = Array.TrueForAll(spawnData.MeleeData, data => data.spawnChance == 0);
            bool isRangedEmpty = Array.TrueForAll(spawnData.RangedData, data => data.spawnChance == 0);

            // If neither is empty, then randomly choose an enemy from either list
            if (!isMeleeEmpty && !isRangedEmpty)
            {
                enemyName = Random.Range(0, 2) == 0
                    ? FindSuitableMeleeEnemy(spawnData)
                    : FindSuitableRangedEnemy(spawnData);
            }
            else 
            {
                // If melee is empty, then get an enemy from Ranged and viceversa.
                enemyName = isMeleeEmpty
                    ? FindSuitableRangedEnemy(spawnData)
                    : FindSuitableMeleeEnemy(spawnData);
            }
            return enemyName;
        }

        private static string FindSuitableRangedEnemy(SpawnData spawnData)
        {
            bool foundSuitableEnemy = false;
            string enemyName = "";
            while (!foundSuitableEnemy)
            {
                int randomIndex = Random.Range(0, spawnData.RangedData.Length);
                if (spawnData.RangedData[randomIndex].spawnChance > 0)
                {
                    enemyName = GetRangedEnemyNameFromMovementProbability(spawnData, spawnData.RangedData[randomIndex].enemyType);
                    foundSuitableEnemy = true;
                }
            }
            return enemyName;
            //return "EnemyNormalRushPistol";
        }

        private static string FindSuitableMeleeEnemy(SpawnData spawnData)
        {
            bool foundSuitableEnemy = false;
            string enemyName = "";
            while (!foundSuitableEnemy)
            {
                int randomIndex = Random.Range(0, spawnData.MeleeData.Length);
                if (spawnData.MeleeData[randomIndex].spawnChance > 0)
                {
                    enemyName = GetMeleeEnemyNameFromMovementProbability(spawnData, spawnData.MeleeData[randomIndex].enemyType);
                    foundSuitableEnemy = true;
                }
            }
            return enemyName;
        }

        private static string GetMeleeEnemyNameFromMovementProbability(SpawnData spawnData, EnemyType.MeleeEnemyType enemyType)
        {
            string enemyName;
            MeleeMovement meleeMovement = spawnData.MeleeMovementProbability.meleeProbabilities.Find(movement =>
                movement.enemyType == enemyType);
            MeleeEnemyTypeToNameContainer nameContainer =
                spawnData.NameContainer.meleeEnemyTypeToNameContainer.Find(container =>
                    container.meleeEnemyType == enemyType);
            float randomValue = Random.value;

            if (randomValue < meleeMovement.rushChance)
            {
                // Rushing enemy
                enemyName = nameContainer.meleeEnemyNames[0];
            }
            else
            {
                float increment = meleeMovement.flankChance + meleeMovement.rushChance;
                enemyName = randomValue < increment ? nameContainer.meleeEnemyNames[1] : nameContainer.meleeEnemyNames[2];
            }
            return enemyName;
        }
        
        private static string GetRangedEnemyNameFromMovementProbability(SpawnData spawnData, EnemyType.RangedEnemyType enemyType)
        {
            RangedMovement rangedMovement = spawnData.RangedMovementProbability.rangedProbabilities.Find(movement =>
                movement.enemyType == enemyType);
            RangedEnemyTypeToNameContainer nameContainer =
                spawnData.NameContainer.rangedEnemyTypeToNameContainer.Find(container =>
                    container.rangedEnemyType == enemyType);
            float randomValue = Random.value;
            string enemyName = randomValue < rangedMovement.rushChance ? nameContainer.rangedEnemyNames[0] : nameContainer.rangedEnemyNames[1];
            return enemyName;
        }

        private void SpawnEnemy(string enemyName, SpawnDelegate spawnDelegate)
        {
            // Get the corresponding spawn information
            SpawnPositionAndArea spawnPositionAndArea = spawnDelegate();
                
            // Spawn the enemy object
            _objectPooler.Spawn(enemyName, spawnPositionAndArea.spawnPosition, Quaternion.identity, spawnPositionAndArea.spawnAreaName);
        }

        private SpawnPositionAndArea SpawnEnemyTop() => GetSpawningPosition(topSpawnAreas);

        private SpawnPositionAndArea SpawnEnemyBottom() => GetSpawningPosition(bottomSpawnAreas);

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

using System.Collections.Generic;
using MEC;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Difficulty;
using WarKiwiCode.Game_Files.Scripts.Utilities;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Difficulty Levels")]
        [SerializeField] private MeleeDifficulty meleeDifficulty;
        [SerializeField] private RangedDifficulty rangedDifficulty;
        
        [Header("Movement Probabilities")]
        [SerializeField] private MeleeMovementProbability meleeMovementProbability;
        [SerializeField] private RangedMovementProbability rangedMovementProbability;
        
        [Header("Settings")] 
        [SerializeField] private EnemyTypeToNameContainer enemyNamesContainer;
        [SerializeField] private float spawnCooldown = 1f;
        [SerializeField] private float stageMaxTime;
        
        
        
        
        [Header("Testing")] 
        [SerializeField] private int difficultyLevel = 0;
        

        private SpawnManager _spawnManager;
        private float _stageTimer;
        private float _spawnTimer;
        private bool _canSpawn = true;
        private SpawnData _spawnData;
        

        public static GameManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _spawnManager = SpawnManager.instance;
            
            GetSpawnDataFromDifficulty();

            Timing.RunCoroutine(GameLoop().CancelWith(gameObject));
        }

        // Update is called once per frame
        void Update()
        {
            _stageTimer += Time.deltaTime;
            _spawnTimer += Time.deltaTime;
        }

        private void GetSpawnDataFromDifficulty()
        {
           MeleeEnemySpawnData[] meleeSpawnData = meleeDifficulty.meleeDifficultyLevels[difficultyLevel].meleeEnemySpawnData;
           RangedEnemySpawnData[] rangedSpawnData = rangedDifficulty.rangedDifficultyLevels[difficultyLevel].rangedEnemySpawnData;
           _spawnData = new SpawnData(meleeSpawnData, rangedSpawnData, meleeMovementProbability, rangedMovementProbability, enemyNamesContainer);
        }

        private IEnumerator<float> GameLoop()
        {
            while (_canSpawn)
            {
                // Need to wait a bit before spawning because the SpawnManager hasn't loaded
                yield return Timing.WaitForSeconds(3f);
                _spawnManager.SpawnEnemies(_spawnData);
                yield return Timing.WaitForSeconds(spawnCooldown);
                if (_stageTimer > stageMaxTime)
                {
                    _canSpawn = false;
                }
            }
            yield return Timing.WaitForOneFrame;
        }
        
        
        
        
    }
}

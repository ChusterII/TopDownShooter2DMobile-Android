using System.Collections.Generic;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Utilities;

namespace WarKiwiCode.Game_Files.Scripts.Difficulty
{
    [CreateAssetMenu(fileName = "Ranged Difficulty Settings", menuName = "Difficulty/Ranged Difficulty Setting")]
    public class RangedDifficulty : ScriptableObject
    {
        public List<RangedDifficultyLevel> rangedDifficultyLevels;
    }

/*
public enum RangedEnemyType
    {
        PistolRanged,
        SmgRanged ,
        RpgRanged
    }*/
    
    [System.Serializable]
    public struct RangedEnemySpawnData
    {
        public EnemyType.RangedEnemyType enemyType;
        public float spawnChance;
    }
    
    [System.Serializable]
    public class RangedDifficultyLevel
    {
        public RangedEnemySpawnData[] rangedEnemySpawnData = new RangedEnemySpawnData[3];
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Difficulty
{
    [CreateAssetMenu(fileName = "Ranged Difficulty Settings", menuName = "Difficulty/Ranged Difficulty Setting")]
    public class RangedDifficulty : ScriptableObject
    {
        public List<RangedDifficultyLevel> rangedDifficultyLevels;
    }
    
    public enum RangedEnemyType
    {
        PistolRanged = 0,
        SmgRanged = 1,
        RpgRanged = 2
    }
    
    [System.Serializable]
    public struct RangedEnemySpawnData
    {
        public RangedEnemyType enemyType;
        public float spawnChance;
    }
    
    [System.Serializable]
    public class RangedDifficultyLevel
    {
        public RangedEnemySpawnData[] rangedEnemySpawnData = new RangedEnemySpawnData[3];
    }
}

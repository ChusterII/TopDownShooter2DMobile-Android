using System.Collections.Generic;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Difficulty
{
    [CreateAssetMenu(fileName = "Melee Difficulty Settings", menuName = "Difficulty/Melee Difficulty Setting")]
    public class MeleeDifficulty : ScriptableObject
    {
        public List<MeleeDifficultyLevel> meleeDifficultyLevels;
    }
    
    public enum MeleeEnemyType
    {
        SlowMelee = 0,
        NormalMelee = 1,
        FastMelee = 2
    }
    
    [System.Serializable]
    public struct MeleeEnemySpawnData
    {
        public MeleeEnemyType enemyType;
        public float spawnChance;
    }

    [System.Serializable]
    public class MeleeDifficultyLevel
    {
        public MeleeEnemySpawnData[] meleeEnemySpawnData = new MeleeEnemySpawnData[3];
    }
    
   
    
    

    
}

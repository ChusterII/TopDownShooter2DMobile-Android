using System.Collections.Generic;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Utilities;

namespace WarKiwiCode.Game_Files.Scripts.Difficulty
{
    [CreateAssetMenu(fileName = "Ranged Movement Probability", menuName = "Difficulty/Ranged Movement Probability", order = 3)]
    public class RangedMovementProbability : ScriptableObject
    {
        public List<RangedMovement> rangedProbabilities = new List<RangedMovement>();
    }
    
    [System.Serializable]
    public class RangedMovement
    {
        public EnemyType.RangedEnemyType enemyType;
        public float rushChance;
        public float zigzagChance;
    }
}

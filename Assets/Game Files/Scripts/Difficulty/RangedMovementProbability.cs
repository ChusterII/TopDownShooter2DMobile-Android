using System.Collections.Generic;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Difficulty
{
    [CreateAssetMenu(fileName = "Ranged Movement Probability", menuName = "Difficulty/Ranged Movement Probability", order = 3)]
    public class RangedMovementProbability : ScriptableObject
    {
        public List<MeleeMovement> rangedProbabilities = new List<MeleeMovement>();
    }
    
    [System.Serializable]
    public class RangedMovement
    {
        public RangedEnemyType enemyType;
        public float rushChance;
        public float zigzagChance;
    }
}

using System.Collections.Generic;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Utilities;

namespace WarKiwiCode.Game_Files.Scripts.Difficulty
{
    [CreateAssetMenu(fileName = "Melee Movement Probability", menuName = "Difficulty/Melee Movement Probability", order = 2)]
    public class MeleeMovementProbability : ScriptableObject
    {
        public List<MeleeMovement> meleeProbabilities = new List<MeleeMovement>();
    }

    [System.Serializable]
    public class MeleeMovement
    {
        public EnemyType.MeleeEnemyType enemyType;
        public float rushChance;
        public float flankChance;
        public float zigzagChance;
    }
}

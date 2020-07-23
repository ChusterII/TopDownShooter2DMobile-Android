using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Utilities
{
    [CreateAssetMenu(fileName = "Enemy Types To Names", menuName = "Utility/Enemy Type To Name Container", order = 4)]
    public class EnemyTypesToNames : ScriptableObject
    {
        public List<MeleeEnemyTypeToNameContainer> meleeEnemyTypeToNameContainer = new List<MeleeEnemyTypeToNameContainer>();
        public List<RangedEnemyTypeToNameContainer> rangedEnemyTypeToNameContainer = new List<RangedEnemyTypeToNameContainer>();
    }

    [Serializable]
    public class MeleeEnemyTypeToNameContainer
    {
        public EnemyType.MeleeEnemyType meleeEnemyType;
        public List<string> meleeEnemyNames;
    }
    
    [Serializable]
    public class RangedEnemyTypeToNameContainer
    {
        public EnemyType.RangedEnemyType rangedEnemyType;
        public List<string> rangedEnemyNames;
    }
}

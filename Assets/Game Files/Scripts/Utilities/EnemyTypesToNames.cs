using System;
using System.Collections.Generic;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core;
using WarKiwiCode.Game_Files.Scripts.Difficulty;

namespace WarKiwiCode.Game_Files.Scripts.Utilities
{
    [CreateAssetMenu(fileName = "Enemy Types To Names", menuName = "Utility/Enemy Type To Name Container", order = 2)]
    public class EnemyTypesToNames : ScriptableObject
    {
        public List<EnemyTypeToNameContainer> enemyTypeToNameContainer = new List<EnemyTypeToNameContainer>();
    }

    [Serializable]
    public class EnemyTypeToNameContainer
    {
        public EnemyType enemyType;
        public List<string> enemyNames;
    }
}

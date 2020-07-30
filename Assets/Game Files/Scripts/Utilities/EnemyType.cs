using System;
using System.CodeDom;

namespace WarKiwiCode.Game_Files.Scripts.Utilities
{
    [Serializable]
    public class EnemyType
    {
        public enum RangedEnemyType
        {
            Pistol,
            Smg,
            Rpg
        }
        
        public enum MeleeEnemyType
        {
            Slow,
            Normal,
            Fast
        }
    }
}

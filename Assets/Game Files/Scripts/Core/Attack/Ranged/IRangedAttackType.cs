using WarKiwiCode.Game_Files.Scripts.Utilities;

namespace WarKiwiCode.Game_Files.Scripts.Core.Attack.Ranged
{
    public interface IRangedAttackType
    {
        EnemyType.RangedEnemyType GetAttackType();
        EnemyWeapon GetEnemyWeaponData();
    }
}

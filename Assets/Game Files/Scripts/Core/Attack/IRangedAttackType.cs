namespace WarKiwiCode.Game_Files.Scripts.Core.Attack
{
    public interface IRangedAttackType
    {
        EnemyType GetAttackType();
        EnemyWeaponData GetEnemyWeaponData();
    }
}

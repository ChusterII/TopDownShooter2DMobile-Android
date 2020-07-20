namespace WarKiwiCode.Game_Files.Scripts.Core.Attack
{
    public class EnemyWeaponData
    {
        public readonly float weaponMinRange;
        public readonly float weaponMaxRange;

        public EnemyWeaponData(float minRange, float maxRange)
        {
            weaponMaxRange = maxRange;
            weaponMinRange = minRange;
        }
    }
}
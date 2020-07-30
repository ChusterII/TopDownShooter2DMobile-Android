using System.Collections.Generic;
using MEC;
using WarKiwiCode.Game_Files.Scripts.Managers;
using WarKiwiCode.Game_Files.Scripts.Utilities;

namespace WarKiwiCode.Game_Files.Scripts.Core.Attack.Ranged
{
    public class SmgRangedAttack : EnemyRangedAttack, IRangedAttackType
    {

        protected override void Attack()
        {
            Timing.RunCoroutine(AttackWithAutomaticWeapon().CancelWith(gameObject));
        }

        private IEnumerator<float> AttackWithAutomaticWeapon()
        {
            for (int i = 0; i < weapon.bulletsToFire; i++)
            {
                ObjectPoolerManager.instance.Spawn("EnemyPistolBullet", transform.position, transform.rotation,
                        SpawnAreaName.None);
                yield return Timing.WaitForSeconds(weapon.rateOfFire);
            }
        }

        public EnemyType.RangedEnemyType GetAttackType()
        {
            return EnemyType.RangedEnemyType.Smg;
        }

        public EnemyWeapon GetEnemyWeaponData()
        {
            return weapon;
        }
    }
}

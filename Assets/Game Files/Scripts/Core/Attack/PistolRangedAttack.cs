using System;
using System.Collections.Generic;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Managers;
using WarKiwiCode.Game_Files.Scripts.Utilities;

namespace WarKiwiCode.Game_Files.Scripts.Core.Attack
{
    public class PistolRangedAttack : EnemyRangedAttack, IRangedAttackType
    {
        private GameObject _gameObject;

        
        protected override void Attack()
        {
            ObjectPoolerManager.instance.Spawn("EnemyPistolBullet", transform.position, transform.rotation,
                SpawnAreaName.None);
        }

        public EnemyType.RangedEnemyType GetAttackType()
        {
            return EnemyType.RangedEnemyType.Pistol;
        }

        public EnemyWeapon GetEnemyWeaponData()
        {
            return weapon;
        }
    }
}

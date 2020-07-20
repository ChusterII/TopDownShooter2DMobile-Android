using System;
using System.Collections.Generic;

namespace WarKiwiCode.Game_Files.Scripts.Core.Attack
{
    public class PistolRangedAttack : EnemyAttack, IRangedAttackType
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case State.Normal:
                    state = State.Aiming;
                    StartAimingAtTarget();
                    break;
                case State.Aiming:
                    UpdateAimingAtTarget();
                    break;
                case State.Attacking:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Attack()
        {
            
        }

        

        public EnemyType GetAttackType()
        {
            return EnemyType.PistolRanged;
        }

        // Enemy Pistol Data
        public EnemyWeaponData GetEnemyWeaponData()
        {
            return new EnemyWeaponData(2.5f, 3.5f);
        }
    }
}

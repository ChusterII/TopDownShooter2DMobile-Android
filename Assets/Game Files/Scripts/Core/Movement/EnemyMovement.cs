using System;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core.Attack;
using WarKiwiCode.Game_Files.Scripts.Managers;
using Random = UnityEngine.Random;

namespace WarKiwiCode.Game_Files.Scripts.Core.Movement
{
    public abstract class EnemyMovement : MonoBehaviour
    {
        
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float minDistanceToTargetPosition;

        private enum TargetPositionType
        {
            Player,
            WeaponRange
        }

        protected float step;
        protected bool canMove = true;
        protected bool disableMovement;
        protected Vector3 finalPosition;
        private bool _spawnedTop;
        protected SpawnManager spawnManager;
        private IRangedAttackType _enemyRangedAttackType;
        

        protected abstract void Move();
        
        public void InitializeMovement()
        {
            DisableEnemyMovement(false);
            GetSpawnArea(GetPosition());
            spawnManager = SpawnManager.instance;
            _enemyRangedAttackType = GetComponent<IRangedAttackType>(); // Ranged Attack Interface
            finalPosition = _enemyRangedAttackType != null
                ? FindWeaponRangePosition(_enemyRangedAttackType.GetEnemyWeaponData())
                : FindNearestPlayer();

        }
        
        protected Vector3 GetPosition()
        {
            return transform.position;
        }

        public Vector3 FindNearestPlayer()
        {
            // TODO: Esto se puede cambiar por una lista estatica de los jugadores? 
            return _spawnedTop ? GameObject.FindWithTag("PlayerTop").transform.position : GameObject.FindWithTag("PlayerBottom").transform.position;
        }

        private Vector3 FindWeaponRangePosition(EnemyWeaponData weaponData)
        {
            float minRange = weaponData.weaponMinRange;
            float maxRange = weaponData.weaponMaxRange;
            float randomX = Random.Range(-3f, 3f);
            float randomY = _spawnedTop ? Random.Range(minRange, maxRange) : Random.Range(-maxRange, -minRange);
            return new Vector3(randomX, randomY);
        }

        private void GetSpawnArea(Vector3 startingPosition)
        {
            _spawnedTop = startingPosition.y >= 0;
        }

        public void DisableEnemyMovement(bool value)
        {
            disableMovement = value;
        }
    }
}

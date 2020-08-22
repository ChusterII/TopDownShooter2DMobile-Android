using System;
using System.Collections.Generic;
using Data.Util;
using UnityEngine;
using UnityEngine.Events;
using WarKiwiCode.Game_Files.Scripts.Core.Attack;
using WarKiwiCode.Game_Files.Scripts.Core.Attack.Ranged;
using WarKiwiCode.Game_Files.Scripts.Managers;
using Random = UnityEngine.Random;

namespace WarKiwiCode.Game_Files.Scripts.Core.Movement
{
    public abstract class EnemyMovement : MonoBehaviour
    {
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float minDistanceToTargetPosition;
        public UnityEvent startAttacking; // TODO: creo que no hace falta!!!

        public static List<Vector3> AllFinalPositionsRanged { get; private set; }
        
        protected float step;
        protected bool canMove;
        protected bool disableMovement;
        protected Vector3 finalPosition;
        private bool _spawnedTop;
        protected SpawnManager spawnManager;
        private IRangedAttackType _enemyRangedAttackType;
        protected bool attackStarted;

        protected abstract void Move();
        
        public void InitializeMovement()
        {
            //print(Vector2.Distance(new Vector2(-1.7f, 3.7f), new Vector2(-1.4f, 3.8f)));
            // If empty, create it.
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (AllFinalPositionsRanged == null)
            {
                AllFinalPositionsRanged = new List<Vector3>();
            }
            attackStarted = false;
            canMove = true;
            DisableEnemyMovement(false);
            GetSpawnArea(GetPosition());
            spawnManager = SpawnManager.instance;
            _enemyRangedAttackType = GetComponent<IRangedAttackType>(); // Ranged Attack Interface
            CalculateFinalPosition();
            CheckValidRangedFinalPosition();
            SetForwardVector(finalPosition);
        }

        public void SetForwardVector(Vector3 lookAtTarget)
        {
            // Get the direction of the touch
            Vector3 direction = (lookAtTarget - GetPosition()).normalized;

            // Line up the rotation towards the touched point on screen
            transform.up = direction;
        }

        private void CheckValidRangedFinalPosition()
        {
            if (AllFinalPositionsRanged.Count > 0 && _enemyRangedAttackType != null)
            {
                //print("entered check");
                bool foundSuitableFinalPosition = false;
                List<Vector3> closestPoints = new List<Vector3>();
                while (!foundSuitableFinalPosition)
                {
                    foreach (Vector3 position in AllFinalPositionsRanged)
                    {
                        float distanceBetweenPositions = Vector3.Distance(finalPosition, position);
                        if (distanceBetweenPositions < 0.75f)
                        {
                            closestPoints.Add(position);
                        }
                    }
                    if (closestPoints.Count == 0)
                    {
                        //print("Check passed.");
                        foundSuitableFinalPosition = true;
                    }
                    else
                    {
                        //print("Check failed with a count of: " + closestPoints.Count);
                        //print("Getting a new position.");
                        // Select a new spawn point
                        CalculateFinalPosition();
                        closestPoints.Clear();
                    }
                }
            }
            AllFinalPositionsRanged.Add(finalPosition);
            //print("Position: " + finalPosition + " added.");
        }

        private void CalculateFinalPosition()
        {
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
            if (_spawnedTop)
            {
                //print("Spawned Top: " + GameObject.FindWithTag("PlayerTop").tag);
                return GameObject.FindWithTag("PlayerTop").transform.position;
            }
            else
            {
                //print("Spawned Bottom: " + GameObject.FindWithTag("PlayerBottom").tag);
                return GameObject.FindWithTag("PlayerBottom").transform.position;
            }
                
        }

        private Vector3 FindWeaponRangePosition(EnemyWeapon weapon)
        {
            float minRange = weapon.weaponMinRange;
            float maxRange = weapon.weaponMaxRange;
            float randomX = Random.Range(-3f, 3f);
            float randomY = _spawnedTop ? Random.Range(minRange, maxRange) : Random.Range(-maxRange, -minRange);
            return new Vector3(randomX, randomY);
        }

        private void GetSpawnArea(Vector3 startingPosition) => _spawnedTop = startingPosition.y >= 0;

        public void DisableEnemyMovement(bool value) => disableMovement = value;

        public void RemoveFinalPositionFromList()
        {
            //print("Position: " + finalPosition + " removed.");
            AllFinalPositionsRanged.Remove(finalPosition);
        }
    }
}

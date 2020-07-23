using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Managers;

namespace WarKiwiCode.Game_Files.Scripts.Core.Movement
{
    public class FlankMovement : EnemyMovement
    {
        private bool _moveToTarget = true;
        private bool _moveToPosition;
        private bool _moveToPlayer;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Move();
        }

        protected override void Move()
        {
            step = moveSpeed * Time.deltaTime;
            if (disableMovement) return;
            FlankInfo flankInfo = spawnManager.GetFlankInfo(GetPosition());
            if (canMove)
            {
                TryMoveFlankingTowards(_moveToTarget, flankInfo.flankTarget, step);
                TryMoveFlankingTowards(_moveToPosition, flankInfo.flankPosition, step);
                TryMoveFlankingTowards(_moveToPlayer, finalPosition, step);
            }
            else
            {
                // If enemy finished moving, needs to start attacking.
                if (!attackStarted)
                {
                    startAttacking.Invoke();
                    attackStarted = true;
                }
            }
            CheckIfReached(ref _moveToTarget, flankInfo.flankTarget, ref _moveToPosition);
            CheckIfReached(ref _moveToPosition, flankInfo.flankPosition, ref _moveToPlayer);
            canMove = !(Vector3.Distance(transform.position, finalPosition) < minDistanceToTargetPosition && _moveToPlayer);
        }
        
        private void TryMoveFlankingTowards(bool flankingStage, Vector2 target, float speed)
        {
            if (flankingStage)
            {
                SetForwardVector(target);
                transform.position = Vector3.MoveTowards(transform.position, target, speed);
            }
        }

        private void CheckIfReached(ref bool isInFlankingStage, Vector2 target, ref bool nextFlankingStage)
        {
            if (Vector3.Distance(transform.position, target) < 0.01f && isInFlankingStage)
            {
                isInFlankingStage = false;
                nextFlankingStage = true;
            }
        }
    }
}

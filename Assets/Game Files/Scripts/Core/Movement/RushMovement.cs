using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Core.Movement
{
    public class RushMovement : EnemyMovement
    {
        // Update is called once per frame
        void Update()
        {
            Move();
        }

        protected override void Move()
        {
            step = moveSpeed * Time.deltaTime;
            if (disableMovement) return;
            if (canMove)
            {
                transform.position = Vector3.MoveTowards(transform.position, finalPosition, step);
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
            canMove = !(Vector3.Distance(transform.position, finalPosition) < minDistanceToTargetPosition);
            
        }
    }
}

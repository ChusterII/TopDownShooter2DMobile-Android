using System;
using System.Xml.Schema;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Managers;
using Random = UnityEngine.Random;

namespace WarKiwiCode.Game_Files.Scripts.Core
{
    public class MeleeEnemy : Enemy
    {
        [Range(0f,1f)]
        [SerializeField] private float chanceToFlank;

        [SerializeField] private MovementType movement;
        
        
        private Transform _player;
        private bool _moveToTarget = true;
        private bool _moveToPosition;
        private bool _moveToPlayer;

        // Start is called before the first frame update
        private void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        private void Update()
        {
            Move();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            //SetMovementType();
            movementType = movement;
        }

        // TODO: Modificar EL CHANCE DE FLANKEAR
        
        
        protected override void Move()
        {
            float step = moveSpeed * Time.deltaTime;

            if (disableMovement) return;
            switch (movementType)
            {
                case MovementType.Rush:
                    if (canMove)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, playerPosition, step);
                    }
                    canMove = !(Vector3.Distance(transform.position, playerPosition) < 0.9f);
                    break;
                case MovementType.Flank:
                    FlankInfo flankInfo = _spawnManager.GetFlankInfo(GetPosition());
                    if (canMove)
                    {
                        TryMoveFlankingTowards(_moveToTarget, flankInfo.flankTarget, step);
                        TryMoveFlankingTowards(_moveToPosition, flankInfo.flankPosition, step);
                        TryMoveFlankingTowards(_moveToPlayer, playerPosition, step);
                    }
                    CheckIfReached(ref _moveToTarget, flankInfo.flankTarget, ref _moveToPosition);
                    CheckIfReached(ref _moveToPosition, flankInfo.flankPosition, ref _moveToPlayer);
                    canMove = !(Vector3.Distance(transform.position, playerPosition) < 0.9f && _moveToPlayer);
                    break;
                case MovementType.Zigzag:
                    
                    
                    /*
                    *PSEUDOCODIGO TIME!!!
                    Zigzag:
                    - Enemigo spawnea.
                    - Escoje 2 valores (a y b) en rango [-3.5, 3.5]. Seran nuestros x's.
                    - Sea a el minimo de ambos valores, entonces (a+2) <= b (diferencia de 2 para que sea valido. Si no, escoja otro par)
                    - Elije el valor que esté mas cerca de él, y se mueve ahi (e.g. Move(a, y)) = ZIG
                    - Calcule el valor del ZAG dependiendo si esta arriba o abajo:
                    --- Si esta arriba, z = Random.Range[0.5,1]. Entonces Move(b, y-z)
                    --- Si esta abajo, z = Random.Range[0.5,1]. Entonces Move(b, y+z)
                    - Hacer lo mismo del ZAG para calcular el siguiente ZIG.
                    - Revisa si en el siguiente movimiento:
                    --- Si esta arriba, si y <= 2.5, pare y haga rush.
                    --- Si esta abajo, si y >= 2.5, pare y haga rush.
                     * 
                     */
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void TryMoveFlankingTowards(bool flankingStage, Vector2 target, float speed)
        {
            if (flankingStage)
            {
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

        protected override void AttackPlayer()
        {
            throw new NotImplementedException();
        }
        
        private static float GetChanceToFlank()
        {
            return Random.value;
        }

        private void SetMovementType()
        {
            movementType = GetChanceToFlank() <= chanceToFlank ? MovementType.Flank : MovementType.Rush;
            
        }

    }
}

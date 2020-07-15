using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Managers;

namespace WarKiwiCode.Game_Files.Scripts.Core.Movement
{
    public abstract class EnemyMovement : MonoBehaviour
    {
        
        [SerializeField] protected float moveSpeed;

        protected float step;
        protected bool canMove = true;
        protected bool disableMovement;
        protected Vector3 playerPosition;
        private bool _spawnedTop;
        protected SpawnManager spawnManager;
        private readonly float _screenMiddle = Screen.height / 2f; 
        
        protected abstract void Move();
        
        public void InitializeMovement()
        {
            DisableEnemyMovement(false);
            GetSpawnArea(GetPosition());
            spawnManager = SpawnManager.instance;
            playerPosition = FindNearestPlayer();
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

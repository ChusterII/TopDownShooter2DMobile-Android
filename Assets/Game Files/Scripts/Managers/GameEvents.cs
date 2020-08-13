using System;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents instance;

        private void Awake()
        {
            instance = this;
        }

        public event Action<SpawnPositionAndArea> OnEnemyDeath;
        public void EnemyDeath(SpawnPositionAndArea deathAreaAndPosition) => OnEnemyDeath?.Invoke(deathAreaAndPosition);
        
        public event Action<string> OnPowerUpSpent;
        public void PowerUpSpent(string player) => OnPowerUpSpent?.Invoke(player);
    }
}

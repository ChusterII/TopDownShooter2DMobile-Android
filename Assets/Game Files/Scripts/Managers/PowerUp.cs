using System;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class PowerUp : MonoBehaviour, ISpawnable
    {
        public float spawnChance;
        public string powerUpName;
        protected SpawnAreaName spawnArea;
        
        public abstract void Use();

        private void Start()
        {
            Use();
        }

        public void SetSpawnArea(SpawnAreaName areaName) => spawnArea = areaName;

        public void SetEnemySpawnName(string spawnName)
        {
        }
    }
    
    
}

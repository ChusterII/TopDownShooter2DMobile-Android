using System;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class PowerUp : MonoBehaviour, ISpawnable, IPooledObject
    {
        public float spawnChance;
        public string powerUpName;
        public Sprite topSprite;
        public Sprite bottomSprite;
        protected SpawnAreaName spawnArea;
        public bool isEnabled;

        private SpriteRenderer _spriteRenderer;
        
        public abstract void Use();

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSpawnArea(SpawnAreaName areaName) => spawnArea = areaName;

        public void SetEnemySpawnName(string spawnName)
        {
        }

        private void SetSprite()
        {
            switch (spawnArea)
            {
                case SpawnAreaName.Top:
                    _spriteRenderer.sprite = topSprite;
                    break;
                case SpawnAreaName.Bottom:
                    _spriteRenderer.sprite = bottomSprite;
                    break;
            }
        }

        public void OnSpawn()
        {
            SetSprite();
            Use();
        }
    }
    
    
}

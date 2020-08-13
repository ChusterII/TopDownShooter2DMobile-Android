using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using WarKiwiCode.Game_Files.Scripts.Core.Attack;
using WarKiwiCode.Game_Files.Scripts.Core.Movement;
using WarKiwiCode.Game_Files.Scripts.Managers;
using WarKiwiCode.Game_Files.Scripts.Projectiles;

namespace WarKiwiCode.Game_Files.Scripts.Core
{
    [RequireComponent(typeof(EnemyMovement))]
    public sealed class Enemy : MonoBehaviour, IPooledObject, ISpawnable
    {
        [SerializeField] private Light2D enemyLight;
        [SerializeField] private int maxHealth;
        [SerializeField] private SpriteRenderer healthBarSprite;
        [SerializeField] private SpriteRenderer healthBackgroundSprite;
        [Tooltip("The Bar container")]
        [SerializeField] private Transform healthBar;

        private int _currentHealth;
        private SpriteRenderer _sprite;
        private Collider2D _collider;
        private EnemyMovement _enemyMovement;
        private IAttack _enemyAttack;
        // TODO: Check if we need the ISpawnable interface that sets the spawn area name
        private SpawnAreaName _spawnArea;
        //private SpawnManager _spawnManager;
        private Vector3 _playerPosition;
        private string _enemySpawnName;
        private float _originalLightIntensity;

        private void Awake()
        {
            _originalLightIntensity = enemyLight.intensity;
        }

        public void OnSpawn()
        {
            _collider = GetComponent<Collider2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _enemyMovement = GetComponent<EnemyMovement>();
            _enemyAttack = GetComponent<IAttack>();
            //_spawnManager = SpawnManager.instance;
            _sprite.DOFade(1, 0.01f);
            _collider.enabled = true;
            enemyLight.intensity = _originalLightIntensity;
            _currentHealth = maxHealth;
            HideHealthBar();
            _enemyMovement.InitializeMovement();
            if (CheckForAttackInterface())
            {
                _enemyAttack.InitializeAttack();
            }
            _playerPosition = _enemyMovement.FindNearestPlayer();
        }

        private void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (!HealthBarEnabled())
            {
                ShowHealthBar();
            }
            float healthPercent = (float)_currentHealth / maxHealth;
            UpdateHealthBar(healthPercent);
            Vector3 bloodDirection = (GetPosition() - _playerPosition).normalized;
            BloodParticleSystemHandler.Instance.SpawnBlood(GetPosition(), bloodDirection);
            if (_currentHealth <= 0)
            {
                _enemyMovement.DisableEnemyMovement(true);
                _currentHealth = 0;
                HideHealthBar();
                // Die
                Timing.RunCoroutine(EnemyDeath());
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<BulletData>() != null)
            {
                int damage = other.gameObject.GetComponent<BulletData>().GetDamage();
                
                // Collided with a bullet
                TakeDamage(damage);
                Timing.RunCoroutine(DisableBullet(other.gameObject).CancelWith(gameObject));
            }
        }

        private static IEnumerator<float> DisableBullet(GameObject bullet)
        {
            yield return Timing.WaitForSeconds(0.07f);
            bullet.SetActive(false);
        }

        
        private IEnumerator<float> EnemyDeath()
        {
            // TODO: Might wanna change the enemy death animation.
            _collider.enabled = false;
            enemyLight.intensity = 0;
            if (CheckForAttackInterface())
            {
                _enemyAttack.EndAttackWhenDead();
            }
            _sprite.DOFade(0, 1f);
            yield return Timing.WaitForSeconds(1f);
            GameObject thisEnemy = gameObject;
            thisEnemy.SetActive(false);
            // TODO: Revisar que esto sirva
            _enemyMovement.RemoveFinalPositionFromList();
            GameEvents.instance.EnemyDeath(new SpawnPositionAndArea(_spawnArea, GetPosition()));
            ObjectPoolerManager.instance.Despawn(_enemySpawnName, thisEnemy);
        }

        private void HideHealthBar()
        {
            healthBarSprite.DOFade(0, 0.01f);
            healthBackgroundSprite.DOFade(0, 0.01f);
            healthBarSprite.enabled = false;
            healthBackgroundSprite.enabled = false;
        }

        private void ShowHealthBar()
        {
            healthBarSprite.enabled = true;
            healthBackgroundSprite.enabled = true;
            
            Sequence showHealthBar = DOTween.Sequence();
            showHealthBar.Append(healthBarSprite.DOFade(1, 0.25f));
            showHealthBar.Join(healthBackgroundSprite.DOFade(1, 0.25f));
            showHealthBar.Play();
        }
        
        private Vector3 GetPosition() => transform.position;
        
        private bool HealthBarEnabled() => healthBarSprite.enabled && healthBackgroundSprite.enabled;

        private void UpdateHealthBar(float value) => healthBar.DOScaleX(value, 0.1f);

        public void SetSpawnArea(SpawnAreaName areaName) => _spawnArea = areaName;

        public void SetEnemySpawnName(string spawnName) => _enemySpawnName = spawnName;
        
        // TEST
        private bool CheckForAttackInterface()
        {
            return GetComponent<IAttack>() != null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Xml;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using WarKiwiCode.Game_Files.Scripts.Managers;
using WarKiwiCode.Game_Files.Scripts.Projectiles;
using Random = UnityEngine.Random;


namespace WarKiwiCode.Game_Files.Scripts.Core
{
    public abstract class Enemy : MonoBehaviour, IPooledObject, ISpawnable
    {
        [SerializeField] protected Light2D enemyLight;
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected int maxHealth;
        
        
        [SerializeField] private SpriteRenderer healthBarSprite;
        [SerializeField] private SpriteRenderer healthBackgroundSprite;
        [Tooltip("The Bar container")]
        [SerializeField] private Transform healthBar;


        protected enum MovementType
        {
            Rush,
            Flank,
            Zigzag
        }

        private int _currentHealth;
        protected Rigidbody2D rb;
        private readonly float _screenMiddle = Screen.height / 2f; 
        protected Vector3 playerPosition;
        protected bool spawnedTop;
        private SpriteRenderer _sprite;
        private Collider2D _collider;
        protected bool canMove = true;
        protected bool disableMovement;
        protected MovementType movementType;
        protected SpawnAreaName spawnArea;
        protected SpawnManager _spawnManager;
        

        protected void Start()
        {
            /*
            _collider = GetComponent<Collider2D>();
            _sprite = GetComponent<SpriteRenderer>();
            currentHealth = maxHealth;
            HideHealthBar();
            GetSpawnArea(GetPosition());
            FindNearestPlayer();
            SetMovementType();
            */
        }

        public virtual void OnSpawn()
        {
            _collider = GetComponent<Collider2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _spawnManager = SpawnManager.instance;
            _sprite.DOFade(1, 0.01f);
            _collider.enabled = true;
            enemyLight.intensity = 1;
            _currentHealth = maxHealth;
            disableMovement = false;
            HideHealthBar();
            GetSpawnArea(GetPosition());
            FindNearestPlayer();
        }

        protected abstract void Move();
        protected abstract void AttackPlayer();

        private void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (!HealthBarEnabled())
            {
                ShowHealthBar();
            }

            float healthPercent = (float)_currentHealth / maxHealth;
            
            UpdateHealthBar(healthPercent);
            
            Vector3 bloodDirection = (GetPosition() - playerPosition).normalized;
            BloodParticleSystemHandler.Instance.SpawnBlood(GetPosition(), bloodDirection);
            if (_currentHealth <= 0)
            {
                disableMovement = true;
                _currentHealth = 0;
                HideHealthBar();
                //Die
                Timing.RunCoroutine(EnemyDeath());
            }
        }
    
        protected void OnCollisionEnter2D(Collision2D other)
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

        protected Vector3 GetPosition()
        {
            return transform.position;
        }

        private void GetSpawnArea(Vector3 startingPosition)
        {
            spawnedTop = startingPosition.y > _screenMiddle;
        }

        private void FindNearestPlayer()
        {
            playerPosition = spawnedTop ? GameObject.FindWithTag("PlayerTop").transform.position : GameObject.FindWithTag("PlayerBottom").transform.position;
        }

        private IEnumerator<float> EnemyDeath()
        {
            // TODO: Might wanna change the enemy death animation.
            
            _collider.enabled = false;
            enemyLight.intensity = 0;
            _sprite.DOFade(0, 1f);
            yield return Timing.WaitForSeconds(1f);
            GameObject thisEnemy = gameObject;
            thisEnemy.SetActive(false);
            // BUG: HAY QUE HACER OVERRIDE A ESTE DESPAWN!
            ObjectPoolerManager.instance.Despawn("EnemySlowMelee", thisEnemy);
        }

        private void HideHealthBar()
        {
            healthBarSprite.DOFade(0, 0.01f);
            healthBackgroundSprite.DOFade(0, 0.01f);
            healthBarSprite.enabled = false;
            healthBackgroundSprite.enabled = false;
        }

        private bool HealthBarEnabled()
        {
            return healthBarSprite.enabled && healthBackgroundSprite.enabled;
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

        private void UpdateHealthBar(float value)
        {
            healthBar.DOScaleX(value, 0.1f);
        }

        
        public void SetSpawnArea(SpawnAreaName areaName)
        {
            spawnArea = areaName;
        }
    }
}

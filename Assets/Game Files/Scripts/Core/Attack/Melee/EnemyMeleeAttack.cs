using System;
using System.Collections.Generic;
using System.Resources;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;
using WarKiwiCode.Game_Files.Scripts.Core.Movement;
using WarKiwiCode.Game_Files.Scripts.Managers;
using WarKiwiCode.Game_Files.Scripts.Managers.Player;

namespace WarKiwiCode.Game_Files.Scripts.Core.Attack.Melee
{
    public class EnemyMeleeAttack : MonoBehaviour, IAttack
    {
        [Header("Weapon settings")]
        [SerializeField] protected EnemyWeapon weapon;
        

        private float _cooldownTimer;
        protected State state;
        private EnemyMovement _enemyMovement;
        private bool _hasToAttack;
        private bool _hasToCharge;
        private float _originalLightIntensity;
        private Color _originalLightColor;
        protected Vector3 playerPosition;
        private PlayerHealth _playerHealth;
        private Collider2D[] _nearestPlayerCollider;
        private ObjectPoolerManager _objectPooler;
        private Collider2D _collider;
        private Sequence _sequence;

        
        
        protected enum State
        {
            Moving,
            StartCharging,
            Charging,
            Attacking,
            Cooldown
        }

        // Start is called before the first frame update
        void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            _cooldownTimer += Time.deltaTime;
            switch (state)
            {
                case State.Moving:
                    break;
                case State.StartCharging:
                    state = State.Charging;
                    StartChargingAttack();
                    break;
                case State.Charging:
                    _hasToCharge = true;
                    break;
                case State.Attacking:
                    state = State.Cooldown;
                    _hasToAttack = true;
                    _cooldownTimer = 0;
                    break;
                case State.Cooldown:
                    if (_cooldownTimer > weapon.weaponCooldown)
                    {
                        state = State.StartCharging;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Attack()
        {
            // Make player call Take Damage
            Vector3 closestPointToPlayer = _collider.ClosestPoint(playerPosition);
            _objectPooler.Spawn("HitParticles", closestPointToPlayer, Quaternion.identity, SpawnAreaName.None);
            _playerHealth.TakeDamage(weapon.meleeDamage);
        }

        public void InitializeAttack()
        {
            _nearestPlayerCollider = new Collider2D[3];
            _cooldownTimer = 0;
            _hasToCharge = false;
            _enemyMovement = GetComponent<EnemyMovement>();
            _objectPooler = ObjectPoolerManager.instance;
            state = State.Moving;
            playerPosition = _enemyMovement.FindNearestPlayer();
            _playerHealth = GetNearestPlayerHealth(playerPosition);
            Timing.RunCoroutine(AttackSignal().CancelWith(gameObject));
            Timing.RunCoroutine(ChargeSignal().CancelWith(gameObject));
        }

        private PlayerHealth GetNearestPlayerHealth(Vector3 position)
        {
            Physics2D.OverlapCircleNonAlloc(position, 0.05f, _nearestPlayerCollider);
            return _nearestPlayerCollider[0].gameObject.GetComponent<PlayerHealth>();
        }

        private void StartChargingAttack()
        {
            _enemyMovement.SetForwardVector(playerPosition);
        }

        private IEnumerator<float> ChargeSignal()
        {
            while (true)
            {
                while (!_hasToCharge)
                {
                    // Wait until the hasToAttack flag has been raised.
                    yield return Timing.WaitForOneFrame;
                }
                yield return Charge();
                _hasToCharge = false;
                state = State.Attacking;
                yield return Timing.WaitForOneFrame;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private float Charge()
        {
            // Charging sequence
            Sequence chargeSequence = DOTween.Sequence();
            Vector3 originalPosition = GetPosition();
            Vector3 chargeDirection = -(playerPosition - GetPosition()).normalized;
            Vector3 chargePosition = GetPosition() + chargeDirection;
            chargeSequence.Append(transform.DOMove(chargePosition, weapon.meleeComboSpeed));
            chargeSequence.Append(transform.DOMove(originalPosition, 0.01f));
            _sequence = chargeSequence.Play();
            return Timing.WaitForSeconds(weapon.meleeComboSpeed);
        }

        private IEnumerator<float> AttackSignal()
        {
            while (true)
            {
                while (!_hasToAttack)
                {
                    // Wait until the hasToAttack flag has been raised.
                    yield return Timing.WaitForOneFrame;
                }
                // Attack and loop again
                Attack();
                _hasToAttack = false;
                yield return Timing.WaitForOneFrame;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public void StartAttack() => state = State.StartCharging;

        public void EndAttackWhenDead()
        {
            /*if (_sequence.active)
            {
                _sequence.Kill();
            }*/
        }
        
        protected Vector3 GetPosition() => transform.position;
    }
}

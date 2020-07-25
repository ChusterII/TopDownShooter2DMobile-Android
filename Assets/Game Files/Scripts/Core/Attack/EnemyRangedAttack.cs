using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using CodeMonkey.Utils;
using DG.Tweening;
using MEC;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core.Movement;
using WarKiwiCode.Game_Files.Scripts.Utilities;

namespace WarKiwiCode.Game_Files.Scripts.Core.Attack
{
    public abstract class EnemyRangedAttack : MonoBehaviour, IAttack
    {
        [Header("Weapon settings")]
        [SerializeField] protected EnemyWeapon weapon;
        [SerializeField] protected float aimSpeed;
        [Header("Aim transforms")]
        [SerializeField] protected Transform aimTransform;
        [SerializeField] protected Transform aimLeftTransform;
        [SerializeField] protected Transform aimRightTransform;
        [Header("Aim sprites")]
        [SerializeField] protected SpriteRenderer aimLeftSpriteRenderer;
        [SerializeField] protected SpriteRenderer aimRightSpriteRenderer;
        [Header("Testing")] 
        
        // End testing variables

        private Color _aimColor;
        private const float StartAimAngle = 30f;
        private const float ShootAngle = 0f;
        protected EnemyMovement enemyMovement;
        protected Vector3 playerPosition;
        private float _aimAngle;
        protected State state;
        protected EnemyType.RangedEnemyType enemyType;
        private bool _hasToAttack;

        private float _cooldownTimer;
        protected float rofTimer;
        
        protected enum State
        {
            Moving,
            StartAiming,
            Aiming,
            Attacking,
            Cooldown
        }

        void Update()
        {
            if (weapon.isAutomatic)
            {
                rofTimer += Time.deltaTime;
            }
            _cooldownTimer += Time.deltaTime;
            switch (state)
            {
                case State.Moving:
                    break;
                case State.StartAiming:
                    state = State.Aiming;
                    enemyMovement.SetForwardVector(playerPosition);
                    StartAimingAtTarget();
                    break;
                case State.Aiming:
                    UpdateAimingAtTarget();
                    break;
                case State.Attacking:
                    state = State.Cooldown;
                    _hasToAttack = true;
                    _cooldownTimer = 0;
                    break;
                case State.Cooldown:
                    if (_cooldownTimer > weapon.weaponCooldown)
                    {
                        state = State.StartAiming;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected abstract void Attack();
        
        public void InitializeAttack()
        {
            _cooldownTimer = 0;
            _aimColor = aimLeftSpriteRenderer.color;
            enemyMovement = GetComponent<EnemyMovement>();
            state = State.Moving;
            HideAim();
            if (GetComponent<IRangedAttackType>() != null)
            {
                // TODO : Might not need this
                enemyType = GetComponent<IRangedAttackType>().GetAttackType();
            }
            playerPosition = enemyMovement.FindNearestPlayer();
            Timing.RunCoroutine(AttackSignal().CancelWith(gameObject));
        }

        private void StartAimingAtTarget()
        {
            ShowAim();
            _aimColor = Color.yellow;
            aimLeftSpriteRenderer.DOFade(0, 0.001f);
            aimRightSpriteRenderer.DOFade(0, 0.001f);
            SetAimAngle(StartAimAngle);
            AimAtPosition(playerPosition);
        }

        private void UpdateAimingAtTarget()
        {
            AimAtPosition(playerPosition);
            SetAimAngle(_aimAngle - aimSpeed * Time.deltaTime);
            SetAimColor(IsInsideFiringAngle() ? Color.red : Color.yellow, 1 - _aimAngle / StartAimAngle);
            
            if (_aimAngle <= ShootAngle)
            {
                HideAim();
                // Change state to Attacking
                state = State.Attacking;
            }
        }
        
        private void SetAimColor(Color color, float fadeDuration)
        {
            _aimColor = color;
            aimLeftSpriteRenderer.color = _aimColor;
            aimRightSpriteRenderer.color = _aimColor;
            aimLeftSpriteRenderer.DOFade(1, fadeDuration);
            aimRightSpriteRenderer.DOFade(1, fadeDuration);
        }

        private void SetAimAngle(float angle)
        {
            _aimAngle = angle;
            aimLeftTransform.localEulerAngles = new Vector3(0, 0, +angle);
            aimRightTransform.localEulerAngles = new Vector3(0, 0, -angle);
            // Change the length of the aim sprite depending on the distance to the player
            float distanceToPlayer = Vector3.Distance(GetPosition(), playerPosition);
            float yScale = aimLeftTransform.localScale.y;
            aimLeftTransform.localScale = new Vector3(distanceToPlayer, yScale, 1);
            aimRightTransform.localScale = new Vector3(distanceToPlayer, yScale, 1);
        }

        private void AimAtPosition(Vector3 aimPosition) 
        {
            Vector3 aimDir = (aimPosition - GetPosition()).normalized;
            aimTransform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(aimDir));
        }

        private bool IsInsideFiringAngle()
        {
            const float firingAngle = 4.5f;
            return _aimAngle < firingAngle;
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

        private void HideAim() => aimTransform.gameObject.SetActive(false);

        private void ShowAim() => aimTransform.gameObject.SetActive(true);

        protected Vector3 GetPosition() => transform.position;

        public void StartAttack() => state = State.StartAiming;
        public void EndAttackWhenDead() => HideAim();
    }
}

using System;
using CodeMonkey.Utils;
using DG.Tweening;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core.Movement;

namespace WarKiwiCode.Game_Files.Scripts.Core.Attack
{
    public abstract class EnemyAttack : MonoBehaviour
    {

        [SerializeField] protected float fireCooldown;
        [SerializeField] protected int numberOfBulletsFired;
        
        [SerializeField] protected float aimSpeed;

        [SerializeField] protected Transform aimTransform;
        [SerializeField] protected Transform aimLeftTransform;
        [SerializeField] protected Transform aimRightTransform;
        
        [SerializeField] protected SpriteRenderer aimLeftSpriteRenderer;
        [SerializeField] protected SpriteRenderer aimRightSpriteRenderer;
        
        
        protected Color aimColor;
        protected const float StartAimAngle = 20f;
        protected const float ShootAngle = 0f;
        private EnemyMovement _enemyMovement;
        private Vector3 _playerPosition;
        private float _aimAngle;
        protected State state;
        protected EnemyType enemyType;
        protected EnemyWeaponData _weaponData;
        
        protected enum State
        {
            Normal,
            Aiming,
            Attacking
        }
        
        protected abstract void Attack();

        public void InitializeAttackPattern()
        {
            aimColor = aimLeftSpriteRenderer.color;
            _enemyMovement = GetComponent<EnemyMovement>();
            HideAim();
            state = State.Normal;
            enemyType = GetComponent<IRangedAttackType>().GetAttackType();
            _playerPosition = _enemyMovement.FindNearestPlayer();
        }

        protected void StartAimingAtTarget()
        {
            ShowAim();
            aimColor = Color.yellow;
            aimLeftSpriteRenderer.DOFade(0, 0.001f);
            aimRightSpriteRenderer.DOFade(0, 0.001f);
            SetAimAngle(StartAimAngle);
            AimAtPosition(_playerPosition);
        }

        protected void UpdateAimingAtTarget()
        {
            AimAtPosition(_playerPosition);
            
            SetAimAngle(_aimAngle - aimSpeed * Time.deltaTime);

            SetAimColor(IsInsideFiringAngle() ? Color.red : Color.yellow, 1 - _aimAngle / StartAimAngle);

            if (_aimAngle <= ShootAngle) {
                // Shoot!
                HideAim();
                // Call to shoot
                Debug.Log("SHOT PLAYER!");
            }
        }
        
        private void SetAimColor(Color color, float fadeDuration) {
            aimColor = color;
            aimLeftSpriteRenderer.color = aimColor;
            aimRightSpriteRenderer.color = aimColor;
            aimLeftSpriteRenderer.DOFade(1, fadeDuration);
            aimRightSpriteRenderer.DOFade(1, fadeDuration);
        }


        private void SetAimAngle(float angle)
        {
            _aimAngle = angle;
            aimLeftTransform.localEulerAngles = new Vector3(0, 0, +angle);
            aimRightTransform.localEulerAngles = new Vector3(0, 0, -angle);

            float distanceToPlayer = Vector3.Distance(GetPosition(), _playerPosition);
            aimLeftTransform.localScale = new Vector3(distanceToPlayer, 1, 1);
            aimRightTransform.localScale = new Vector3(distanceToPlayer, 1, 1);
        }

        private void AimAtPosition(Vector3 aimPosition) {
            Vector3 aimDir = (aimPosition - GetPosition()).normalized;
            aimTransform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(aimDir));
        }

        private bool IsInsideFiringAngle()
        {
            const float firingAngle = 4.5f;
            return _aimAngle < firingAngle;
        }

        private void HideAim()
        {
            aimTransform.gameObject.SetActive(false);
        }

        private void ShowAim()
        {
            aimTransform.gameObject.SetActive(true);
        }
        
        protected Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core;

namespace WarKiwiCode.Game_Files.Scripts.Projectiles
{
    public class BulletMovement : MonoBehaviour, IPooledObject
    {
        [SerializeField] private float bulletSpeed = 40f;
        
        private Rigidbody2D _rigidBody;

        // Start is called before the first frame update
        void Start()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }
        
        
        public void OnSpawn()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            LaunchBullet();
        }

        private void LaunchBullet()
        {
            _rigidBody.velocity = transform.up * bulletSpeed;
        }

    }
}

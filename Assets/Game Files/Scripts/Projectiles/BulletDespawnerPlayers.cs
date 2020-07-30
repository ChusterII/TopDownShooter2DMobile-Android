using System;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Projectiles
{
    public class BulletDespawnerPlayers : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("EnemyBullet"))
            {
                print("Got hit!");
                other.gameObject.SetActive(false);
            }
            
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("EnemyBullet"))
            {
                print("Got hit!");
                other.gameObject.SetActive(false);
            }
        }
    }
    
}

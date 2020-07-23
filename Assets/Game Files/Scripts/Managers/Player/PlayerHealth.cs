using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Projectiles;

namespace WarKiwiCode.Game_Files.Scripts.Managers.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        private void TakeDamage(int damage)
        {
            // Take damage
            print(gameObject.name + " took damage.");
        }

        private void OnTriggerEnter2D(Collider2D other)
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
    }
}

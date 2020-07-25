using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Projectiles
{
    public class BulletDespawner : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Timing.RunCoroutine(DisableBullet(other.gameObject));
        }

        private static IEnumerator<float> DisableBullet(GameObject bullet)
        {
            yield return Timing.WaitForSeconds(0.2f);
            bullet.SetActive(false);
        }
    }
}

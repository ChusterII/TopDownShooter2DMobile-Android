using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Projectiles
{
    public class BulletData : MonoBehaviour
    {
        [SerializeField] private int bulletDamage;

        public int GetDamage()
        {
            return bulletDamage;
        }
    
    }
}

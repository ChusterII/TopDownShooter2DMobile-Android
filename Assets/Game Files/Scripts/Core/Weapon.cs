using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Core
{
    public enum WeaponType
    {
        Pistol,
        Shotgun,
        Smg,
        AssaultRifle
    }
    
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
    public class Weapon : ScriptableObject
    {
        public new string name;
        [Tooltip("Max clip size")] 
        public WeaponType weaponType;
        public int maxAmmo;
        public int startingClips;
        [Tooltip("Damage per bullet")]
        public float damage;
        public float rateOfFire;
        public float reloadTime;
        [Tooltip("Allows the touch to be held for continuous fire?")]
        public bool allowsHold;
        [Tooltip("Error margin in Degrees from the touched position")]
        public float spread;
        [Tooltip("How many bullets per shot will this gun spawn")]
        public float numberOfBullets;

        public int splashRadius;
        public int splashDamage;
        public bool isPiercing;
       

        public AudioClip firingSound;
        public AudioClip reloadingSound;

    }
}

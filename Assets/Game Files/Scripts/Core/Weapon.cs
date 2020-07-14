using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Core
{
    public enum WeaponType
    {
        Pistol,
        Shotgun,
        AssaultRifle
    }
    
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
    public class Weapon : ScriptableObject
    {
        public new string name;
        [Tooltip("Max clip size")] 
        public WeaponType weaponType;
        public int maxAmmo;
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

        public AudioClip firingSound;
        public AudioClip reloadingSound;

    }
}

using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using WarKiwiCode.Game_Files.Scripts.Core;
using Random = UnityEngine.Random;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    [System.Serializable]
    public class MuzzleFlash
    {
        public string tag;
        public GameObject flash;
        public float flashSpeed;
    }
    public class PlayerController : MonoBehaviour
    {
        [System.Serializable]
        public class IntEvent : UnityEvent<int>{}
        [System.Serializable]
        public class FloatEvent : UnityEvent<float>{}
        [System.Serializable]
        public class BoolEvent : UnityEvent<bool>{}
        [System.Serializable]
        public class SpawnShellEvent : UnityEvent<Vector3, Vector3, string>{}

        public IntEvent updateMaxAmmo;
        public IntEvent updateCurrentAmmo;
        public UnityEvent resetReloadProgressBar;
        public BoolEvent disableInput;
        public BoolEvent displayReload;
        public SpawnShellEvent spawnShell;
        public FloatEvent onReload;
        //public IntEvent onSwitchWeapon;
        
        [SerializeField] private PlayerInputManager playerInput;
        [SerializeField] private Transform bulletSpawnPosition;
        [SerializeField] private float maxShootingAngle = 70f;
        [FormerlySerializedAs("_muzzleFlashes")] 
        [SerializeField] private List<MuzzleFlash> muzzleFlashes = new List<MuzzleFlash>();
        [SerializeField] private Weapon[] weapons;
        

        private ObjectPoolerManager _objectPooler;
        private AudioSource _audioSource;
        private static Vector2 _playerPosition;
        private float _touchedAngle;
        private Weapon _currentWeapon;
        private int _currentAmmo;
        private float _weaponRofTimer = 0;
        private float _reloadTimer = 0;
        private Dictionary<WeaponType, Weapon> _availableWeapons;
        private bool isReloading;
        
        private void Start()
        {
            _objectPooler = ObjectPoolerManager.instance;
            _audioSource = GetComponent<AudioSource>();
            _playerPosition = new Vector2(transform.position.x, transform.position.y);
            
            // Add the weapon array into a dictionary for more clear usage
            _availableWeapons = new Dictionary<WeaponType, Weapon>();
            foreach (Weapon weapon in weapons)
            {
                _availableWeapons.Add(weapon.weaponType, weapon);
            }

            _currentWeapon = _availableWeapons[WeaponType.Pistol]; // Make the Pistol the default weapon
            UiManager.instance.SetWeaponButtonText(_currentWeapon.name);
            // Set Hold Allowed value
            playerInput.SetHold(_currentWeapon.allowsHold);
            _currentAmmo = _currentWeapon.maxAmmo;
            updateMaxAmmo.Invoke(_currentWeapon.maxAmmo);
            updateCurrentAmmo.Invoke(_currentWeapon.maxAmmo);
        }

        private void Update()
        {
            _weaponRofTimer += Time.deltaTime;
            if (isReloading)
            {
                _reloadTimer += Time.deltaTime;
            }
            
        }

        public void Shoot(Vector2 shootPosition)
        {
            // Get the direction of the touch
            Vector3 direction = (playerInput.GetTouchedPosition() - _playerPosition).normalized;

            // Get the angle between the up direction and where the touch was. 
            _touchedAngle = Vector2.SignedAngle(Vector2.up, direction);

            // Line up the rotation towards the touched point on screen
            transform.up = direction;

            // Clamp the shooting angle
            ClampShootingAngle();
            
            // Cache the position
            Vector3 spawnPosition = bulletSpawnPosition.position;
            
            // Check rate of fire
            if (_weaponRofTimer > _currentWeapon.rateOfFire && _currentAmmo > 0)
            {
                // FIRE ZE MISSILES!!
                Timing.RunCoroutine(ShowMuzzleFlash());

                for (int i = 0; i < _currentWeapon.numberOfBullets; i++)
                {
                    // Set the spread of the bullet in terms of rotation
                    float randomSpread = Random.Range(-_currentWeapon.spread, _currentWeapon.spread);
                    Quaternion spreadRotation = Quaternion.Euler(0, 0, randomSpread) * transform.rotation;
                    
                    // Spawn the bullet
                    switch (_currentWeapon.weaponType)
                    {
                        case WeaponType.Pistol:
                            _objectPooler.Spawn("Pistol Bullet", spawnPosition, spreadRotation, SpawnAreaName.None);
                            break;
                        case WeaponType.Shotgun:
                            _objectPooler.Spawn("Shotgun Bullet", spawnPosition, spreadRotation, SpawnAreaName.None);
                            break;
                        case WeaponType.AssaultRifle:
                            _objectPooler.Spawn("Assault Rifle Bullet", spawnPosition, spreadRotation, SpawnAreaName.None);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                }
                
                // Play firing sound
                _audioSource.PlayOneShot(_currentWeapon.firingSound);
                
                // Spawn the casing
                spawnShell.Invoke(spawnPosition, direction, _currentWeapon.name);
                
                // Decrease the ammo count
                _currentAmmo--;
                
                // Call the event for updating the counter
                updateCurrentAmmo.Invoke(_currentAmmo);
                
                // Reset the ROF timer
                _weaponRofTimer = 0;
            }
            if (_currentAmmo <= 0)
            {
               // Out of ammo, time to reload
               Timing.RunCoroutine(Reload());
            }
            
        }

        private IEnumerator<float> Reload()
        {
            // Reset the progress bar, disable player input and display the UI elements
            resetReloadProgressBar.Invoke();
            disableInput.Invoke(true);
            displayReload.Invoke(true);
            
            // Reset the timer and start it
            _reloadTimer = 0;
            isReloading = true;
            
            // Play reloading sound
            _audioSource.PlayOneShot(_currentWeapon.reloadingSound);

            // Get the completed % of the reload and update the progress bar with it
            while (_reloadTimer < _currentWeapon.reloadTime)
            {
                float completedPercentage = _reloadTimer / _currentWeapon.reloadTime;
                if (completedPercentage > 1)
                {
                    completedPercentage = 1;
                }
                onReload.Invoke(completedPercentage);
                yield return Timing.WaitForOneFrame;
            }
            
            // Stop the timer
            isReloading = false;
            
            // Set the new ammo level
            _currentAmmo = _currentWeapon.maxAmmo;
            updateCurrentAmmo.Invoke(_currentAmmo);

            // Disable the UI elements and enable input again
            displayReload.Invoke(false);
            
            yield return Timing.WaitForSeconds(0.05f);
            disableInput.Invoke(false);
            
        }

        public void SwitchCurrentWeapon()
        {
            switch (_currentWeapon.name)
            {
                case "Pistol":
                    _currentWeapon = _availableWeapons[WeaponType.Shotgun];
                    break;
                case "Shotgun":
                    _currentWeapon = _availableWeapons[WeaponType.AssaultRifle];
                    break;
                case "Assault Rifle":
                    _currentWeapon = _availableWeapons[WeaponType.Pistol];
                    break;
            }
            updateMaxAmmo.Invoke(_currentWeapon.maxAmmo);
            _currentAmmo = _currentWeapon.maxAmmo; // Testing purposes
            updateCurrentAmmo.Invoke(_currentAmmo);
            UiManager.instance.SetWeaponButtonText(_currentWeapon.name);
            // Set Hold Allowed value
            playerInput.SetHold(_currentWeapon.allowsHold);
        }
        
        

        private IEnumerator<float> ShowMuzzleFlash()
        {
            MuzzleFlash muzzleFlash = muzzleFlashes.Find(flash => flash.tag.Equals(_currentWeapon.name));
            muzzleFlash.flash.SetActive(true);
            yield return Timing.WaitForSeconds(muzzleFlash.flashSpeed);
            muzzleFlash.flash.SetActive(false);
            
        }

        private void ClampShootingAngle()
        {
            if (playerInput.TouchedTop())
            {
                // Clamp the rotation if the touch exceeds the maxShootingAngle value
                if (_touchedAngle > maxShootingAngle)
                {
                    transform.rotation = Quaternion.Euler(0, 0, maxShootingAngle);
                }
                else if (_touchedAngle < -maxShootingAngle)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -maxShootingAngle);
                }
            }
            else
            {
                // Calculate the bottom Shooting angle since object is rotated 180° 
                float bottomMaxShootingAngle = 180 - maxShootingAngle;
                
                // Clamp the rotation if the touch exceeds the bottomMaxShootingAngle value
                // Also, we need to check if the value is negative or positive to avoid confusion
                if (_touchedAngle < bottomMaxShootingAngle && _touchedAngle > 0)
                {
                    //print("Touched: " + _touchedAngle + " / Max: " + bottomMaxShootingAngle);
                    transform.rotation = Quaternion.Euler(0, 0, bottomMaxShootingAngle);
                }
                else if (_touchedAngle > -bottomMaxShootingAngle && _touchedAngle < 0)
                {
                    //print("Touched: " + _touchedAngle + " / Max: " + -bottomMaxShootingAngle);
                    transform.rotation = Quaternion.Euler(0, 0, -bottomMaxShootingAngle);
                }
            }
        }

    }
}

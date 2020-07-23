using UnityEngine;
using WarKiwiCode.Game_Files.Scripts.Core;

namespace WarKiwiCode.Game_Files.Scripts.Managers.Player
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        private PlayerController _playerController;
        private enum Weapons
        {
            Pistol,
            Shotgun,
            Rifle
        }
        
        // Start is called before the first frame update
        void Start()
        {
            _playerController = GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

    public class Pistol : Weapon
    {
        protected int maxAmmo = 32;
    }
}

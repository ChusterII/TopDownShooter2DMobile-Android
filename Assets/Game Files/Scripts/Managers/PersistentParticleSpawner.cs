using CodeMonkey.Utils;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class PersistentParticleSpawner : MonoBehaviour
    {
        public void SpawnShell(Vector3 position, Vector3 direction, string weaponName)
        {
            Vector3 quadPosition = position;
            quadPosition += (direction * -0.1f) * 2f; // Offset to get it close to the weapon

            float shellAngle = Random.Range(-90f, 90f);
            Vector3 shellMoveDirection = UtilsClass.ApplyRotationToVector(direction, shellAngle);
            
            int uvIndex = 0;
            switch (weaponName)
            {
                case "Pistol":
                    uvIndex = 0;
                    break;
                case "Shotgun":
                    print("shotgun uv" + uvIndex);
                    uvIndex = 1;
                    break;
                case "Assault Rifle":
                    uvIndex = 2;
                    break;
            }
            
            ShellParticleSystemHandler.Instance.SpawnShell(quadPosition, shellMoveDirection, uvIndex);
        }

        
    }
}

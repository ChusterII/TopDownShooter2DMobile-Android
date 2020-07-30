using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Utilities
{
    public class HitParticleManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Invoke(nameof(Disable), 0.51f);
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}

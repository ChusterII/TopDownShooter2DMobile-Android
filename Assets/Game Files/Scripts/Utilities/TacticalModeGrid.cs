using System;
using UnityEngine;
using UnityEngine.Events;

namespace WarKiwiCode.Game_Files.Scripts.Utilities
{
    public class TacticalModeGrid : MonoBehaviour
    {
        [Serializable]
        public class BoolEvent : UnityEvent<bool>{}
        
        public BoolEvent displayZinji;
        public BoolEvent displayBlake;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Zinji"))
            {
                displayZinji.Invoke(false);
            }
            else if (other.gameObject.tag.Equals("Blake"))
            {
                displayBlake.Invoke(false);
            }
        }
    }
}

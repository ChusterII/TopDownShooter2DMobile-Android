using System;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class Managers : MonoBehaviour
    {
        public static Managers instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

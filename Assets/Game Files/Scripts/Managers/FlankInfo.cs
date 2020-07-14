using System;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    [Serializable]
    public class FlankInfo
    {
        public Vector2 flankTarget;
        public Vector2 flankPosition;

        public FlankInfo(Vector2 flankTarget, Vector2 flankPosition)
        {
            this.flankTarget = flankTarget;
            this.flankPosition = flankPosition;
        }
    }
}

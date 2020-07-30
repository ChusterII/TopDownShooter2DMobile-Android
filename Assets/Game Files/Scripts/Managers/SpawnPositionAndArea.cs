using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    [System.Serializable]
    public class SpawnPositionAndArea
    {
        public SpawnAreaName spawnAreaName;
        public Vector2 spawnPosition;

        public SpawnPositionAndArea(SpawnAreaName areaName, Vector2 position)
        {
            spawnAreaName = areaName;
            spawnPosition = position;
        }
    }
}
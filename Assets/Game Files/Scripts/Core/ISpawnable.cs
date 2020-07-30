using WarKiwiCode.Game_Files.Scripts.Managers;

namespace WarKiwiCode.Game_Files.Scripts.Core
{
    public interface ISpawnable
    {
        void SetSpawnArea(SpawnAreaName areaName);
        void SetEnemySpawnName(string spawnName);
    }
}

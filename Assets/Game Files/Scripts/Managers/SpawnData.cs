using WarKiwiCode.Game_Files.Scripts.Difficulty;
using WarKiwiCode.Game_Files.Scripts.Utilities;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class SpawnData
    {
        public MeleeEnemySpawnData[] MeleeData { get; }
        public RangedEnemySpawnData[] RangedData { get; }
        public MeleeMovementProbability MeleeMovementProbability { get; }
        public RangedMovementProbability RangedMovementProbability { get; }
        public EnemyTypesToNames NameContainer { get; }

        public SpawnData(MeleeEnemySpawnData[] meleeData, RangedEnemySpawnData[] rangedData,
            MeleeMovementProbability meleeMovementProbability, RangedMovementProbability rangedMovementProbability, EnemyTypesToNames nameContainer)
        {
            MeleeData = meleeData;
            RangedData = rangedData;
            MeleeMovementProbability = meleeMovementProbability;
            RangedMovementProbability = rangedMovementProbability;
            NameContainer = nameContainer;
        }
    }
}
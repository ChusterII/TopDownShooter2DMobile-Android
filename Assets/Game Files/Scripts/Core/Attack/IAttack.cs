namespace WarKiwiCode.Game_Files.Scripts.Core.Attack
{
    public interface IAttack
    {
        void InitializeAttack();
        void StartAttack();
        void EndAttackWhenDead();
    }
}

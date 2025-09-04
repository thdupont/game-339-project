using JRPG.Core;

namespace JRPG.Services.Abilities
{
    public interface IAbility
    {
        string Name { get; }
        int MpCost { get; }
        Targeting Targeting { get; }
        bool CanUse(JRPGCharacter user, BattleContext ctx, JRPGCharacter target = null);
        AbilityResult Use(JRPGCharacter user, BattleContext ctx, JRPGCharacter target = null);
    }
}
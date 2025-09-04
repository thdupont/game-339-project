using System;
using System.Linq;
using JRPG.Core;

namespace JRPG.Services.Abilities
{
    public class FirestormAbility : IAbility
    {
        public string Name => "Firestorm";
        public int MpCost => 8;
        public Targeting Targeting => Targeting.AllEnemies;

        public bool CanUse(JRPGCharacter user, BattleContext ctx, JRPGCharacter target = null)
            => user.IsAlive && user.MP >= MpCost && ctx.LivingEnemiesOf(user).Any();

        public AbilityResult Use(JRPGCharacter user, BattleContext ctx, JRPGCharacter target = null)
        {
            var result = new AbilityResult(Name, user);

            if (!CanUse(user, ctx))
            {
                ctx.Logger.Warning("{0} tried to cast {1} but couldn't.", user.Name, Name);
                return result;
            }

            if (!user.SpendMP(MpCost))
            {
                ctx.Logger.Warning("{0} lacked MP for {1}.", user.Name, Name);
                return result;
            }

            int rawBase = (int)Math.Round(user.Attack * 1.25) + 10;
            ctx.Logger.Debug("{0} casts {1} (raw {2}).", user.Name, Name, rawBase);

            foreach (var enemy in ctx.LivingEnemiesOf(user))
            {
                int dealt = enemy.ApplyDamage(DamageType.Magical, rawBase);
                result.Effects.Add((enemy, dealt, "DMG"));
                ctx.Logger.Information("{0} takes {1} damage from {2}. ({3}/{4} HP)",
                    enemy.Name, dealt, Name, enemy.HP, enemy.MaxHP);
            }

            return result;
        }
    }
}
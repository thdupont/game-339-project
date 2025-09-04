using System;
using System.Linq;
using JRPG.Core;

namespace JRPG.Services.Abilities
{
    public class MendAbility : IAbility
    {
        public string Name => "Mend";
        public int MpCost => 5;
        public Targeting Targeting => Targeting.SingleAlly;

        public bool CanUse(JRPGCharacter user, BattleContext ctx, JRPGCharacter target = null)
        {
            if (!user.IsAlive || user.MP < MpCost) return false;
            if (target == null) return ctx.LivingAlliesOf(user, includeSelf: true).Any(a => a.HP < a.MaxHP);
            return target.IsAlive && target.PartyId == user.PartyId && target.HP < target.MaxHP;
        }

        public AbilityResult Use(JRPGCharacter user, BattleContext ctx, JRPGCharacter target = null)
        {
            var result = new AbilityResult(Name, user);

            if (!CanUse(user, ctx, target))
            {
                ctx.Logger.Warning("{0} tried to cast {1} but couldn't.", user.Name, Name);
                return result;
            }

            if (!user.SpendMP(MpCost))
            {
                ctx.Logger.Warning("{0} lacked MP for {1}.", user.Name, Name);
                return result;
            }

            int healAmt = 20 + (int)Math.Ceiling(user.Attack * 0.8);
            target = target ?? ctx.LivingAlliesOf(user, includeSelf: true)
                .OrderBy(a => (double)a.HP / a.MaxHP)
                .FirstOrDefault();

            if (target == null)
            {
                ctx.Logger.Warning("{0} had no valid Mend targets.", user.Name);
                return result;
            }

            int before = target.HP;
            target.Heal(healAmt);
            int healed = target.HP - before;

            result.Effects.Add((target, healed, "HEAL"));
            ctx.Logger.Information("{0} heals {1} for {2}. ({3}/{4} HP)",
                user.Name, target.Name, healed, target.HP, target.MaxHP);

            return result;
        }
    }
}
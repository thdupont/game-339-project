using System.Linq;
using JRPG.Core;
using JRPG.Services;
using JRPG.Services.Abilities;

namespace JRPG.Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var logger = new ConsoleLog { MinimumLevel = LogLevel.Debug }; // tune as desired
            var ctx = new BattleContext(logger);

            var hero = new JRPGCharacter("Hero", 120, 30, attack: 40, defense: 10, physicalDefense: 15, magicDefense: 5, partyId: 1);
            var mage = new JRPGCharacter("Mage", 90, 50, attack: 28, defense: 6, physicalDefense: 8, magicDefense: 12, partyId: 1);

            var slimeA = new JRPGCharacter("Slime A", 80, 0, attack: 18, defense: 5, physicalDefense: 5, magicDefense: 10, partyId: 2);
            var slimeB = new JRPGCharacter("Slime B", 85, 0, attack: 20, defense: 6, physicalDefense: 6, magicDefense: 9, partyId: 2);

            mage.Abilities.Add(new FirestormAbility());
            hero.Abilities.Add(new MendAbility());

            ctx.Participants.AddRange(new[] { hero, mage, slimeA, slimeB });

            logger.Information("{0}", hero.ToString());
            logger.Information("{0}", mage.ToString());
            logger.Information("{0}", slimeA.ToString());
            logger.Information("{0}", slimeB.ToString());

            // Mage casts Firestorm
            var fire = mage.Abilities.OfType<FirestormAbility>().First();
            var fireRes = fire.Use(mage, ctx);
            logger.Information("{0}", fireRes.ToString());

            // Slime A hits Hero
            int dealt = slimeA.AttackTarget(hero, DamageType.Physical);
            logger.Information("{0} hits {1} for {2}. {1} HP: {3}/{4}", slimeA.Name, hero.Name, dealt, hero.HP, hero.MaxHP);

            // Hero casts Mend on self
            var mend = hero.Abilities.OfType<MendAbility>().First();
            var mendRes = mend.Use(hero, ctx, target: hero);
            logger.Information("{0}", mendRes.ToString());

            logger.Information("Final:");
            logger.Information("{0}", hero.ToString());
            logger.Information("{0}", mage.ToString());
            logger.Information("{0}", slimeA.ToString());
            logger.Information("{0}", slimeB.ToString());
        }
    }
}
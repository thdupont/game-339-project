using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JRPG.Core;
using JRPG.Services.Abilities;
using NUnit.Framework;

namespace JRPG.Services.Tests
{
    [TestFixture]
    [SuppressMessage("Assertion", "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of ClassicAssert.AreEqual(expected, actual)")]
    public class CombatTests
    {
        private static CombatTestArena MakeArena()
        {
            var log = new TestLog(TestContext.Out) { MinimumLevel = LogLevel.Debug };
            var ctx = new BattleContext(log);

            var hero = new JRPGCharacter("Hero", 120, 30, 40, 10, 15, 5, partyId: 1);
            var mage = new JRPGCharacter("Mage", 90, 50, 28, 6, 8, 12, partyId: 1);
            var slimeA = new JRPGCharacter("Slime A", 80, 0, 18, 5, 5, 10, partyId: 2);
            var slimeB = new JRPGCharacter("Slime B", 85, 0, 20, 6, 6, 9, partyId: 2);

            mage.Abilities.Add(new FirestormAbility());
            hero.Abilities.Add(new MendAbility());

            ctx.Participants.AddRange(new[] { hero, mage, slimeA, slimeB });
            return new CombatTestArena(ctx, hero, mage, slimeA, slimeB, log);
        }

        [Test]
        public void DemoTest()
        {
            var i = 10;
            Assert.AreEqual(10 / 2, 5);
            var isEqualFive = 10 / 2 == 5;
            Assert.IsTrue(!isEqualFive);
        }
        
        [Test]
        public void Firestorm_HitsAllEnemies_ConsumesMP()
        {
            var arena = MakeArena();
            int mpBefore = arena.Mage.MP;

            var fire = arena.Mage.Abilities.OfType<FirestormAbility>().First();
            Assert.IsTrue(fire.CanUse(arena.Mage, arena.Ctx));

            var res = fire.Use(arena.Mage, arena.Ctx);
            Assert.That(arena.Mage.MP, Is.EqualTo(mpBefore - fire.MpCost));
            Assert.That(res.Effects.Count, Is.EqualTo(2));
            Assert.IsTrue(arena.SlimeA.HP < arena.SlimeA.MaxHP);
            Assert.IsTrue(arena.SlimeB.HP < arena.SlimeB.MaxHP);

            // Some logs should exist
            Assert.IsTrue(arena.Log.Entries.Any(e => e.text.Contains("casts Firestorm")));
            Assert.IsTrue(arena.Log.Entries.Any(e => e.text.Contains("takes")));
        }

        [Test]
        public void Mend_HealsSingleAlly_And_ConsumesMP()
        {
            var arena = MakeArena();

            // Make hero injured
            arena.SlimeA.AttackTarget(arena.Hero, DamageType.Physical);
            int hpInjured = arena.Hero.HP;

            var mend = arena.Hero.Abilities.OfType<MendAbility>().First();
            int mpBefore = arena.Hero.MP;

            Assert.IsTrue(mend.CanUse(arena.Hero, arena.Ctx, arena.Hero));
            var res = mend.Use(arena.Hero, arena.Ctx, arena.Hero);

            Assert.That(arena.Hero.MP, Is.EqualTo(mpBefore - mend.MpCost));
            Assert.That(arena.Hero.HP, Is.GreaterThan(hpInjured));
            Assert.That(res.Effects.Count, Is.EqualTo(1));
            Assert.IsTrue(arena.Log.Entries.Any(e => e.text.Contains("heals")));
        }

        [Test]
        public void Logger_MinimumLevel_FiltersEntries()
        {
            var arena = MakeArena();
            arena.Log.MinimumLevel = LogLevel.Warning;

            arena.Ctx.Logger.Debug("debug message");
            arena.Ctx.Logger.Information("info message");
            arena.Ctx.Logger.Warning("warn message");
            arena.Ctx.Logger.Error("error message");

            Assert.IsFalse(arena.Log.Entries.Any(e => e.level == LogLevel.Debug));
            Assert.IsFalse(arena.Log.Entries.Any(e => e.level == LogLevel.Information));
            Assert.IsTrue(arena.Log.Entries.Any(e => e.level == LogLevel.Warning));
            Assert.IsTrue(arena.Log.Entries.Any(e => e.level == LogLevel.Error));
        }

        [Test]
        public void CannotUseAbilities_WithoutMP_OrTargets()
        {
            var arena = MakeArena();

            // Kill enemies so Firestorm has no valid targets
            arena.SlimeA.ApplyDamage(DamageType.Magical, 9999);
            arena.SlimeB.ApplyDamage(DamageType.Magical, 9999);

            var fire = arena.Mage.Abilities.OfType<FirestormAbility>().First();
            Assert.IsFalse(fire.CanUse(arena.Mage, arena.Ctx));

            // Drain hero MP for Mend
            arena.Hero.SpendMP(arena.Hero.MP);
            var mend = arena.Hero.Abilities.OfType<MendAbility>().First();
            Assert.IsFalse(mend.CanUse(arena.Hero, arena.Ctx, arena.Hero));
        }
    }
}
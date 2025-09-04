using System.Collections.Generic;
using System.Linq;
using JRPG.Core;

namespace JRPG.Services
{
    public class BattleContext
    {
        public List<JRPGCharacter> Participants { get; } = new();
        public ILog Logger { get; }

        public BattleContext(ILog logger) { Logger = logger; }

        public IEnumerable<JRPGCharacter> AlliesOf(JRPGCharacter c, bool includeSelf = false)
            => Participants.Where(x => x.PartyId == c.PartyId && (includeSelf || x != c));

        public IEnumerable<JRPGCharacter> EnemiesOf(JRPGCharacter c)
            => Participants.Where(x => x.PartyId != c.PartyId);

        public IEnumerable<JRPGCharacter> LivingAlliesOf(JRPGCharacter c, bool includeSelf = false)
            => AlliesOf(c, includeSelf).Where(x => x.IsAlive);

        public IEnumerable<JRPGCharacter> LivingEnemiesOf(JRPGCharacter c)
            => EnemiesOf(c).Where(x => x.IsAlive);
    }
}
using System.Collections.Generic;
using System.Linq;

namespace JRPG.Services.Abilities
{
    public class AbilityResult
    {
        public string AbilityName { get; }
        public JRPGCharacter User { get; }
        public List<(JRPGCharacter Target, int Amount, string Kind)> Effects { get; } = new ();

        public AbilityResult(string abilityName, JRPGCharacter user)
        {
            AbilityName = abilityName;
            User = user;
        }

        public override string ToString()
        {
            if (Effects.Count == 0) return $"{User.Name} used {AbilityName}, but nothing happened.";
            return string.Join("\n", Effects.Select(e =>
                $"{User.Name} used {AbilityName}: {e.Kind} {e.Amount} → {e.Target.Name}"));
        }
    }
}
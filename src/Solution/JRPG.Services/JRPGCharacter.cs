using System;
using System.Collections.Generic;
using JRPG.Core;
using JRPG.Services.Abilities;

namespace JRPG.Services
{
    public class JRPGCharacter
    {
        public string Name { get; }

        // Core stats
        public int MaxHP { get; private set; }
        public int HP     { get; private set; }
        public int MaxMP  { get; private set; }
        public int MP     { get; private set; }

        // Offensive / defensive stats
        public int Attack           { get; set; }
        public int Defense          { get; set; }
        public int PhysicalDefense  { get; set; }
        public int MagicDefense     { get; set; }

        public int PartyId { get; set; }
        public List<IAbility> Abilities { get; } = new();

        public bool IsAlive => HP > 0;

        public JRPGCharacter(
            string name,
            int maxHP, int maxMP,
            int attack,
            int defense,
            int physicalDefense,
            int magicDefense,
            int partyId = 0)
        {
            Name = name;

            MaxHP = Math.Max(1, maxHP);
            MaxMP = Math.Max(0, maxMP);
            HP = MaxHP;
            MP = MaxMP;

            Attack = Math.Max(0, attack);
            Defense = Math.Max(0, defense);
            PhysicalDefense = Math.Max(0, physicalDefense);
            MagicDefense = Math.Max(0, magicDefense);

            PartyId = partyId;
        }

        public int GetDefense(DamageType type)
        {
            int typed = type == DamageType.Physical ? PhysicalDefense : MagicDefense;
            return Math.Max(0, Defense + typed);
        }

        /// Mitigation: final = raw * 100 / (100 + totalDefense), min 1 if raw > 0.
        public int ApplyDamage(DamageType type, int rawDamage)
        {
            if (rawDamage <= 0 || !IsAlive) return 0;
            int def = GetDefense(type);
            int mitigated = (int)Math.Round(rawDamage * 100.0 / (100.0 + def));
            mitigated = Math.Max(1, mitigated);
            HP = Math.Max(0, HP - mitigated);
            return mitigated;
        }

        public int AttackTarget(JRPGCharacter target, DamageType type = DamageType.Physical)
        {
            if (!IsAlive) return 0;
            return target.ApplyDamage(type, Attack);
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || !IsAlive) return;
            HP = Math.Min(MaxHP, HP + amount);
        }

        public bool SpendMP(int amount)
        {
            if (amount <= 0) return true;
            if (MP < amount) return false;
            MP -= amount;
            return true;
        }

        public void RestoreMP(int amount)
        {
            if (amount <= 0) return;
            MP = Math.Min(MaxMP, MP + amount);
        }

        public void Revive(int hp = 1)
        {
            HP = Math.Min(MaxHP, Math.Max(1, hp));
        }

        public override string ToString()
            => $"{Name} | HP {HP}/{MaxHP} | MP {MP}/{MaxMP} | ATK {Attack} | DEF {Defense} | PDEF {PhysicalDefense} | MDEF {MagicDefense}";
    }
}
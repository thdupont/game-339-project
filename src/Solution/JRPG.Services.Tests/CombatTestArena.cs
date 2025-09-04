namespace JRPG.Services.Tests
{
    public class CombatTestArena
    {
        public CombatTestArena(BattleContext ctx, JRPGCharacter hero, JRPGCharacter mage, JRPGCharacter slimeA, JRPGCharacter slimeB, TestLog log)
        {
            Ctx = ctx;
            Hero = hero;
            Mage = mage;
            SlimeA = slimeA;
            SlimeB = slimeB;
            Log = log;
        }

        public BattleContext Ctx { get; }
        public JRPGCharacter Hero { get; }
        public JRPGCharacter Mage { get; }
        public JRPGCharacter SlimeA { get; }
        public JRPGCharacter SlimeB { get; }
        public TestLog Log { get; }
    }
}
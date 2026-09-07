namespace DiceParser.Randomizers
{
    public static class Randomizer
    {
        public static IRandomizer FairDie() => new FairDie();
        public static IRandomizer FailDie() => new FailDie();
        public static IRandomizer CriticalDie() => new CriticalDie();
        public static IRandomizer DeckDie() => new DeckDie();
        public static IRandomizer GamblersAxionDie() => new GamblersAxionDie();
    }
}

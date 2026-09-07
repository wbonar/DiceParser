namespace DiceParser.Randomizers;
internal class FairDie : IRandomizer
{
    private readonly Random _rng = new();
    public int Roll(int max) => _rng.Next(1, max+1);
}

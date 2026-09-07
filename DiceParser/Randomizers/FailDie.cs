namespace DiceParser.Randomizers;
internal class FailDie : IRandomizer
{
    public int Roll(int max) => 1;
}

namespace DiceParser.Randomizers;

public class FailDie : IRandomizer
{
    public int Roll(int max) => 1;
}

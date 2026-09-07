namespace DiceParser.Test;

internal class RollingTests
{
    private readonly DiceParser _parser = new();


    [Test]
    [TestCase("1")]
    [TestCase("123")]
    [TestCase("-1")]
    public void StaticNumbers(string s)
    {
        var expected = int.Parse(s);
        Assert.That(_parser.Roll(s), Is.EqualTo(expected));
    }


    [Test]
    [TestCase("d2", 2)]
    [TestCase("d4", 4)]
    [TestCase("d6", 6)]
    [TestCase("d8", 8)]
    [TestCase("d10", 10)]
    [TestCase("d12", 12)]
    [TestCase("d20", 20)]
    [TestCase("d100", 100)]
    public void RollDie(string s, int maxValue)
    {
        // test with roller
        Assert.That(_parser.Roll(s), Is.GreaterThan(0));
        Assert.That(_parser.Roll(s), Is.LessThanOrEqualTo(maxValue));

        var criticalDie = new DiceParser(Randomizer.CriticalDie());
        Assert.That(criticalDie.Roll(s), Is.EqualTo(maxValue));

        var failDie = new DiceParser(Randomizer.FailDie());
        Assert.That(failDie.Roll(s), Is.EqualTo(1));
    }


    [Test]
    [TestCase("1d6", 1, 6)]
    [TestCase("2d6", 2, 12)]
    [TestCase("3d6", 3, 18)]
    [TestCase("4d6", 4, 24)]
    public void MultiRoll(string s, int minValue, int maxValue)
    {
        // test with roller
        Assert.That(_parser.Roll(s), Is.GreaterThanOrEqualTo(minValue));
        Assert.That(_parser.Roll(s), Is.LessThanOrEqualTo(maxValue));

        var criticalDie = new DiceParser(Randomizer.CriticalDie());
        Assert.That(criticalDie.Roll(s), Is.EqualTo(maxValue));

        var failDie = new DiceParser(Randomizer.FailDie());
        Assert.That(failDie.Roll(s), Is.EqualTo(minValue));
    }
}

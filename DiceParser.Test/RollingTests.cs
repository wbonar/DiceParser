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

    [Test]
    [TestCase(6)]
    [TestCase(20)]
    [TestCase(100)]
    public void DeckHasOneCardPerFace(int dieSize)
    {
        var die = Randomizer.DeckDie();
        var rolls = RollMany(die, dieSize, dieSize * 50).OrderBy(x => x);

        Assert.That(rolls.Distinct(), Is.EquivalentTo(Enumerable.Range(1, dieSize)));
    }


    [Test]
    [TestCase("4d6dl1", 3, 18)]
    [TestCase("4d6dh1", 3, 18)]
    [TestCase("4d6dl2", 2, 12)]
    [TestCase("4d6dh2", 2, 12)]
    public void DropDiceRange(string s, int minValue, int maxValue)
    {
        for (int i = 0; i < 100; i++)
        {
            Assert.That(_parser.Roll(s), Is.GreaterThanOrEqualTo(minValue));
            Assert.That(_parser.Roll(s), Is.LessThanOrEqualTo(maxValue));
        }
        var criticalDie = new DiceParser(Randomizer.CriticalDie());
        Assert.That(criticalDie.Roll(s), Is.EqualTo(maxValue));

        var failDie = new DiceParser(Randomizer.FailDie());
        Assert.That(failDie.Roll(s), Is.EqualTo(minValue));
    }


    [Test]
    [TestCase("4d6dl1", new int[] { 6, 1, 5, 3 }, 14)]
    [TestCase("4d6dh1", new int[] { 6, 1, 5, 3 }, 9)]
    [TestCase("4d6dl2", new int[] { 6, 1, 5, 3 }, 11)]
    [TestCase("4d6dh2", new int[] { 6, 1, 5, 3 }, 4)]
    [TestCase("4D6DL1", new int[] { 2, 4, 6, 5 }, 15)]
    [TestCase("3d8dh1", new int[] { 8, 3, 1 }, 4)]
    [TestCase("4d6kh3", new int[] { 6, 1, 5, 3 }, 14)]
    [TestCase("4d6kl3", new int[] { 6, 1, 5, 3 }, 9)]
    [TestCase("4d6kh2", new int[] { 6, 1, 5, 3 }, 11)]
    [TestCase("4d6kl2", new int[] { 6, 1, 5, 3 }, 4)]
    [TestCase("4d6kh1", new int[] { 6, 1, 5, 3 }, 6)]
    [TestCase("4d6kl1", new int[] { 6, 1, 5, 3 }, 1)]
    [TestCase("4D6KH3", new int[] { 2, 4, 6, 5 }, 15)]
    [TestCase("3d8kl1", new int[] { 8, 3, 1 }, 1)]
    [TestCase("4d6kh3+2", new int[] { 6, 1, 5, 3 }, 16)]
    [TestCase("2d20kh", new int[] { 20, 1 }, 20)]
    public void DropAndKeepDiceExact(string s, int[] rolls, int expected)
    {
        var parser = new DiceParser(new SequenceDie(rolls));
        Assert.That(parser.Roll(s), Is.EqualTo(expected));
    }


    [Test]
    [TestCase("4d6kh3", 3, 18)]
    [TestCase("4d6kl3", 3, 18)]
    [TestCase("4d6kh2", 2, 12)]
    [TestCase("4d6kl2", 2, 12)]
    [TestCase("4d6kh1", 1, 6)]
    [TestCase("4d6kl1", 1, 6)]
    public void KeepDiceRange(string s, int minValue, int maxValue)
    {
        for (int i = 0; i < 100; i++)
        {
            Assert.That(_parser.Roll(s), Is.GreaterThanOrEqualTo(minValue));
            Assert.That(_parser.Roll(s), Is.LessThanOrEqualTo(maxValue));
        }
        var criticalDie = new DiceParser(Randomizer.CriticalDie());
        Assert.That(criticalDie.Roll(s), Is.EqualTo(maxValue));

        var failDie = new DiceParser(Randomizer.FailDie());
        Assert.That(failDie.Roll(s), Is.EqualTo(minValue));
    }


    [Test]
    [TestCase("4d6kh3", "4d6dl1", new int[] { 6, 1, 5, 3 })]
    [TestCase("4d6kl3", "4d6dh1", new int[] { 6, 1, 5, 3 })]
    [TestCase("4d6kh1", "4d6dl3", new int[] { 6, 1, 5, 3 })]
    [TestCase("4d6kl1", "4d6dh3", new int[] { 2, 4, 6, 5 })]
    [TestCase("4d6kh2", "4d6dl2", new int[] { 6, 1, 5, 3 })]
    [TestCase("4d6kl2", "4d6dh2", new int[] { 6, 1, 5, 3 })]
    [TestCase("2d20kh1", "2d20dl1", new int[] { 20, 1 })]
    [TestCase("2d20kl1", "2d20dh1", new int[] { 20, 1 })]
    [TestCase("4D6KH3", "4d6DL1", new int[] { 2, 4, 6, 5 })]
    public void KeepIsEquivalentToComplementDrop(string keepExpression, string dropExpression, int[] rolls)
    {
        var keepResult = new DiceParser(new SequenceDie(rolls)).Roll(keepExpression);
        var dropResult = new DiceParser(new SequenceDie(rolls)).Roll(dropExpression);

        Assert.That(keepResult, Is.EqualTo(dropResult));
    }


    private static List<int> RollMany(IRandomizer die, int dieSize, int count)
    {
        var rolls = new List<int>(count);
        for (var i = 0; i < count; i++)
        {
            rolls.Add(die.Roll(dieSize));
        }

        return rolls;
    }


    private sealed class SequenceDie(params int[] rolls) : IRandomizer
    {
        private readonly Queue<int> _rolls = new(rolls);
        public int Roll(int max) => _rolls.Dequeue();
    }
}

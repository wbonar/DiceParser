namespace DiceParser.Test;

internal class DeckDieTests
{
    private const double RemainingFractionMin = 0.15;
    private const double RemainingFractionMax = 0.50;

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(6)]
    [TestCase(8)]
    [TestCase(10)]
    [TestCase(12)]
    [TestCase(20)]
    [TestCase(100)]
    public void RollsAreBetweenOneAndDieSize(int dieSize)
    {
        var die = Randomizer.DeckDie();
        var rolls = RollMany(die, dieSize, dieSize * 20);

        Assert.That(rolls, Has.All.GreaterThanOrEqualTo(1));
        Assert.That(rolls, Has.All.LessThanOrEqualTo(dieSize));
    }

    [Test]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(6)]
    [TestCase(8)]
    [TestCase(10)]
    [TestCase(12)]
    [TestCase(20)]
    [TestCase(100)]
    public void EachFaceAppearsAtMostOnceBeforeTheCutCard(int dieSize)
    {
        var die = Randomizer.DeckDie();
        var rolls = RollMany(die, dieSize, GuaranteedDrawsBeforeCut(dieSize));

        Assert.That(rolls, Is.Unique);
        Assert.That(rolls, Has.All.GreaterThanOrEqualTo(1));
        Assert.That(rolls, Has.All.LessThanOrEqualTo(dieSize));
    }

    [Test]
    [TestCase(6)]
    [TestCase(20)]
    [TestCase(100)]
    public void DeckHasOneCardPerFace(int dieSize)
    {
        var die = Randomizer.DeckDie();

        // Over enough shoes every face from the original 1..N deck must appear.
        var rolls = RollMany(die, dieSize, dieSize * 50);

        Assert.That(rolls.Distinct(), Is.EquivalentTo(Enumerable.Range(1, dieSize)));
    }

    [Test]
    public void DeckIsShuffledRatherThanDealtInFaceOrder()
    {
        const int dieSize = 20;
        const int trials = 30;
        var sequential = Enumerable.Range(1, GuaranteedDrawsBeforeCut(dieSize)).ToList();
        var matchingSequential = 0;

        for (var i = 0; i < trials; i++)
        {
            var rolls = RollMany(Randomizer.DeckDie(), dieSize, sequential.Count);
            if (rolls.SequenceEqual(sequential))
            {
                matchingSequential++;
            }
        }

        Assert.That(matchingSequential, Is.LessThan(trials),
            "A shuffled deck should not deal faces in ascending order every time.");
    }

    [Test]
    public void IndependentDecksProduceDifferentDealOrders()
    {
        const int dieSize = 20;
        const int trials = 40;
        var prefixes = new List<string>(trials);

        for (var i = 0; i < trials; i++)
        {
            var rolls = RollMany(Randomizer.DeckDie(), dieSize, GuaranteedDrawsBeforeCut(dieSize));
            prefixes.Add(string.Join(",", rolls));
        }

        Assert.That(prefixes.Distinct().Count(), Is.GreaterThan(1),
            "Separate shuffled decks should not all deal the same order.");
    }

    [Test]
    [TestCase(20)]
    [TestCase(100)]
    public void CanRollPastTheOriginalDeckSizeWithoutThrowing(int dieSize)
    {
        var die = Randomizer.DeckDie();

        Assert.DoesNotThrow(() => RollMany(die, dieSize, dieSize * 10));
    }

    [Test]
    public void ReshufflesBeforeTheDeckIsFullyDealt()
    {
        const int dieSize = 1000;

        var die = Randomizer.DeckDie();
        var rolls = RollMany(die, dieSize, dieSize);

        // WARNING: This test is probabilistic. It may fail if the random shuffle happens to produce a
        // sequence that repeats before the cut card is reached. Odds of failure are 1 in 1000 in this test
        Assert.That(rolls.Distinct().Count(), Is.LessThan(dieSize),
            "The cut card should reshuffle before every face is dealt from a single shoe.");
    }

    [Test]
    [TestCase(6)]
    [TestCase(20)]
    [TestCase(100)]
    public void FirstDuplicateNeverOccursBeforeTheEarliestCut(int dieSize)
    {
        var die = Randomizer.DeckDie();
        var uniqueWindow = GuaranteedDrawsBeforeCut(dieSize);
        var rolls = RollMany(die, dieSize, dieSize * 3);
        var firstDuplicateAt = IndexOfFirstDuplicate(rolls);

        Assert.That(firstDuplicateAt, Is.GreaterThanOrEqualTo(uniqueWindow));
    }

    [Test]
    public void TracksASeparateDeckPerDieSize()
    {
        const int smallDie = 6;
        const int largeDie = 20;
        var die = Randomizer.DeckDie();

        var largePrefix = RollMany(die, largeDie, 5);
        RollMany(die, smallDie, smallDie * 10);
        largePrefix.AddRange(RollMany(die, largeDie, GuaranteedDrawsBeforeCut(largeDie) - 5));

        Assert.That(largePrefix, Has.Count.EqualTo(GuaranteedDrawsBeforeCut(largeDie)));
        Assert.That(largePrefix, Is.Unique);
        Assert.That(largePrefix, Has.All.GreaterThanOrEqualTo(1));
        Assert.That(largePrefix, Has.All.LessThanOrEqualTo(largeDie));
    }

    [Test]
    [TestCase("d6", 1, 6)]
    [TestCase("d20", 1, 20)]
    [TestCase("2d6", 2, 12)]
    [TestCase("4d6", 4, 24)]
    public void ParserRollsWithDeckDieStayInRange(string expression, int minValue, int maxValue)
    {
        var parser = new DiceParser(Randomizer.DeckDie());

        for (var i = 0; i < 40; i++)
        {
            Assert.That(parser.Roll(expression), Is.InRange(minValue, maxValue));
        }
    }

    private static int GuaranteedDrawsBeforeCut(int dieSize) =>
        dieSize - (int)Math.Ceiling(dieSize * RemainingFractionMax) + 1;

    private static List<int> RollMany(IRandomizer die, int dieSize, int count)
    {
        var rolls = new List<int>(count);
        for (var i = 0; i < count; i++)
        {
            rolls.Add(die.Roll(dieSize));
        }

        return rolls;
    }

    private static int IndexOfFirstDuplicate(IReadOnlyList<int> rolls)
    {
        var seen = new HashSet<int>();
        for (var i = 0; i < rolls.Count; i++)
        {
            if (!seen.Add(rolls[i]))
            {
                return i;
            }
        }

        return rolls.Count;
    }
}

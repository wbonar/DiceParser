namespace DiceParser.Test;

internal class GamblersAxionDieTests
{
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
        var die = Randomizer.GamblersAxionDie();
        var rolls = RollMany(die, dieSize, dieSize * 20);

        Assert.That(rolls, Has.All.GreaterThanOrEqualTo(1));
        Assert.That(rolls, Has.All.LessThanOrEqualTo(dieSize));
    }

    [Test]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(6)]
    [TestCase(20)]
    [TestCase(100)]
    public void EveryFaceStaysInThePool(int dieSize)
    {
        var die = Randomizer.GamblersAxionDie();

        // Drawing a face removes every copy, then one of each face is added back,
        // so no face is ever eliminated from the pool.
        var rolls = RollMany(die, dieSize, dieSize * 50);

        Assert.That(rolls.Distinct(), Is.EquivalentTo(Enumerable.Range(1, dieSize)));
    }

    [Test]
    [TestCase(6)]
    [TestCase(20)]
    [TestCase(100)]
    public void CanRollManyTimesWithoutThrowing(int dieSize)
    {
        var die = Randomizer.GamblersAxionDie();

        Assert.DoesNotThrow(() => RollMany(die, dieSize, dieSize * 10));
    }

    [Test]
    public void JustDrawnFaceIsImmediatelyAddedBack()
    {
        const int dieSize = 6;
        const int trials = 400;

        var sawImmediateRepeat = false;
        for (var i = 0; i < trials; i++)
        {
            var die = Randomizer.GamblersAxionDie();
            if (die.Roll(dieSize) == die.Roll(dieSize))
            {
                sawImmediateRepeat = true;
                break;
            }
        }

        Assert.That(sawImmediateRepeat, Is.True,
            "After a face is drawn it is added back with every other face, so an immediate repeat must be possible.");
    }

    [Test]
    public void JustDrawnFaceIsColderThanTheOthers()
    {
        const int dieSize = 6;
        const int trials = 8000;
        var repeats = 0;

        for (var i = 0; i < trials; i++)
        {
            var die = Randomizer.GamblersAxionDie();
            if (die.Roll(dieSize) == die.Roll(dieSize))
            {
                repeats++;
            }
        }

        const double expectedRepeatRate = 1.0 / (2 * dieSize - 1);
        var repeatRate = repeats / (double)trials;

        Assert.That(repeatRate, Is.EqualTo(expectedRepeatRate).Within(0.025),
            "A just-drawn face should be the least likely outcome on the next roll.");
    }

    [Test]
    public void DrawingAFaceRemovesEveryCopyNotJustOne()
    {
        const int dieSize = 6;
        const int trials = 20000;
        var thirdMatchesFirst = 0;
        var thirdMatchesSecond = 0;
        var usableTrials = 0;

        for (var i = 0; i < trials; i++)
        {
            var die = Randomizer.GamblersAxionDie();
            var first = die.Roll(dieSize);
            var second = die.Roll(dieSize);
            if (first == second)
            {
                continue;
            }

            var third = die.Roll(dieSize);
            usableTrials++;
            if (third == first)
            {
                thirdMatchesFirst++;
            }

            if (third == second)
            {
                thirdMatchesSecond++;
            }
        }

        Assert.That(usableTrials, Is.GreaterThan(trials / 2));

        // After drawing A then B (A != B): A has 2 copies, B has 1, others have 3.
        // P(third == first) = 2/15, P(third == second) = 1/15.
        // Removing only one copy of B would leave A and B with the same count.
        Assert.That(thirdMatchesFirst, Is.GreaterThan(thirdMatchesSecond * 1.3),
            "Removing every copy of the drawn face should make it colder than the face drawn before it.");
    }

    [Test]
    public void TwoSidedDieFollowsAxiomOdds()
    {
        const int trials = 24000;
        var counts = new Dictionary<string, int>
        {
            ["111"] = 0,
            ["112"] = 0,
            ["121"] = 0,
            ["122"] = 0,
            ["211"] = 0,
            ["212"] = 0,
            ["221"] = 0,
            ["222"] = 0
        };

        for (var i = 0; i < trials; i++)
        {
            var die = Randomizer.GamblersAxionDie();
            var sequence = $"{die.Roll(2)}{die.Roll(2)}{die.Roll(2)}";
            counts[sequence]++;
        }

        // Fresh d2 pool is [1, 2]. Drawing a face removes every copy, then 1 and 2
        // are both added back. Exact sequence probabilities:
        // 111/222 = 1/24, 112/221 = 1/8, 121/212 = 2/9, 122/211 = 1/9.
        AssertFrequency(counts["111"], trials / 24.0);
        AssertFrequency(counts["222"], trials / 24.0);
        AssertFrequency(counts["112"], trials / 8.0);
        AssertFrequency(counts["221"], trials / 8.0);
        AssertFrequency(counts["121"], trials * 2 / 9.0);
        AssertFrequency(counts["212"], trials * 2 / 9.0);
        AssertFrequency(counts["122"], trials / 9.0);
        AssertFrequency(counts["211"], trials / 9.0);
    }

    [Test]
    public void TracksASeparatePoolPerDieSize()
    {
        var die = Randomizer.GamblersAxionDie();

        RollMany(die, 6, 80);
        var largeRolls = RollMany(die, 20, 200);

        Assert.That(largeRolls, Has.Some.GreaterThan(6),
            "Rolling a d6 should not leave the d20 pool containing only faces 1-6.");
        Assert.That(largeRolls.Distinct(), Is.EquivalentTo(Enumerable.Range(1, 20)));
    }

    [Test]
    [TestCase("d6", 1, 6)]
    [TestCase("d20", 1, 20)]
    [TestCase("2d6", 2, 12)]
    [TestCase("4d6", 4, 24)]
    public void ParserRollsWithGamblersAxionDieStayInRange(string expression, int minValue, int maxValue)
    {
        var parser = new DiceParser(Randomizer.GamblersAxionDie());

        for (var i = 0; i < 40; i++)
        {
            Assert.That(parser.Roll(expression), Is.InRange(minValue, maxValue));
        }
    }

    private static void AssertFrequency(int actual, double expected)
    {
        Assert.That(actual, Is.EqualTo(expected).Within(Math.Max(80, expected * 0.12)));
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
}

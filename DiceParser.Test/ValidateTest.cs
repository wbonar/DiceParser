namespace DiceParser.Test;

internal class ValidateTests
{
    private readonly DiceParser _parser = new();

    [Test]
    [TestCase("")]
    [TestCase("Hello")]
    [TestCase(" ")]
    [TestCase("5 * 5")]
    [TestCase("dl1")]
    [TestCase("dh1")]
    [TestCase("kh1")]
    [TestCase("kl1")]
    public void InvalidStrings(string s)
    {
        var ex = Assert.Throws<ArgumentException>(() => _parser.Roll(s));
        Assert.That(ex?.Message, Is.Not.Empty);
    }

    [Test]
    [TestCase("")]
    [TestCase("Hello")]
    [TestCase(" ")]
    [TestCase("5 * 5")]
    [TestCase("dl1")]
    [TestCase("dh1")]
    [TestCase("kh1")]
    [TestCase("kl1")]
    public void TestStringForValidity(string s)
    {
        Assert.That(_parser.IsValidRoll(s), Is.False);
    }

    [Test]
    [TestCase("4d6dl1")]
    [TestCase("4d6dh1")]
    [TestCase("4d6dl2")]
    [TestCase("4D6DL1")]
    [TestCase("4d6kh3")]
    [TestCase("4d6kl3")]
    [TestCase("4d6kh1")]
    [TestCase("4D6KH3")]
    [TestCase("2d20kl")]
    public void KeepAndDropDiceIsValid(string s)
    {
        Assert.That(_parser.IsValidRoll(s), Is.True);
    }
}

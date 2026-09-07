namespace DiceParser.Test;

internal class ValidateTests
{
    private readonly DiceParser _parser = new();

    [Test]
    [TestCase("")]
    [TestCase("Hello")]
    [TestCase(" ")]
    [TestCase("5 * 5")]
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
    public void TestStringForValidity(string s)
    {
        Assert.That(_parser.IsValidRoll(s), Is.False);
    }
}

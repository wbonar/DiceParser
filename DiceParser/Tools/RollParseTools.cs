using System.Text.RegularExpressions;

namespace DiceParser.Tools;

internal class RollParseTools
{
    internal static IEnumerable<RollEvaluator> SplitToFormatMembers(string inputString)
    {
        return NormalizeInput(inputString).Split(' ')
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => new RollEvaluator(s.Trim()));
    }


    internal static bool IsValidDiceSyntax(string input)
    {
        return ValidDiceRegex().IsMatch(input);
    }

    private static Regex ValidDiceRegex() => new(@"((\d*)?[dD](\d+)(?:(?:[dD][lL]|[dD][hH])\d*)?([+-]\d+)?){1}");

    private static string NormalizeInput(string input)
    {
        return input
            .ToLowerInvariant()
            .Replace(" ", "")
            .Replace("+", " +")
            .Replace("-", " -")
            .Replace("kh", " kh")
            .Replace("kl", " kl")
            .Replace("dh", " dh")
            .Replace("dl", " dl");
    }
        
}

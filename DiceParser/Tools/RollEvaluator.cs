using DiceParser.Randomizers;

namespace DiceParser.Tools;

internal class RollEvaluator(string rawString)
{
    internal RollCommands Command
    {
        get
        {
            var token = rawString.ToLowerInvariant();
            if (token is "+") return RollCommands.Add;
            if (token is "-") return RollCommands.Subtract;
            if (token.StartsWith("dl")) return RollCommands.DropLowest;
            if (token.StartsWith("dh")) return RollCommands.DropHighest;
            return RollCommands.Value;
        }
    }

    internal int ModifierCount
    {
        get
        {
            var token = rawString.ToLowerInvariant();
            if (token.Length <= 2) return 1;
            return int.TryParse(token[2..], out var count) ? count : 1;
        }
    }

    internal int Evaluate(IRandomizer randomizer) => EvaluateDice(randomizer).Sum();

    internal List<int> EvaluateDice(IRandomizer randomizer)
    {
        if (Command != RollCommands.Value) throw new ArgumentException("Invalid Roll String");

        if (int.TryParse(rawString, out var rollValue))
        {
            return [rollValue];
        }

        if (RollParseTools.IsValidDiceSyntax(rawString))
        {
            var diceFormat = rawString.Split('d', 'D');

            int.TryParse(diceFormat[0], out var dieCount);
            if (dieCount < 1) dieCount = 1;
            var dieSize = int.Parse(diceFormat[1]);

            var rolls = new List<int>(dieCount);
            for (var i = 0; i < dieCount; i++)
            {
                rolls.Add(randomizer.Roll(dieSize));
            }
            return rolls;
        }

        throw new ArgumentException("Invalid Roll String");
    }
}

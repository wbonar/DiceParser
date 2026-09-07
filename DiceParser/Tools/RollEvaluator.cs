using DiceParser.Randomizers;

namespace DiceParser.Tools;

internal class RollEvaluator(string rawString)
{
    internal RollCommands Command
    {
        get
        {
            return rawString switch
            {
                "+" => RollCommands.Add,
                "-" => RollCommands.Subtract,
                _ => RollCommands.Value
            };
        }
    }

    internal int Evaluate(IRandomizer randomizer)
    {
        if (Command != RollCommands.Value) throw new ArgumentException("Invalid Roll String");

        if (int.TryParse(rawString, out var rollValue))
        {
            return rollValue;
        }
        else if (RollParseTools.IsValidDiceSyntax(rawString))
        {
            var diceFormat = rawString.Split('d');

            int.TryParse(diceFormat[0], out var dieCount);
            if (dieCount < 1) dieCount = 1;
            var dieSize = int.Parse(diceFormat[1]);

            var total = 0;
            for (var i = 0; i < dieCount; i++)
            {
                total += randomizer.Roll(dieSize);
            }
            return total;
        }
        else
        {
            throw new ArgumentException("Invalid Roll String");
        }
    }
}
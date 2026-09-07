using DiceParser.Randomizers;
using DiceParser.Tools;

namespace DiceParser;


public class DiceParser : IDiceParser
{
    public DiceParser() { }

    public DiceParser(IRandomizer randomizer) => _randomizer = randomizer;

    private readonly IRandomizer _randomizer = new FairDie();

    public bool IsValidRoll(string s) => RollParseTools.IsValidDiceSyntax(s);

    public int Roll(string s)
    {
        var total = 0;
        var members = RollParseTools.SplitToFormatMembers(s).ToArray();
        if (members.Length == 0) throw new ArgumentException("Invalid Roll String");

        var currentDice = new List<int>();
        for (var i = 0; i < members.Length; i++)
        {
            switch (members[i].Command)
            {
                case RollCommands.Value:
                    {
                        currentDice = members[i].EvaluateDice(_randomizer);
                        total = currentDice.Sum();
                        break;
                    }
                case RollCommands.Add:
                    {
                        i++;
                        total += members[i].Evaluate(_randomizer);
                        break;
                    }
                case RollCommands.Subtract:
                    {
                        i++;
                        total -= members[i].Evaluate(_randomizer);
                        break;
                    }
                case RollCommands.DropLowest:
                    {
                        currentDice = DropDice(currentDice, members[i].ModifierCount, dropHighest: false);
                        total = currentDice.Sum();
                        break;
                    }
                case RollCommands.DropHighest:
                    {
                        currentDice = DropDice(currentDice, members[i].ModifierCount, dropHighest: true);
                        total = currentDice.Sum();
                        break;
                    }
                case RollCommands.KeepHighest:
                    {
                        currentDice = KeepDice(currentDice, members[i].ModifierCount, keepHighest: true);
                        total = currentDice.Sum();
                        break;
                    }
                case RollCommands.KeepLowest:
                    {
                        currentDice = KeepDice(currentDice, members[i].ModifierCount, keepHighest: false);
                        total = currentDice.Sum();
                        break;
                    }
            }
        }

        return total;
    }

    // Keep highest N == drop lowest (count - N); keep lowest N == drop highest (count - N).
    private static List<int> KeepDice(List<int> dice, int keepCount, bool keepHighest)
    {
        var dropCount = Math.Max(0, dice.Count - keepCount);
        return DropDice(dice, dropCount, dropHighest: !keepHighest);
    }

    private static List<int> DropDice(List<int> dice, int count, bool dropHighest)
    {
        if (dice.Count == 0) throw new ArgumentException("Invalid Roll String");

        return dropHighest
            ? dice.OrderByDescending(value => value).Skip(count).ToList()
            : dice.OrderBy(value => value).Skip(count).ToList();
    }
}

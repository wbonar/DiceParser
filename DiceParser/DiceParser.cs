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

        for (var i = 0; i < members.Length; i++)
        {
            switch (members[i].Command)
            {
                case RollCommands.Value:
                    {
                        total = members[i].Evaluate(_randomizer);
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

            }
        }

        return total;
    }
}

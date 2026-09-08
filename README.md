# DiceParser

A .NET library that interprets and rolls tabletop RPG dice expressions.

Parse strings like `4d6kh3` or `2d20dl1`, roll them, and get a numeric total. Swap in different randomizers when you want fair dice, guaranteed crits, or stranger distributions.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Quick start

```csharp
using DiceParser;
using DiceParser.Randomizers;

var parser = new DiceParser();

if (parser.IsValidRoll("4d6kh3"))
{
    int total = parser.Roll("4d6kh3");
}

// Inject a randomizer (default is a fair die)
var alwaysCrit = new DiceParser(Randomizer.CriticalDie());
int maxed = alwaysCrit.Roll("d20"); // 20
```

`Roll` throws `ArgumentException` when the expression is empty or cannot be evaluated.

## Dice syntax

Expressions are case-insensitive. Whitespace is ignored.

| Expression | Meaning |
| --- | --- |
| `d20` | One twenty-sided die (`d` with no count is one die) |
| `4d6` | Four six-sided dice, summed |
| `4d6dl1` | Roll 4d6, drop the lowest 1, sum the rest |
| `4d6dh2` | Roll 4d6, drop the highest 2, sum the rest |
| `4d6kh3` | Roll 4d6, keep the highest 3 |
| `4d6kl1` | Roll 4d6, keep the lowest 1 |
| `2d20kh` | Keep/drop count defaults to 1 when omitted |
| `12` | A constant value |

Standard polyhedral sizes work (`d2`, `d4`, `d6`, `d8`, `d10`, `d12`, `d20`, `d100`) as well as any other positive face count.

Keep and drop are complements: `4d6kh3` is the same as `4d6dl1`.

## Randomizers

`DiceParser` rolls through an `IRandomizer`. The default constructor uses a fair die. Pass another implementation to change how each face is chosen:

```csharp
public interface IRandomizer
{
    int Roll(int max); // returns a value from 1 to max, inclusive
}
```

Factory methods live on `DiceParser.Randomizers.Randomizer`:

| Randomizer | Behavior |
| --- | --- |
| `FairDie()` | Uniform `1..N` (default) |
| `FailDie()` | Always `1` |
| `CriticalDie()` | Always `N` |
| `DeckDie()` | Shuffles a deck of faces `1..N` and deals without replacement until a cut card (15–50% of the deck remaining), then reshuffles. Separate decks are tracked per die size. |
| `GamblersAxionDie()` | Drawing a face removes every copy of it from the pool, then one of each face is added back. Recently drawn faces go “cold”; others get hotter. Separate pools are tracked per die size. |

Use `FailDie` and `CriticalDie` in tests when you need a known minimum or maximum total.

You can also implement `IRandomizer` yourself (for example, to replay a fixed sequence of rolls).

## Building and testing

```bash
dotnet test DiceParser.slnx
```

The test project (`DiceParser.Test`) covers syntax validation, keep/drop results, and the deck and gambler’s-axiom randomizers.

## Project layout

```
DiceParser/                 Library
  DiceParser.cs             Parse and evaluate expressions
  Randomizers/              Die implementations
  Tools/                    Tokenizer and roll evaluation
DiceParser.Test/            NUnit tests
DiceParser.slnx             Solution
```

## License

[GNU General Public License v3.0](LICENSE)

namespace DiceParser.Randomizers;
internal class GamblersAxionDie : IRandomizer
{
    private readonly Random _rng = new();
    private readonly Dictionary<int, List<int>> _dicePools = new();


    public int Roll(int max)
    {
        CheckDicePool(max);
        return PullFromPool(max);
    }

    private int PullFromPool(int max)
    {
        var pool = _dicePools[max];
        var randomIndex = _rng.Next(0, pool.Count);
        var rollValue = pool[randomIndex];

        pool.RemoveAll(x => x == rollValue);
        pool.AddRange(GetPoolContents(max));

        return rollValue;
    }

    private void CheckDicePool(int dieSize)
    {
        if (_dicePools[dieSize] == null || _dicePools[dieSize].Count == 0)
        {
            _dicePools[dieSize] = GetPoolContents(dieSize);
        }
    }

    private List<int> GetPoolContents(int dieSize) => Enumerable.Range(1, dieSize).ToList();
}

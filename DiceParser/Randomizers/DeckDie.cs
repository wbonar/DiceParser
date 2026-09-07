namespace DiceParser.Randomizers;

internal class DeckDie : IRandomizer
{
    private readonly Random _rng = new();
    private readonly Dictionary<int, List<int>> _deckLibrary = new();
    private readonly Dictionary<int, double> _deckCutoffs = new();

    private double _cutoffMin = 0.15;
    private double _cutoffMax = 0.33;

    public int Roll(int max)
    {
        var deck = GetDeck(max);
        return DealFromDeck(deck);
    }

    private int DealFromDeck(List<int> deck)
    {
        var card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    private List<int> GetDeck(int dieSize)
    {
        if (ShouldShuffle(dieSize))
        {
            BuildDeck(dieSize);
        }

        return _deckLibrary[dieSize];
    }
    
    private void BuildDeck(int dieSize)
    {
        _deckLibrary[dieSize] = Enumerable.Range(1, dieSize).OrderBy(x => _rng.Next()).ToList();
        _deckCutoffs[dieSize] = _rng.NextDouble() * (_cutoffMax - _cutoffMin) + _cutoffMin;
    }

    private bool ShouldShuffle(int dieSize)
    {
        if (!_deckLibrary.ContainsKey(dieSize) || !_deckCutoffs.ContainsKey(dieSize))
        {
            return true;
        }
        var cutoff = _deckCutoffs[dieSize];
        var currentDeck = _deckLibrary[dieSize];
        var currentDeckSize = currentDeck.Count;
        return currentDeckSize < cutoff * dieSize;
    }

}

namespace DiceParser
{
    public interface IDiceParser
    {
        public int Roll(string s);

        public bool IsValidRoll(string s);
    }
}

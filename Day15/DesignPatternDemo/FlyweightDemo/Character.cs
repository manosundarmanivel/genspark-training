namespace FlyweightDemo
{
    public class Character
    {
        private readonly char _symbol;
        private readonly Font _font;

        public Character(char symbol, Font font)
        {
            _symbol = symbol;
            _font = font;
        }

        public void Display(int x, int y)
        {
            Console.WriteLine($"'{_symbol}' at ({x},{y}) in {_font.Name}");
        }
    }
}
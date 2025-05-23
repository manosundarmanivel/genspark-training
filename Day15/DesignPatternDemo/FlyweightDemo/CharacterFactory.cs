using System.Collections.Generic;

namespace FlyweightDemo
{
    public class CharacterFactory
    {
        private readonly Dictionary<string, Font> _fonts = new();

        public Character GetCharacter(char symbol, string fontName)
        {
            if (!_fonts.ContainsKey(fontName))
            {
                _fonts[fontName] = new Font(fontName);
            }
            return new Character(symbol, _fonts[fontName]);
        }
    }

    public class FlyweightDemoRunner
    {
        public static void Run()
        {
            var factory = new CharacterFactory();

            var c1 = factory.GetCharacter('A', "Arial");
            var c2 = factory.GetCharacter('B', "Arial");
            var c3 = factory.GetCharacter('A', "Arial");

            c1.Display(10, 10);
            c2.Display(20, 10);
            c3.Display(30, 10);
        }
    }
}
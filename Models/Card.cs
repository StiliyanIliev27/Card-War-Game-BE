using WarGame.Models.Enums;

namespace WarGame.Models
{
    public class Card
    {
        public Suit Suit { get; set; }
        public int Number { get; set; }
        public string Name { get; set; } = null!;
    }
}

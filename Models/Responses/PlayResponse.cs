namespace WarGame.Models.Responses
{
    public class PlayResponse
    {
        public int FirstPlayerDeckSize { get; set; }
        public int SecondPlayerDeckSize { get; set; }
        public string FirstPlayerCard { get; set; } = null!;
        public string SecondPlayerCard { get; set; } = null!;  
        public string Message { get; set; } = string.Empty;
        public Player[] Players { get; set; } = [];
    }
}

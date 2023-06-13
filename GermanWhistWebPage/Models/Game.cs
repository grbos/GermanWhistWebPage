namespace GermanWhistWebPage.Models
{
    public class Game
    {
        public int Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        // public GameState GameState { get; set; }

        public IEnumerable<Card> CardStack { get; set; }

        public ICollection<Card> HandPlayer1 { get; set; }
        public ICollection<Card> HandPlayer2 { get; set; }
        public Card NewHandCardPlayer1 { get; set; }
        public Card NewHandCardPlayer2 { get; set; }

        public Card PlayedCardPlayer1 { get; set; }
        public Card PlayedCardPlayer2 { get; set; }


        public Player StartingPlayer { get; set; }
        public Player TrickStartPlayer { get; set; }
        public Player CurrentPlayer { get; set; }

        public Suit TrumpSuit { get; set; }

        public int TargetScore { get; set; }
        public int TotalScore { get; set; }
        public int RoundScore { get; set; }
    }
}

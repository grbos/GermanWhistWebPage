using System.Xml.Linq;

namespace GermanWhistWebPage.Models
{
    public class PlayerViewOfGameStateDTO
    {
        public int Id { get; set; }

        public ICollection<Card> Hand { get; private set; }
        public Card NewHandCard { get; private set; }

        public Card TopCard { get; private set; }

        public Card PlayedCardPlayer1 { get; private set; }
        public Card PlayedCardPlayer2 { get; private set; }


        public Player StartingPlayer { get; private set; }
        public Player TrickStartPlayer { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public Suit TrumpSuit { get; private set; }

        public int TargetScore { get;   private set; }
        public int TotalScore { get; private set; }
        public int RoundScore { get; private set; }

        
        public PlayerViewOfGameStateDTO(Game game , Player player)
        {
            Id = game.Id;
            Hand = player == game.Player1? game.HandPlayer1 : game.HandPlayer2;
            NewHandCard = player == game.Player1 ? game.NewHandCardPlayer1 : game.NewHandCardPlayer2;
            TopCard = game.CardStack.First();

            PlayedCardPlayer1 = game.PlayedCardPlayer1;
            PlayedCardPlayer2 = game.PlayedCardPlayer2;
            StartingPlayer = game.StartingPlayer;
            TrickStartPlayer = game.TrickStartPlayer;
            CurrentPlayer = game.CurrentPlayer;
            TrumpSuit = game.TrumpSuit;
            TargetScore = game.TargetScore;
            TotalScore = game.TotalScore;
            RoundScore = game.RoundScore;

        }
    }
}

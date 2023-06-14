using System.Xml.Linq;

namespace GermanWhistWebPage.Models
{
    public class PlayerViewOfGameStateDTO
    {
        public int Id { get; set; }

        public ICollection<int> Hand { get; private set; }
        public int? NewHandCardId { get; private set; }

        public int TopCardId { get; private set; }

        public int? PlayedCardIdPlayer1 { get; private set; }
        public int? PlayedCardIdPlayer2 { get; private set; }


        public int StartingPlayerId { get; private set; }
        public int TrickStartPlayerId { get; private set; }
        public int CurrentPlayerId { get; private set; }

        public Suit TrumpSuit { get; private set; }

        public int TargetScore { get;   private set; }
        public int TotalScorePlayer1 { get; private set; }
        public int TotalScorePlayer2 { get; private set; }
        public int RoundScorePlayer1 { get; private set; }
        public int RoundScorePlayer2 { get; private set; }
        
        public ICollection<int> ValidMoves { get; private set; }


        public PlayerViewOfGameStateDTO(Game game , int playerId, ICollection<int> validMoves)
        {
            Id = game.Id;
            Hand = playerId == game.Player1Id ? game.HandPlayer1 : game.HandPlayer2;
            NewHandCardId = playerId == game.Player1Id ? game.NewHandCardIdPlayer1 : game.NewHandCardIdPlayer2;
            TopCardId = game.CardStack.First();

            PlayedCardIdPlayer1 = game.PlayedCardIdPlayer1;
            PlayedCardIdPlayer2 = game.PlayedCardIdPlayer2;
            StartingPlayerId = game.StartingPlayerId;
            TrickStartPlayerId = game.TrickStartPlayerId;
            CurrentPlayerId = game.CurrentPlayerId;
            TrumpSuit = game.TrumpSuit;
            TargetScore = game.TargetScore;
            TotalScorePlayer1 = game.TotalScorePlayer1;
            TotalScorePlayer2 = game.TotalScorePlayer2;
            RoundScorePlayer1 = game.RoundScorePlayer1;
            RoundScorePlayer2 = game.RoundScorePlayer2;
            ValidMoves = validMoves;
        }
    }
}

using System.Xml.Linq;

namespace GermanWhistWebPage.Models
{
    public class PlayerViewOfGameStateDTO
    {
        public int Id { get; set; }

        public ICollection<int> Hand { get; private set; }
        public int? NewHandCardId { get; private set; }

        public int NumberOfHandCardsOpponent { get; private set; }

        public int TopCardId { get; private set; }

        public int? PlayedCardIdPlayer { get; private set; }
        public int? PlayedCardIdOpponent { get; private set; }


        public int StartingPlayerId { get; private set; }
        public int TrickStartPlayerId { get; private set; }
        public int CurrentPlayerId { get; private set; }

        public Suit TrumpSuit { get; private set; }
        public string TrumpSuitName { get; private set; }

        public int TargetScore { get;   private set; }
        public int TotalScorePlayer { get; private set; }
        public int TotalScoreOpponent { get; private set; }
        public int RoundScorePlayer { get; private set; }
        public int RoundScoreOpponent { get; private set; }
        
        public ICollection<int> ValidMoves { get; private set; }


        public PlayerViewOfGameStateDTO(Game game , int playerId, ICollection<int> validMoves)
        {
            Id = game.Id;
            Hand = playerId == game.Player1Id ? game.HandPlayer1 : game.HandPlayer2;
            NewHandCardId = playerId == game.Player1Id ? game.NewHandCardIdPlayer1 : game.NewHandCardIdPlayer2;
            TopCardId = game.CardStack.First();

            PlayedCardIdPlayer = playerId == game.Player1Id ? game.PlayedCardIdPlayer1 : game.PlayedCardIdPlayer2;
            PlayedCardIdOpponent = playerId == game.Player1Id ? game.PlayedCardIdPlayer2 : game.PlayedCardIdPlayer1;
            StartingPlayerId = game.StartingPlayerId;
            TrickStartPlayerId = game.TrickStartPlayerId;
            CurrentPlayerId = game.CurrentPlayerId;
            TrumpSuit = game.TrumpSuit;
            TrumpSuitName = game.TrumpSuit.ToString();
            TargetScore = game.TargetScore;
            TotalScorePlayer = playerId == game.Player1Id ? game.TotalScorePlayer1 : game.TotalScorePlayer2;
            TotalScoreOpponent = playerId == game.Player1Id ? game.TotalScorePlayer2 : game.TotalScorePlayer1;
            RoundScorePlayer = playerId == game.Player1Id ? game.RoundScorePlayer1 : game.TotalScorePlayer2;
            RoundScoreOpponent = playerId == game.Player1Id ? game.RoundScorePlayer2 : game.TotalScorePlayer2;
            ValidMoves = validMoves;
            NumberOfHandCardsOpponent = playerId == game.Player1Id ? game.HandPlayer2.Count() : game.HandPlayer1.Count();
        }
    }
}

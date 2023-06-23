using System.Numerics;
using System.Xml.Linq;

namespace GermanWhistWebPage.Models
{
    public class PlayerViewOfGameStateDTO
    {
        public int Id { get; set; }

        public ICollection<int> Hand { get; private set; }
        public int? NewHandCardId { get; private set; }

        public int NumberOfHandCardsOpponent { get; private set; }

        public int? TopCardId { get; private set; }

        public int? PlayedCardIdPlayer { get; private set; }
        public int? PlayedCardIdOpponent { get; private set; }

        public int? PreviousPlayedCardIdPlayer { get; private set; }
        public int? PreviousPlayedCardIdOpponent { get; private set; }


        public bool IsPlayerStartingPlayer { get; private set; }
        public bool IsPlayerTrickStartPlayer { get; private set; }
        public bool IsPlayerCurrentPlayer { get; private set; }

        public bool IsPlayerPreviousTrickWinner { get; private set; }

        public Suit TrumpSuit { get; private set; }
        public string TrumpSuitName { get; private set; }

        public int TargetScore { get;   private set; }
        public int TotalScorePlayer { get; private set; }
        public int TotalScoreOpponent { get; private set; }
        public int RoundScorePlayer { get; private set; }
        public int RoundScoreOpponent { get; private set; }
        
        public ICollection<int> ValidMoves { get; private set; }

        public bool IsTrickOngoing { get
            {
                return PlayedCardIdPlayer != null || PlayedCardIdOpponent != null;
            } 
        }

        


        public PlayerViewOfGameStateDTO(Game game , int playerId, ICollection<int> validMoves)
        {
            if (game.Player1Id != playerId && game.Player2Id != playerId)
                throw new ArgumentException("Player not playing this game", nameof(playerId));

            Id = game.Id;
            Hand = playerId == game.Player1Id ? game.HandPlayer1 : game.HandPlayer2;
            NewHandCardId = playerId == game.Player1Id ? game.NewHandCardIdPlayer1 : game.NewHandCardIdPlayer2;
            TopCardId = game.CardStack.Count() > 0 ? game.CardStack.First() : null;

            PlayedCardIdPlayer = playerId == game.Player1Id ? game.PlayedCardIdPlayer1 : game.PlayedCardIdPlayer2;
            PlayedCardIdOpponent = playerId == game.Player1Id ? game.PlayedCardIdPlayer2 : game.PlayedCardIdPlayer1;
            PreviousPlayedCardIdPlayer = playerId == game.Player1Id ? game.PreviousPlayedCardIdPlayer1 : game.PreviousPlayedCardIdPlayer2;
            PreviousPlayedCardIdOpponent = playerId == game.Player1Id ? game.PreviousPlayedCardIdPlayer2 : game.PreviousPlayedCardIdPlayer1;

            IsPlayerStartingPlayer = game.StartingPlayerId == playerId;
            IsPlayerTrickStartPlayer = game.TrickStartPlayerId == playerId;
            IsPlayerCurrentPlayer = game.CurrentPlayerId == playerId;

            IsPlayerPreviousTrickWinner = game.TrickWiningPlayerPreviousRound == playerId;

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

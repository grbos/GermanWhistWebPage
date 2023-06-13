using GermanWhistWebPage.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace GermanWhistWebPage.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }

        // public GameState GameState { get; set; }

        public ICollection<int> CardStack { get; set; }

        public ICollection<int> HandPlayer1 { get; set; }

        public ICollection<int> HandPlayer2 { get; set; }
        public int? NewHandCardIdPlayer1 { get; set; }
        public int? NewHandCardIdPlayer2 { get; set; }

        public int? PlayedCardIdPlayer1 { get; set; }
        public int? PlayedCardIdPlayer2 { get; set; }


        public int StartingPlayerId { get; set; }
        public int TrickStartPlayerId { get; set; }
        public int CurrentPlayerId { get; set; }

        public Suit TrumpSuit { get; set; }

        public int TargetScore { get; set; }
        public int TotalScorePlayer1 { get; set; }
        public int TotalScorePlayer2 { get; set; }
        public int RoundScorePlayer1 { get; set; }
        public int RoundScorePlayer2 { get; set; }

        public ICollection<int>? HandCurrentPlayer { get
            {
                if (CurrentPlayerId == Player1Id)
                    return HandPlayer1;
                else
                    return HandPlayer2;
            } 
        }
        public bool isEndOfTrick { get
            {
                if (PlayedCardIdPlayer1 == null || PlayedCardIdPlayer2 == null)
                    return false;
                return true;
            } 
        }
    }
}

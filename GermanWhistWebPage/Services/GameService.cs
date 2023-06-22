using GermanWhistWebPage.Models;
using System.Linq;

namespace GermanWhistWebPage.Services
{
    public class GameService
    {
        private static readonly int _nCardsPerPlayer = 13;
        private static readonly int _targetScore = 20;


        private readonly CardService _cardService;

        public GameService(CardService cardService)
        {
            _cardService = cardService;
        }
        public void deleteGame(Game game) { }

        public Game createGame(int player1Id, int? player2Id)
        {

            List<int> shuffledCards = _cardService.GetShuffeledCards();
            var cardStack = shuffledCards.Skip(2 * _nCardsPerPlayer).ToList();

            return new Game()
            {
                Player1Id = player1Id,
                Player2Id = player2Id,
                CardStack = cardStack,
                HandPlayer1 = shuffledCards.Take(_nCardsPerPlayer).ToList(),
                HandPlayer2 = shuffledCards.Skip(_nCardsPerPlayer).Take(_nCardsPerPlayer).ToList(),

                StartingPlayerId = player1Id,
                TrickStartPlayerId = player1Id,
                CurrentPlayerId = player1Id,

                TrumpSuit = _cardService.getCardFromId(cardStack.First()).Suit,

                TargetScore = _targetScore,
                TotalScorePlayer1 = 0,
                TotalScorePlayer2 = 0,
                RoundScorePlayer1 = 0,
                RoundScorePlayer2 = 0
            };
        }

        public bool isValidMove(Game game, int playerId, int cardId)
        {
            if (game == null) return false;

            if (game.CurrentPlayerId != playerId)
                return false;

            ICollection<int> validMoves = getValidMoves(game, playerId);
            return validMoves.Contains(cardId);
        }

        private Suit? getLeadCardSuit(Game game)
        {
            int? leadCardId = (game.TrickStartPlayerId == game.Player1Id) ? game.PlayedCardIdPlayer1 : game.PlayedCardIdPlayer2;
            if (leadCardId == null)
                return null;
            Card card = _cardService.getCardFromId(leadCardId.Value);
            return card.Suit;
        }


        public ICollection<int> getValidMoves(Game game, int PlayerId)
        {
            if (PlayerId != game.CurrentPlayerId)
            {
                return new List<int>();
            }
            Suit? leadCardSuit = getLeadCardSuit(game);
            if (leadCardSuit == null || !game.HandCurrentPlayer.Any(cardId => _cardService.getCardFromId(cardId).Suit == leadCardSuit))
                return game.HandCurrentPlayer;

            return game.HandCurrentPlayer.Where(cardId => (_cardService.getCardFromId(cardId).Suit == leadCardSuit ||
                _cardService.getCardFromId(cardId).Suit == game.TrumpSuit)).ToList();
        }

        public void makeMove(Game game, int playerId, int cardId)
        {
            if (!(game.Player1Id == playerId) && !(game.Player2Id == playerId))
            {
                throw new ArgumentException("Player not playing this game", nameof(playerId));
            }
            if (!(game.CurrentPlayerId == playerId))
            {
                throw new ArgumentException("Currently not this players turn", nameof(playerId));
            }
            if (!isValidMove(game, playerId, cardId))
            {
                throw new ArgumentException("This is not a valid card", nameof(cardId));
            }


            if (playerId == game.Player1Id)
            {
                game.PlayedCardIdPlayer1 = cardId;
                game.HandPlayer1.Remove(cardId);
            }
            else
            {
                game.PlayedCardIdPlayer2 = cardId;
                game.HandPlayer2.Remove(cardId);
            }
            ProcessGameUntilNextInput(game);
        }

        private void ProcessGameUntilNextInput(Game game)
        {
            int nextPlayerId;
            if (!game.IsEndOfTrick)
            {
                nextPlayerId = game.CurrentPlayerId == game.Player1Id ? game.Player2Id.GetValueOrDefault() : game.Player1Id;
                game.CurrentPlayerId = nextPlayerId;
                return;

            }

            int winningPlayerId = get_trick_winner(game);
            evaluateTrick(game, winningPlayerId);
            game.PreviousPlayedCardIdPlayer1 = game.PlayedCardIdPlayer1;
            game.PreviousPlayedCardIdPlayer2 = game.PlayedCardIdPlayer2;
            game.PlayedCardIdPlayer1 = null;
            game.PlayedCardIdPlayer2 = null;
            nextPlayerId = winningPlayerId;
            game.TrickStartPlayerId = nextPlayerId;
            game.TrickWiningPlayerPreviousRound = winningPlayerId;


            if (game.IsEndOfRound)
            {
                List<int> shuffledCards = _cardService.GetShuffeledCards();
                var cardStack = shuffledCards.Skip(2 * _nCardsPerPlayer).ToList();

                int nextStartPlayerId = game.StartingPlayerId == game.Player1Id ? game.Player2Id.GetValueOrDefault() : game.Player1Id;
                game.CardStack = cardStack;
                game.HandPlayer1 = shuffledCards.Take(_nCardsPerPlayer).ToList();
                game.HandPlayer2 = shuffledCards.Skip(_nCardsPerPlayer).Take(_nCardsPerPlayer).ToList();

                game.StartingPlayerId = nextStartPlayerId;
                game.TrickStartPlayerId = nextStartPlayerId;
                game.CurrentPlayerId = nextStartPlayerId;

                game.TrumpSuit = _cardService.getCardFromId(cardStack.First()).Suit;
            }
            else
            {
                game.CurrentPlayerId = nextPlayerId;
            }

        }

        private void evaluateTrick(Game game, int winningPlayerId)
        {
            if (game.CardStack.Count() > 0)
            {
                if (winningPlayerId == game.Player1Id)
                {
                    game.HandPlayer1.Add(game.CardStack.First());
                    game.NewHandCardIdPlayer1 = game.CardStack.First();
                    game.CardStack.Remove(game.CardStack.First());

                    game.HandPlayer2.Add(game.CardStack.First());
                    game.NewHandCardIdPlayer2 = game.CardStack.First();
                    game.CardStack.Remove(game.CardStack.First());
                }
                else
                {
                    game.HandPlayer2.Add(game.CardStack.First());
                    game.NewHandCardIdPlayer2 = game.CardStack.First();
                    game.CardStack.Remove(game.CardStack.First());

                    game.HandPlayer1.Add(game.CardStack.First());
                    game.NewHandCardIdPlayer1 = game.CardStack.First();
                    game.CardStack.Remove(game.CardStack.First());
                }
            }
            else
            {
                game.NewHandCardIdPlayer1 = null;
                game.NewHandCardIdPlayer2 = null;
                if (winningPlayerId == game.Player1Id)
                {
                    game.TotalScorePlayer1++;
                    game.RoundScorePlayer1++;
                }
                else
                {
                    game.TotalScorePlayer2++;
                    game.RoundScorePlayer2++;
                }
            }


        }

        private int get_trick_winner(Game game)
        {
            Card player1Card = _cardService.getCardFromId(game.PlayedCardIdPlayer1.Value);
            Card player2Card = _cardService.getCardFromId(game.PlayedCardIdPlayer2.Value);
            Suit? leadCardSuit = getLeadCardSuit(game);
            if (player1Card.Suit == game.TrumpSuit && player2Card.Suit != game.TrumpSuit)
            {
                return game.Player1Id;
            }
            if (player1Card.Suit != game.TrumpSuit && player2Card.Suit == game.TrumpSuit)
            {
                return game.Player2Id.GetValueOrDefault();
            }
            if (player1Card.Suit == game.TrumpSuit && player2Card.Suit == game.TrumpSuit)
            {
                if (player1Card.Number > player2Card.Number)
                    return game.Player1Id;
                else
                    return game.Player2Id.GetValueOrDefault();
            }


            if (player1Card.Suit == leadCardSuit && player2Card.Suit != leadCardSuit)
            {
                return game.Player1Id;
            }
            if (player1Card.Suit != leadCardSuit && player2Card.Suit == leadCardSuit)
            {
                return game.Player2Id.GetValueOrDefault();
            }

            if (player1Card.Number > player2Card.Number)
                return game.Player1Id;
            else
                return game.Player2Id.GetValueOrDefault();

        }


    }
}

using GermanWhistWebPage.Models;

namespace GermanWhistWebPage.Services
{
    public class BotService
    {
        private const int minBotDificulty = 0;
        private const int maxBotDificulty = 0;
        private readonly GameContext _gameContext;
        private readonly GameService _gameService;
        private readonly Random _random;

        //public Player GetUserPlayer(GameContext gameContext)
        //{
        //    // TODO
        //    int testPlayerId = 1;
        //    var p = gameContext.Players.Find(testPlayerId);
        //    return p;
        //}
        public BotService(GameContext gameContext, GameService gameService)
        {
            _gameContext = gameContext;
            _gameService = gameService;
            _random = new Random();
        }

        public BotPlayer getBotPlayer(int botDifficulty)
        {
            if (botDifficulty < minBotDificulty || botDifficulty > maxBotDificulty)
                return null;

            BotPlayer? player = _gameContext.BotPlayers.FirstOrDefault(p => p.Difficulty == botDifficulty);
            if (player == null)
            {
                player = new BotPlayer()
                {
                    Difficulty = botDifficulty,
                    BotName = $"Bot with Difficulty {botDifficulty}"
                };
                _gameContext.BotPlayers.Add(player);
                _gameContext.SaveChanges();
            }
            return player;
        }

        public void MakeBotMove(Game game)
        {
            if (!game.IsBotGame)
                throw new ArgumentException("Game is not a bot game");

            int botPlayerId = game.Player2Id.Value;


            var validMoves = _gameService.getValidMoves(game, botPlayerId).ToList();

            if (validMoves.Count() == 0)
                throw new Exception("Bot has no valid moves");

            int move = validMoves[new Random().Next(validMoves.Count())];
            _gameService.makeMove(game, botPlayerId, move);
        }
    }
}

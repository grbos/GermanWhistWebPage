using GermanWhistWebPage.Models;

namespace GermanWhistWebPage.Services
{
    public class PlayerService
    {
        private const int minBotDificulty = 0;
        private const int maxBotDificulty = 10;
        private readonly GameContext _gameContext;

        //public Player GetUserPlayer(GameContext gameContext)
        //{
        //    // TODO
        //    int testPlayerId = 1;
        //    var p = gameContext.Players.Find(testPlayerId);
        //    return p;
        //}
        public PlayerService(GameContext gameContext)
        {
            _gameContext = gameContext;
        }

        public BotPlayer getBotPlayer(int botDifficulty)
        {
            if (botDifficulty < minBotDificulty || botDifficulty > maxBotDificulty) 
                return null;

            BotPlayer player = _gameContext.BotPlayers.FirstOrDefault(p => p.Difficulty == botDifficulty);
            if (player == null)
            {
                player = new BotPlayer()
                {
                    Difficulty = botDifficulty,
                    BotName = $"Bot with Difficulty {botDifficulty}"
                };
            }
            return player;
        }
    }
}

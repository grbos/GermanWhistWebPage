using GermanWhistWebPage.Models;

namespace GermanWhistWebPage.Services
{
    public class PlayerService
    {
        public Player GetUserPlayer(GameContext gameContext)
        {
            // TODO
            int testPlayerId = 1;
            var p = gameContext.Players.Find(testPlayerId);
            return p;
        }
    }
}

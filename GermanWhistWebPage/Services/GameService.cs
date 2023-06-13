using GermanWhistWebPage.Models;

namespace GermanWhistWebPage.Services
{
    public class GameService
    {
        public GameService() { }
        public void deleteGame(Game game) { }

        public Game createGame(int Player1Id, int Player2Id) 
        {
            return new Game();
        }

        public bool isValidMove(Game game, Player player, int cardId) 
        {
            return true;
        }

        public void makeMove(Game game, Player player, int cardId) { }
    }
}

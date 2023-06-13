namespace GermanWhistWebPage.Models
{
    public class GameInfoDTO
    {
        public int Id { get; private set; }
        public int Player1Id { get; private set; }
        public int Player2Id { get; private set; }
        public bool HasGameStarted {
            get {
                if (Player1Id != null && Player2Id != null)
                {
                    return true;
                }
                return false;
            }
        }
        public GameInfoDTO(Game game)
        {
            Id = game.Id;
            Player1Id = game.Player1Id;
            Player2Id = game.Player2Id;
        }
    }
}

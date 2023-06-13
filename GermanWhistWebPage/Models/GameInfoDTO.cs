namespace GermanWhistWebPage.Models
{
    public class GameInfoDTO
    {
        public int Id { get; private set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public bool HasGameStarted {
            get {
                if (Player1 != null && Player2 != null)
                {
                    return true;
                }
                return false;
            }
        }
        public GameInfoDTO(Game game)
        {
            Id = game.Id;
            Player1 = game.Player1;
            Player2 = game.Player2;
        }
    }
}

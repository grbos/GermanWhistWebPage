namespace GermanWhistWebPage.Models
{
    public class GameInfoDTO
    {
        public int Id { get; private set; }
        public int? UserPlayerId { get; private set; }
        public int? OpponentPlayerId { get; private set; }
        public bool HasGameStarted { get; private set; }
        public GameInfoDTO(Game game, int playerId)
        {
            Id = game.Id;
            UserPlayerId = playerId == game.Player1Id ? game.Player1Id : game.Player2Id;
            OpponentPlayerId = playerId != game.Player1Id ? game.Player1Id : game.Player2Id;
            HasGameStarted = game.HasGameStarted;

        }
    }
}

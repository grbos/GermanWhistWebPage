namespace GermanWhistWebPage.Models
{
    public class NewGameInfoDTO
    {
        public int? OpponentPlayerId { get; set; }
        public bool AgainstBotOpponent { get; set; }
        public int? BotDifficulty { get; set; }
    }
}

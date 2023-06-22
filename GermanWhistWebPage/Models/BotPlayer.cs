namespace GermanWhistWebPage.Models
{
    public class BotPlayer : Player
    {
        public int Difficulty { get; set; }
        public string BotName { get; set; }
        public override string Name => BotName;
    }
}

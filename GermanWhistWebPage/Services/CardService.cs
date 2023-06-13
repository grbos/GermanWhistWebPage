using GermanWhistWebPage.Models;

namespace GermanWhistWebPage.Services
{
    public class CardService
    {
        public static bool IsWinningAgainst(Card card, Card otherCard, Suit trumpSuit, Suit leadSuit)
        {
            if (card.Suit == trumpSuit && otherCard.Suit != trumpSuit)
            {
                return true;
            }
            if (card.Suit != trumpSuit && otherCard.Suit == trumpSuit)
            {
                return false;
            }


            if (card.Suit == trumpSuit && otherCard.Suit == trumpSuit)
            {
                return card.Number > otherCard.Number;
            }

            if (card.Suit == leadSuit && otherCard.Suit != leadSuit)
            {
                return true;
            }
            if (card.Suit != leadSuit && otherCard.Suit == leadSuit)
            {
                return false;
            }
            if (card.Suit == leadSuit && otherCard.Suit == leadSuit)
            {
                return card.Number > otherCard.Number;
            }
            return false;
        }
    }
}

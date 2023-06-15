using GermanWhistWebPage.Extensions;
using GermanWhistWebPage.Models;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace GermanWhistWebPage.Services
{

    public class CardService
    {
        private List<Card> _cards;
        public CardService()
        {
            _cards = new List<Card>();
            int j = 0;
            for (int i = 2; i < 15; i++) {
                foreach (Suit suit in Enum.GetValues(typeof(Suit))) {
                _cards.Add(new Card(j, suit, i));
                j += 1;
            } }
        }

        public List<int> GetShuffeledCards()
        {
            var newList = _cards.Select(card => card.Id).ToList();
            newList.Shuffle();
            return newList;
        }

        public Card getCardFromId(int cardId)
        {
            return _cards[cardId];
        }
    }
}

namespace GermanWhistWebPage.Models
{
    public class Card
    {
        public Card(int id, Suit suit, int number)
        {
            Id = id;
            Suit = suit;
            Number = number;
        }

        public int Id { get; private set; }
        public Suit Suit { get; private set; }
        public int Number { get; private set; }
        public string Name { get 
            {
                if (Number == 11)
                    return "jack";
                if (Number == 12)
                    return "queen";
                if (Number == 13)
                    return "king";
                if (Number == 14)
                    return "ace";
                return Number.ToString();
            } 
        }

        public override bool Equals(object? obj)
        {
            var card = obj as Card;
            if (card == null) 
                return false;

            return Id == card.Id;
        }


    }
}

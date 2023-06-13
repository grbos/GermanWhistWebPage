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

        public int Id { get; set; }
        public Suit Suit { get; set; }
        public int Number { get; set; }
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
    }
}

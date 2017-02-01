using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class Deck
    {
        public List<Card> Cards {  get; set; }
        public List<Card> ThrownCards { get; set; }
        public CardSuit TrumpSuit { get; set; }

        public bool HasOpenedCard { get; set; }

        public Deck()
        {
            Cards = new List<Card>(Constants.DECK_COUNT);
            ThrownCards = new List<Card>(Constants.DECK_COUNT);
        }

        public override string ToString()
        {
            String output = "";
            foreach (var card in Cards)
            {
                output += card.ToString();
                output += Environment.NewLine;
            }

            return output;
        }
    }
}

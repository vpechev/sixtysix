using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class Card {

        public string CardImageSrc
        {
            get
            {
                return @"..\CardImageFiles\PNG-cards-1.3\" + this.ToString() + ".png".ToLower();
            }
        }
        public string _cardBackImageSrc { get; set; }
        public CardSuit Suit { get; set; }
        public CardValue Value { get; set; }


        public string CardBackImageSrc
        {
            get { return @"..\CardImageFiles\PNG-cards-1.3\red_joker.png"; }
            set { _cardBackImageSrc = value; }
        }

        public override string ToString(){            
            return String.Format("{0}_of_{1}s", Value.ToString(), Suit.ToString());
        }

        public Card()
        {

        }

        public Card(CardValue value, CardSuit suit)
        {
            this.Suit = suit;
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            var otherCard = obj as Card;

            if (otherCard == null)
            {
                return false;
            }

            return this.Suit.Equals(otherCard.Suit) && this.Value.Equals(otherCard.Value);
        }

        public override int GetHashCode()
        {
            var primeNumber = 31;
            return primeNumber * this.Suit.GetHashCode() * this.Value.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class Card {
    
        public CardSuit Suit { get; set; }
        public CardValue Value { get; set; }

        public override string ToString(){            
            return String.Format("{0} {1} ", Value.ToString(), Suit.ToString());
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

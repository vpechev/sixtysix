using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class Player
    {
        public Player(bool isAIPlayer)
        {
            Cards = new List<Card>();
            ThrownCards = new List<Card>();
            IsAIPlayer = isAIPlayer;
        }

        public List<Card> Cards { get; set; }
        public int Score { get; set; }
        public int WinsCount { get; set; }
        public bool HasWonLastDeal { get; set; }
        public bool HasWonLastHand { get; set; }
        public bool IsAIPlayer { get; set; }
        public List<Card> ThrownCards { get; set; }

        public Card GiveCard(Card card)
        {
            this.Cards.Remove(card);
            this.ThrownCards.Add(card);
            return card;
        }

        public string ToStringPlayerCards()
        {
            string output = Environment.NewLine;
            
            foreach (var card in Cards)
            {
                output += "\t";
                output += card;
                output += Environment.NewLine;
            }

            return output;
        }

        public override string ToString()
        {
            string outputStr = IsAIPlayer ? "The machine" : "You";
            return String.Format("{0}", outputStr);
        }
    }
}

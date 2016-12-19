using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class AIMovementUtil
    {
        public static Card MakeTurn(Player player, Deck deck, Card playedFromOther=null)
        {
            var rand = new Random();
            //TODO This line should be replaced with smarter move (choosing card)
            var card = player.Cards[rand.Next(0, player.Cards.Count)];
            Console.WriteLine("AI Hand: " + player.ToStringPlayerCards());
            Console.WriteLine("AI has played: {0}", card);
            player.GiveCard(card);

            return card;
            //return new Card() { Value = 0, Suit = 0 };
        }

        public static int GetDeckSplittingIndex()
        {
            Random rand = new Random(System.DateTime.Now.Millisecond);
            var splitIndex = rand.Next(0, Constants.DECK_COUNT);

            return splitIndex;
        }
    }
}

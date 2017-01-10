using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class InputPlayerMovementUtil
    {
        public static Card MakeTurn(Player player, Deck deck, Card playedFromOther=null)
        {
            Console.WriteLine();
            if (playedFromOther != null)
            {
                Console.WriteLine("Other player played: " + playedFromOther);
            }
            Console.WriteLine("Your Hand: " + player.ToStringPlayerCards());
            Card card = null;
            do
            {
                card = ParseInputCard();
            } while (card == null || card.Suit == 0);

            if ((deck.Cards.Count == 0 || deck.IsClosed) && playedFromOther != null && !card.Suit.Equals(playedFromOther.Suit) && SixtySixUtil.HasAnsweringCard(player, playedFromOther))
            {
                while(!card.Suit.Equals(playedFromOther.Suit)){
                    Console.WriteLine("You HAVE to answer.");
                    card = ParseInputCard();
                }
            }

            player.GiveCard(card);

            return card;
        }

        private static Card ParseInputCard()
        {
            Console.WriteLine("Ender the choosen card in format <<<cardValue cardSuit>>>");
            String input = Console.ReadLine();
            var parts = input.Split(null);
            var value = ParseInputToCardValue(parts[0]);
            var suit = ParseInputToCardSuit(parts[1]);

            return new Card() { Value = value, Suit = suit };
        }

        private static CardValue ParseInputToCardValue(string input)
        {
            switch (input)
            {
                case "9": return CardValue.NINE; 
                case "nine" : return CardValue.NINE;
                case "10" : return CardValue.TEN;
                case "ten" : return CardValue.TEN;
                case "jack": return CardValue.JACK;
                case "queen": return CardValue.QUEEN;
                case "king": return CardValue.KING;
                case "ace": return CardValue.ACE;
            }
            return 0;
        }

        private static CardSuit ParseInputToCardSuit(string input)
        {
            //return (CardSuit)Int32.Parse(input);
            switch (input)
            {
                case "c": return CardSuit.CLUB;
                case "d": return CardSuit.DIAMOND;
                case "h": return CardSuit.HEART;
                case "s": return CardSuit.SPADE;
            }
            return 0;
        }

        //TODO This should be changed to ask the user for split index
        public static int GetDeckSplittingIndex()
        {
            Random rand = new Random(System.DateTime.Now.Millisecond);
            var splitIndex = rand.Next(0, Constants.DECK_COUNT);

            return splitIndex;
        }
    }
}

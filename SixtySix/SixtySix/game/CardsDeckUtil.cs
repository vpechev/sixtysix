using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class CardsDeckUtil
    {
        public static Deck InitializeDeck() {
            var deck = new Deck();

            var suits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>();
            var cardValues = Enum.GetValues(typeof(CardValue)).Cast<CardValue>();

            foreach (var suit in suits)
            {
                foreach (var value in cardValues)
                {
                    deck.Cards.Add(new Card() { Suit = suit, Value = value });
                }
            }
            
            return deck;
        }

        public static void shuffleDeck(Deck deck)
        {
            Random rand = new Random(System.DateTime.Now.Millisecond);
            for (int i = deck.Cards.Count - 1; i > 0; --i)
            {
                int k = rand.Next(i + 1);
                var temp = deck.Cards[i];
                deck.Cards[i] = deck.Cards[k];
                deck.Cards[k] = temp;
            }
        }

        public static void splitDeck(Deck deck, int index)
        {
            int cardsListCount = deck.Cards.Count;

            for (int i = 0; i < index; i++)
            {
                var currentCard = deck.Cards[i];
                deck.Cards.RemoveAt(i);
                deck.Cards.Add(currentCard);
            }

            //TODO TO BE DELETED
            if (deck.Cards.Count != Constants.DECK_COUNT)
            {
                Console.WriteLine("FAIL SPLITTING. DECK CARDS COUNT IS: {0}", deck.Cards.Count);
            } 
        }


        public static void CollectCardsInDeck(Deck deck, Player player1, Player player2)
        {
            if (deck.ThrownCards != null)
            {
                deck.Cards.AddRange(deck.ThrownCards);
            }

            //if (player1.ThrownCards != null)
            //{
            //    deck.Cards.AddRange(player1.ThrownCards);
            //}
            
            if (player1.Cards != null)
            {
                deck.Cards.AddRange(player1.Cards);
            }

            //if (player2.ThrownCards != null)
            //{
            //    deck.Cards.AddRange(player2.ThrownCards);
            //}

            if (player2.Cards != null)
            {
                deck.Cards.AddRange(player2.Cards);
            }

            if (deck.Cards.Count() != Constants.DECK_COUNT)
            {
                throw new Exception("The number of cards in deck should be exactly 24.\nThrown by method CollectCardsInDeck.");
            }
        }
    }
}

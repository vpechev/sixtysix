using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class SixtySixUtil
    {
        public static bool IsSixtySixReached(Player player){
            return player.Score >= Constants.TOTAL_SCORE;
        }

        public static bool HasCardsInDeck(Deck deck)
        {
            return deck.Cards.Count > 0;
        }

        public static bool CanSwap(List<Card> handCards, Deck deck)
        {
            // last card is always deck.Cards[deck.Cards.Count() - 1]
            var lastCard = deck.Cards[deck.Cards.Count() - 1];
            var hasANine = handCards.First(x=>x.Value == CardValue.NINE) != null;
            if (deck.Cards.Count() > 2 && hasANine)
                return true;

            return false;
        }

        public static bool HasToAnswerWithMatching(Deck deck, bool isClosed)
        {
            return deck.Cards.Count == 0 || isClosed;
        }

        public static bool HasTwenty(List<Card> handCards)
        {
            for (int i = 0; i < handCards.Count; i++)
            {
                var firstCard = handCards[i];
                if (firstCard.Value == CardValue.King)
                {
                    for (int j = i+1; j < handCards.Count; j++)
                    {
                        var secondCard = handCards[j];
                        if (secondCard.Value == CardValue.Queen && firstCard.Suit == secondCard.Suit)
                        {
                            return true;
                        }
                    }
                }

                if (firstCard.Value == CardValue.Queen)
                {
                    for (int j = i + 1; j < handCards.Count; j++)
                    {
                        var secondCard = handCards[j];
                        if (secondCard.Value == CardValue.King && firstCard.Suit == secondCard.Suit)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool HasForty(List<Card> handCards, CardSuit trumpSuit)
        {
            for (int i = 0; i < handCards.Count; i++)
            {
                var firstCard = handCards[i];
                if (firstCard.Value == CardValue.King && firstCard.Suit == trumpSuit)
                {
                    for (int j = i + 1; j < handCards.Count; j++)
                    {
                        var secondCard = handCards[j];
                        if (secondCard.Value == CardValue.Queen && firstCard.Suit == secondCard.Suit)
                        {
                            return true;
                        }
                    }
                }

                if (firstCard.Value == CardValue.Queen && firstCard.Suit == trumpSuit)
                {
                    for (int j = i + 1; j < handCards.Count; j++)
                    {
                        var secondCard = handCards[j];
                        if (secondCard.Value == CardValue.King && firstCard.Suit == secondCard.Suit)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static int GetNumberOfWins(Player other)
        {
            if (other.Score == 0)
                return 3;
            else if (other.Score < 33)
                return 2;
            else
                return 1;
        }

        public static void DealCards(Deck deck, Player player1, Player player2)
        {
            GiveAThree(deck, player1);
            GiveAThree(deck, player2);

            GiveAThree(deck, player1);
            GiveAThree(deck, player2);

            //we always store the opened card at last position but is always the next comming card
            var firstCard = deck.Cards.First();
            deck.Cards.Add(firstCard);
            deck.Cards.Remove(firstCard);
        }

        private static void GiveAThree(Deck deck, Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                var card = deck.Cards.First();
                player.Cards.Add(card);
                deck.Cards.Remove(card);
            }
        }
    }
}

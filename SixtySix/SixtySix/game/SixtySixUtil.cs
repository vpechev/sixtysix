using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class SixtySixUtil
    {
        //Announcement methods
        public static bool HasTwenty(List<Card> handCards, Deck deck)
        {
            if (IsFirstHand(deck))
            {
                return false;
            }

            for (int i = 0; i < handCards.Count; i++)
            {
                var firstCard = handCards[i];
                if (firstCard.Value == CardValue.KING)
                {
                    for (int j = i+1; j < handCards.Count; j++)
                    {
                        var secondCard = handCards[j];
                        if (secondCard.Value == CardValue.QUEEN && firstCard.Suit == secondCard.Suit)
                        {
                            return true;
                        }
                    }
                }

                if (firstCard.Value == CardValue.QUEEN)
                {
                    for (int j = i + 1; j < handCards.Count; j++)
                    {
                        var secondCard = handCards[j];
                        if (secondCard.Value == CardValue.KING && firstCard.Suit == secondCard.Suit)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool HasTwenty(List<Card> handCards, Card choosenCard, Deck deck) 
        {
            if (IsFirstHand(deck))
            {
                return false;
            }

            if (choosenCard.Value == CardValue.KING)
            {
                foreach (var card in handCards)
                {
                    if (card.Value == CardValue.QUEEN && card.Suit == choosenCard.Suit)
                    {
                        return true;
                    }
                }
            } else if (choosenCard.Value == CardValue.QUEEN)
            {
                foreach (var card in handCards)
                {
                    if (card.Value == CardValue.KING && card.Suit == choosenCard.Suit)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HasForty(List<Card> handCards, Deck deck)
        {
            if (IsFirstHand(deck))
            {
                return false;
            }

            for (int i = 0; i < handCards.Count; i++)
            {
                var firstCard = handCards[i];
                if (firstCard.Value == CardValue.KING && firstCard.Suit == deck.TrumpSuit)
                {
                    for (int j = i + 1; j < handCards.Count; j++)
                    {
                        var secondCard = handCards[j];
                        if (secondCard.Value == CardValue.QUEEN && firstCard.Suit == secondCard.Suit)
                        {
                            return true;
                        }
                    }
                }

                if (firstCard.Value == CardValue.QUEEN && firstCard.Suit == deck.TrumpSuit)
                {
                    for (int j = i + 1; j < handCards.Count; j++)
                    {
                        var secondCard = handCards[j];
                        if (secondCard.Value == CardValue.KING && firstCard.Suit == secondCard.Suit)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool HasForty(List<Card> handCards, Card choosenCard, Deck deck)
        {
            return !IsFirstHand(deck) && choosenCard.Suit == deck.TrumpSuit && HasTwenty(handCards, choosenCard, deck);
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
            deck.TrumpSuit = firstCard.Suit;
            //so the first card will be on the last position (Cards are stored in List)
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

        public static void CallTwenty(Player player)
        {
            Console.WriteLine("--> Twenty! (+20)");
            player.Score += Constants.TWENTY_ANNOUNCEMENT;
        }

        public static void CallForty(Player player)
        {
            Console.WriteLine("--> Forty! (+40)");
            player.Score += Constants.FORTY_ANNOUNCEMENT;
        }

        //Swap opportunity methods
        public static bool CanSwap(List<Card> handCards, Deck deck)
        {
            // last card is always deck.Cards[deck.Cards.Count() - 1]
            if (deck.Cards.Count() < 2)
            {
                return false;
            }

            var lastCard = deck.Cards[deck.Cards.Count() - 1];
            var hasATrumpNine = handCards.FirstOrDefault(x => x.Value == CardValue.NINE && x.Suit == deck.TrumpSuit) != null;
            if (hasATrumpNine)
                return true;

            return false;
        }

        public static void SwapOpenedCard(Player player, Deck deck)
        {
            // last card is always deck.Cards[deck.Cards.Count() - 1]
            var nineTrumpCard = player.Cards.First(x => x.Value == CardValue.NINE && x.Suit == deck.TrumpSuit);
            var openedTrumpCard = deck.Cards.Last();
            
            deck.Cards.Remove(openedTrumpCard);
            deck.Cards.Add(nineTrumpCard);
            
            player.Cards.Remove(nineTrumpCard);
            player.Cards.Add(openedTrumpCard);
        }

        //Closing opportunity methods
        public static bool CanClose(Player player, Deck deck)
        {
            return player.HasWonLastHand && deck.Cards.Count() > 2 && deck.HasOpenedCard;
        }

        public static void Close(Deck deck)
        {
            deck.HasOpenedCard = false;
            deck.ThrownCards.AddRange(deck.Cards);
            deck.Cards.Clear();
        }

        //COMMON UTILITY METHODS
        public static Card DrawCard(Player player, Deck deck)
        {
            if (HasCardsInDeck(deck))
            {
                var currentCard = deck.Cards.First();
                player.Cards.Add(deck.Cards.First());
                deck.Cards.Remove(deck.Cards.First());
                return currentCard;
            }
            return null;
        }

        /*
         * Is first played card
         */
        private static bool IsFirstHand(Deck deck)
        {
            // We cannnot call Twenty on first play
            if (deck.Cards.Count() == 12)
            {
                return true;
            }

            return false;
        }

        public static bool HasCardsInDeck(Deck deck)
        {
            return deck.Cards.Count > 0;
        }

        public static bool WinsFirstCard(Card first, Card second, CardSuit trumpSuit)
        {
            if (first.Suit == trumpSuit && second.Suit != trumpSuit)
            {
                return true;
            }
            else if (first.Suit != trumpSuit && second.Suit == trumpSuit)
            {
                return false;
            }
            else if (first.Suit != second.Suit)
            {
                return true;
            }
            else
            {
                return first.Value >= second.Value;
            }
        }

        public static bool HasToAnswerWithMatching(Deck deck)
        {
            return deck.Cards.Count == 0 || deck.IsClosed;
        }

        public static bool HasAnsweringCard(Player player, Card card)
        {
            return player.Cards.FirstOrDefault(x => x.Suit == card.Suit) != null;
        }

        public static List<Card> GetHandAnsweringCards(Player player, Card card)
        {
            return player.Cards.Where(x => x.Suit == card.Suit).ToList<Card>();
        }

        public static bool IsSixtySixReached(Player player, Player other)
        {
            if (player.HasWonLastHand && player.Score >= Constants.TOTAL_SCORE)
            {
                player.WinsCount++;
                player.HasWonLastDeal = true;
                other.HasWonLastDeal = false;
                Console.WriteLine(player.ToString() + " has won!!");
                return true;
            }
            else
                return false;
        }
    }
}

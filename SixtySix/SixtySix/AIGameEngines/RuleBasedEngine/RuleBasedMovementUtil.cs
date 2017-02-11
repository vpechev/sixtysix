using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix.RuleBasedEngine
{
    public static class RuleBasedMovementUtil
    {
        public static Card GiveRuleBasedCard(Player player, Deck deck, Card playedFromOther = null)
        {
            if (playedFromOther == null)
            {
                return GiveCardIfFirst(player, deck);
            }
            else
            {
               //other has played some card
                if (deck.Cards.Count() > 0 && !deck.IsClosed)
                {
                    return GiveCardIfOtherHasPlayedPhase1(player, deck, playedFromOther);
                }
                else
                {
                    return GiveCardIfOtherHasPlayedPhase2(player, deck, playedFromOther);
                }
            }
        }

        private static Card GiveCardIfFirst(Player player, Deck deck)
        {
            var rand = new Random(System.DateTime.Now.Millisecond);
            var playerCards = player.Cards;

            if (SixtySixUtil.HasForty(playerCards, deck))
            {
                return player.Cards.First(x => x.Suit == deck.TrumpSuit && (x.Value == CardValue.KING || x.Value == CardValue.QUEEN));
            }
            else if (SixtySixUtil.HasTwenty(playerCards, deck))
            {
                return player.Cards.First(x => x.Suit != deck.TrumpSuit && (x.Value == CardValue.KING || x.Value == CardValue.QUEEN));
            }
            //if player has Ace trump and will reach 66
            else if (playerCards.FirstOrDefault(x => x.Suit == deck.TrumpSuit && x.Value == CardValue.ACE) != null && player.Score + (int)CardValue.ACE >= Constants.TOTAL_SCORE)
            {
                return playerCards.First(x => x.Suit == deck.TrumpSuit && x.Value == CardValue.ACE);
            }
            //if Ace trump is thrown and player has 10 trump and will reach 66
            else if (playerCards.FirstOrDefault(x => x.Suit == deck.TrumpSuit && x.Value == CardValue.TEN) != null && deck.ThrownCards.Contains(new Card(CardValue.ACE, deck.TrumpSuit)) && player.Score + (int)CardValue.TEN >= Constants.TOTAL_SCORE)
            {
                return playerCards.First(x => x.Suit == deck.TrumpSuit && x.Value == CardValue.TEN);
            }
            else {
                // give the lowest card
                if(deck.Cards.Count > 0 && !deck.IsClosed){
                    // 9 not trump
                    if (playerCards.FirstOrDefault(x => x.Value == CardValue.NINE && x.Suit != deck.TrumpSuit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.NINE && x.Suit != deck.TrumpSuit);
                    }
                    // Jack not trump
                    else if (playerCards.FirstOrDefault(x => x.Value == CardValue.JACK && x.Suit != deck.TrumpSuit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.JACK && x.Suit != deck.TrumpSuit);
                    }
                    // Queen not trump
                    else if (playerCards.FirstOrDefault(x => x.Value == CardValue.QUEEN && x.Suit != deck.TrumpSuit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.QUEEN && x.Suit != deck.TrumpSuit);
                    }
                    // KING not trump
                    else if (playerCards.FirstOrDefault(x => x.Value == CardValue.KING && x.Suit != deck.TrumpSuit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.KING && x.Suit != deck.TrumpSuit);
                    }
                    // supposed to be in this case only if the player has only 10 and Aces then just play a random card
                    else
                    {
                        return playerCards[rand.Next(playerCards.Count)];
                    }
                } else {
                    // the cards in deck are left OR the game is closed
                    var aces = playerCards.Where(x=>x.Value == CardValue.ACE);
                    var tenthsWhichAcesAreThrown = playerCards.Where(x=>x.Value == CardValue.TEN && deck.ThrownCards.Contains(new Card(CardValue.ACE, x.Suit)));
                    if (aces.FirstOrDefault(x => x.Suit == deck.TrumpSuit) != null)
                    {
                        return aces.First(x => x.Suit == deck.TrumpSuit);
                    }
                    else if (tenthsWhichAcesAreThrown.FirstOrDefault(x => x.Suit == deck.TrumpSuit) != null && deck.ThrownCards.Contains(new Card(CardValue.ACE, deck.TrumpSuit)))
                    {
                        return tenthsWhichAcesAreThrown.First(x => x.Suit == deck.TrumpSuit);
                    }
                    else if (aces.Count() > 0)
                    {
                        return aces.First();
                    }
                    else if (tenthsWhichAcesAreThrown.Count() > 0)
                    {
                        return tenthsWhichAcesAreThrown.First();
                    }
                    else
                    {
                        return playerCards[rand.Next(playerCards.Count)];
                    }
                }
            }
        }

        private static Card GiveCardIfOtherHasPlayedPhase1(Player player, Deck deck, Card playedFromOther)
        {
            //var rand = new Random(System.DateTime.Now.Millisecond);
            var playerCards = player.Cards;
            // if other player has played trump
            if (playedFromOther.Suit == deck.TrumpSuit)
            {
                if (playedFromOther.Value == CardValue.TEN && playerCards.FirstOrDefault(x => x.Value == CardValue.ACE && x.Suit == playedFromOther.Suit) != null)
                {
                    return playerCards.First(x => x.Value == CardValue.ACE && x.Suit == playedFromOther.Suit);
                }
                return playerCards.Min();
            }
            else
            {
                //if other player has not played trump
                if (playedFromOther.Value == CardValue.ACE) // high card not trump
                {
                    if (playerCards.FirstOrDefault(x => x.Suit == deck.TrumpSuit) != null)
                    {
                        var trumpCards = playerCards.Where(x => x.Suit == deck.TrumpSuit);
                        if ((int)trumpCards.Max().Value + (int)CardValue.ACE + player.Score >= Constants.TOTAL_SCORE)
                        {
                            return trumpCards.Max();
                        }
                        else
                        {
                            return trumpCards.Min();
                        }
                    }
                }
                    
                if (playedFromOther.Value == CardValue.TEN)
                {
                    if (playerCards.FirstOrDefault(x => x.Value == CardValue.ACE && x.Suit == playedFromOther.Suit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.ACE && x.Suit == playedFromOther.Suit);
                    }
                    else
                    {
                        var card = playerCards.Where<Card>(x => x.Suit == deck.TrumpSuit).OrderBy(x=>x.Value).FirstOrDefault();
                        if (card == null)
                        {
                            playerCards.Min();
                        }
                        return card;
                    }
                }
                else
                {
                    var winningCards = playerCards.Where(x => x.Suit == playedFromOther.Suit && x.Value > playedFromOther.Value);
                    if (winningCards != null && winningCards.Count() > 0)
                    {
                        return winningCards.Max();
                    }
                    else
                    {
                        var loosingCards = playerCards.Where(x => x.Suit != playedFromOther.Suit && x.Value <= playedFromOther.Value);
                        if (loosingCards != null && loosingCards.Count() > 0)
                        {
                            return loosingCards.Min();
                        }

                        return playerCards.Min();
                    }
                }
            }
        }


        private static Card GiveCardIfOtherHasPlayedPhase2(Player player, Deck deck, Card playedFromOther)
        {
            //var rand = new Random(System.DateTime.Now.Millisecond);
            var playerCards = player.Cards;
            var playerTrumps = playerCards.Where(x => x.Suit == playedFromOther.Suit);

            if (playerCards.Count == 0)
            {
                int a = 6;
            }
            // if other player has played trump
            if (playedFromOther.Suit == deck.TrumpSuit)
            {
                if (playerTrumps != null && playerTrumps.Count() > 0)
                {
                    if (playerTrumps.FirstOrDefault(x => x.Value > playedFromOther.Value) != null)
                    {
                        return playerTrumps.Max();
                    }
                    else {
                        return playerTrumps.Min();
                    }
                }
                else
                {
                    return playerCards.Min();
                }
            }
            else
            {
                if(SixtySixUtil.HasAnsweringCard(player, playedFromOther)) {
                    var answeringCards = SixtySixUtil.GetHandAnsweringCards(player, playedFromOther);

                    if (answeringCards.FirstOrDefault(x => x.Value > playedFromOther.Value) != null)
                    {
                        return player.Cards.Max();
                    }
                    else
                    {
                        return playerTrumps.Min();
                    }
                }
                else
                {
                    if (playedFromOther.Value == CardValue.ACE || playedFromOther.Value == CardValue.TEN && playerTrumps.Count() > 0)
                    {
                        return playerTrumps.Min();
                    }
                    else
                    {
                        return playerCards.Min();
                    }
                }
            }
        }
    }
}
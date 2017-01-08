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
            var rand = new Random(System.DateTime.Now.Millisecond);
            var playerCards = player.Cards;

            if (playedFromOther == null)
            {
                if (SixtySixUtil.HasForty(playerCards, deck))
                {
                    return player.Cards.First(x => x.Suit == deck.TrumpSuit && (x.Value == CardValue.KING || x.Value == CardValue.QUEEN));
                }
                else if (SixtySixUtil.HasTwenty(playerCards, deck))
                {
                    return player.Cards.First(x => x.Suit != deck.TrumpSuit && (x.Value == CardValue.KING || x.Value == CardValue.QUEEN));
                }
                else if (playerCards.First(x => x.Suit == deck.TrumpSuit && x.Value == CardValue.ACE) != null && player.Score + (int)CardValue.ACE >= Constants.TOTAL_SCORE)
                {
                    return playerCards.First(x => x.Suit == deck.TrumpSuit && x.Value == CardValue.ACE);
                }
                else if (playerCards.First(x => x.Suit == deck.TrumpSuit && x.Value == CardValue.TEN) != null && deck.ThrownCards.Contains(new Card(CardValue.ACE, deck.TrumpSuit)) && player.Score + (int)CardValue.TEN >= Constants.TOTAL_SCORE)
                {
                    return playerCards.First(x => x.Suit == deck.TrumpSuit && x.Value == CardValue.TEN);
                }
                else
                {
                    if (playerCards.First(x => x.Value == CardValue.NINE && x.Suit != deck.TrumpSuit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.NINE && x.Suit != deck.TrumpSuit);
                    }
                    else if (playerCards.First(x => x.Value == CardValue.JACK && x.Suit != deck.TrumpSuit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.JACK && x.Suit != deck.TrumpSuit);
                    }
                    else if (playerCards.First(x => x.Value == CardValue.QUEEN && x.Suit != deck.TrumpSuit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.QUEEN && x.Suit != deck.TrumpSuit);
                    }
                    else if (playerCards.First(x => x.Value == CardValue.KING && x.Suit != deck.TrumpSuit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.KING && x.Suit != deck.TrumpSuit);
                    }
                    else
                    {
                        return playerCards[rand.Next(playerCards.Count)];
                    }
                }
            }
            else
            {
                // if other player has played trump
                if (playedFromOther.Suit == deck.TrumpSuit)
                {
                    if (playedFromOther.Value == CardValue.TEN && playerCards.First(x => x.Value == CardValue.ACE && x.Suit == playedFromOther.Suit) != null)
                    {
                        return playerCards.First(x => x.Value == CardValue.ACE && x.Suit == playedFromOther.Suit);
                    }
                    return null;
                }
                else
                {
                    //if other player has not played trump
                    if (playedFromOther.Value == CardValue.ACE) // high card not trump
                    {
                        if (playerCards.First(x => x.Suit == deck.TrumpSuit) != null)
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
                        if (playerCards.First(x => x.Value == CardValue.ACE && x.Suit == playedFromOther.Suit) != null)
                        {
                            return playerCards.First(x => x.Value == CardValue.ACE && x.Suit == playedFromOther.Suit);
                        }
                        else
                        {
                            return playerCards.Where<Card>(x => x.Suit == deck.TrumpSuit).First();
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

                            return null;
                        }
                    }



                }
            }
        }
    }
}

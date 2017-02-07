using SixtySix.enums;
using SixtySix.RuleBasedEngine;
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
            if (playedFromOther == null && player.HasWonLastHand)
            {
                //check for swapping opened card and swap if is allowed
                if (SixtySixUtil.CanSwap(player.Cards, deck))
                {
                    SixtySixUtil.SwapOpenedCard(player, deck);
                }
            }

            Card card = GetAITurn(player, deck, playedFromOther); ;
            
            //need to andswer
            if (playedFromOther != null && SixtySixUtil.HasToAnswerWithMatching(deck))
            {
                if (!card.Suit.Equals(playedFromOther.Suit) && SixtySixUtil.HasAnsweringCard(player, playedFromOther))
                {
                    do
                    {
                        Console.WriteLine("Wrong card to answer. The algorithm is trying again");
                        card = GetAITurn(player, deck, playedFromOther);
                    } while (!card.Suit.Equals(playedFromOther.Suit));
                }
            }

            Console.WriteLine("AI Hand: " + player.ToStringPlayerCards());
            Console.WriteLine("AI has played: {0}", card);
            player.GiveCard(card);

            return card;
        }

        private static Card GetAITurn(Player player, Deck deck, Card playedFromOther = null)
        {
            Card card = null;
            if (player.PlayStrategy == PlayStrategy.MCTS)
            {
                card = GiveMCTSBasedCard(player, deck, playedFromOther);
            }
            else if (player.PlayStrategy == PlayStrategy.RuleBased)
            {
                card = RuleBasedMovementUtil.GiveRuleBasedCard(player, deck, playedFromOther);
            }
            else if (player.PlayStrategy == PlayStrategy.Random)
            {
                card = GiveRandomBasedCard(player, deck, playedFromOther);
            }
            return card;
        }

        private static Card GiveRandomBasedCard(Player player, Deck deck, Card playedFromOther = null)
        {
            var rand = new Random();
            if(playedFromOther != null){
                if ((deck.Cards.Count() == 0 || deck.IsClosed) && SixtySixUtil.HasAnsweringCard(player, playedFromOther))
                {
                    var answeringCards = SixtySixUtil.GetHandAnsweringCards(player, playedFromOther);
                    return answeringCards[rand.Next(0, answeringCards.Count)];
                }
                else
                {
                    return player.Cards[rand.Next(0, player.Cards.Count)];
                }
            } else {
                return player.Cards[rand.Next(0, player.Cards.Count)];
            }
        }

        private static Card GiveMCTSBasedCard(Player player, Deck deck, Card playedFromOther = null)
        {
            return MCTSEngine.Select(deck, player, playedFromOther);
        }

        public static int GetDeckSplittingIndex()
        {
            Random rand = new Random(System.DateTime.Now.Millisecond);
            var splitIndex = rand.Next(0, Constants.DECK_COUNT);

            return splitIndex;
        }
    }
}

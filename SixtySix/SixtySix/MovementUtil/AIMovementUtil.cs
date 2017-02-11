﻿using SixtySix.enums;
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
		public static Card MakeTurn(Player player, Player opp, Deck deck, Card playedFromOther=null)
        {
            if (playedFromOther == null && player.HasWonLastHand)
            {
                //check for swapping opened card and swap if is allowed
                if (SixtySixUtil.CanSwap(player.Cards, deck))
                {
                    SixtySixUtil.SwapOpenedCard(player, deck);
                }
            }

			Card card = GetAITurn(player,opp, deck, playedFromOther); ;
            
            //need to andswer
            if (playedFromOther != null && SixtySixUtil.HasToAnswerWithMatching(deck))
            {
                if (!card.Suit.Equals(playedFromOther.Suit) && SixtySixUtil.HasAnsweringCard(player, playedFromOther))
                {
                    do
                    {
                        Console.WriteLine("Wrong card to answer. The algorithm is trying again");
                        card = GetAITurn(player, opp, deck, playedFromOther);
                    } while (!card.Suit.Equals(playedFromOther.Suit));
                }
            }
			if (!player.IsSilent) {
				Console.WriteLine ("AI Hand: " + player.ToStringPlayerCards ());
				Console.WriteLine ("AI has played: {0}", card);
			}
            player.GiveCard(card);

            return card;
        }

		private static Card GetAITurn(Player player,Player faggot, Deck deck, Card playedFromOther = null)
        {
            Card card = null;
            if (player.PlayStrategy == PlayStrategy.MCTS)
            {
                card = GiveMCTSBasedCard(player, faggot,deck, playedFromOther);
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
					if (player.Cards.Count != 0)
						return player.Cards [rand.Next (player.Cards.Count)];
					else
						return SixtySixUtil.DrawCard(player,deck);
                }
            } else {
                return player.Cards[rand.Next(0, player.Cards.Count)];
            }
        }

		private static Card GiveMCTSBasedCard(Player player,Player faggot, Deck deck, Card playedFromOther = null)
        {
			
           // return MCTSEngine.Select(deck, player, playedFromOther);
			//TODO : IMPLEMENT THIS.
			return MCTSEngine.MCTS(player, faggot, deck,playedFromOther);
        }

        public static int GetDeckSplittingIndex()
        {
            Random rand = new Random(System.DateTime.Now.Millisecond);
            var splitIndex = rand.Next(0, Constants.DECK_COUNT);

            return splitIndex;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class GameEngine
    {
        public static Player PlaySixtySix()
        {
            var deck = CardsDeckUtil.InitializeDeck();
            var player1 = new Player(false); //human player
            var player2 = new Player(true); //AI player
            player1.HasWonLastDeal = true;

            CardsDeckUtil.shuffleDeck(deck); //we first shuffle the deck
            do
            {
                if (player1.HasWonLastDeal)
                {
                    var splitIndex = MovementUtil.GetDeckSplittingIndex(player2);
                    CardsDeckUtil.splitDeck(deck, splitIndex); //one of the players should split the deck
                    player1.HasWonLastHand = true;
                    player2.HasWonLastHand = false;
                    PlayOneDeal(deck, player1, player2);
                }
                else if (player2.HasWonLastDeal)
                {
                    var splitIndex = MovementUtil.GetDeckSplittingIndex(player1);
                    CardsDeckUtil.splitDeck(deck, splitIndex); //one of the players should split the deck
                    player2.HasWonLastHand = true;
                    player1.HasWonLastHand = false;
                    PlayOneDeal(deck, player2, player1);
                } 
                else 
                {
                    PlayOneDeal(deck, player1, player2);
                }
            }
            while (player1.WinsCount < Constants.TOTAL_PLAYS_FOR_WIN && player2.WinsCount < Constants.TOTAL_PLAYS_FOR_WIN);

            if (player1.WinsCount >= Constants.TOTAL_PLAYS_FOR_WIN)
                return player1;
            else
                return player2;
        }

        /*
         * In the context of this method player1 has always win last game
         * 
         */
        public static void PlayOneDeal(Deck deck, Player player1, Player player2)
        {
            //we have to deal the cards.
            SixtySixUtil.DealCards(deck, player1, player2);
            do
            {
                Console.WriteLine("TRUMP: {0}!!! {1} cards in the deck.", deck.Cards.Count() > 0 ? deck.Cards.Last().ToString() : deck.TrumpSuit.ToString(), deck.Cards.Count());
                Console.WriteLine("-" + player1.ToString() + " has " + player1.Score + " points");
                Console.WriteLine("-" + player2.ToString() + " has " + player2.Score + " points");
                Console.WriteLine();

                if (player1.HasWonLastHand) {
                    MakeTurn(player1, player2, deck);
                    
                    if (SixtySixUtil.IsSixtySixReached(player1, player2))
                    {
                        break;
                    }
                }
                else if (player2.HasWonLastHand)
                {
                    MakeTurn(player2, player1, deck);

                    if (SixtySixUtil.IsSixtySixReached(player2, player1))
                    {
                        break;
                    }
                }

                Console.WriteLine("============================================================================="); 
            } while (player1.Cards.Count() > 0 && player2.Cards.Count() > 0);

            CardsDeckUtil.CollectCardsInDeck(deck, player1, player2);
        }

        private static void MakeTurn(Player player1, Player player2, Deck deck)
        {
            //give card
            var card = MovementUtil.MakeTurn(player1, player2, deck, null);
            var otherCard = MovementUtil.MakeTurn(player2, player1, deck, card);
            var handScore = (int)card.Value + (int)otherCard.Value;

            // player1 plays first, so if first card wins, then the first player wins
            if (SixtySixUtil.WinsFirstCard(card, otherCard, deck.TrumpSuit))
            {
                Console.WriteLine("Winning card {0}", card);
                player1.Score += handScore;
                player1.HasWonLastHand = true;
                player2.HasWonLastHand = false;
                SixtySixUtil.DrawCard(player1, deck);
                SixtySixUtil.DrawCard(player2, deck);
            }
            else
            {
                Console.WriteLine("Winning card {0}", otherCard);
                player2.Score += handScore;
                player2.HasWonLastHand = true;
                player1.HasWonLastHand = false;
                SixtySixUtil.DrawCard(player2, deck);
                SixtySixUtil.DrawCard(player1, deck);
            }
        }
    }
}

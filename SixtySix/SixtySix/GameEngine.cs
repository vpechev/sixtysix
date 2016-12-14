using System;
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
            var player1 = new Player(); //human player
            var player2 = new Player(); //AI player

            CardsDeckUtil.shuffleDeck(deck); //we first shuffle the deck
            do
            {
                //TODO !!HAVE TO CHANGE THE INDEX FOR SPLITTNG THE DECK;
                Random rand = new Random(System.DateTime.Now.Millisecond);
                var splitIndex = rand.Next(0, Constants.DECK_COUNT);

                if (player1.HasWonLastDeal)
                {
                    CardsDeckUtil.splitDeck(deck, splitIndex); //one of the players should split the deck
                    PlayOneDeal(deck, player1, player2);
                }
                else if (player2.HasWonLastDeal)
                {
                    CardsDeckUtil.splitDeck(deck, splitIndex); //one of the players should split the deck
                    PlayOneDeal(deck, player2, player1);
                } else 
                {
                    PlayOneDeal(deck, player1, player2);
                }
            }
            while (player1.WinsCount < 11 || player2.WinsCount < 11);

            if (player1.WinsCount >= 11)
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
            
            
            Console.WriteLine(deck);
        }
    }
}

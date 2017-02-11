using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixtySix.enums;

namespace SixtySix
{
    public class MCTSEngine
    {
        /*
         * Start from the root R and select successive child nodes down to the
         * leaf node L. We have to choose most promising moves 
         */
		public static Node Select(Node root)
        {
			if (root.Children.Count () == 0)
				return Action (root);
			Node chosen = BestChildUCB (root, 1.44);
			return Select (chosen);
        }

        /*
         * unless L ends the game with a win/loss for either player, 
         * either create one or more child nodes or choose from them node C
         * 
         * If L is a not a terminal node (i.e. it does not end the game) then create one or more child nodes and select one C.
         */
        public static Card Expand(Node current, Deck deck, Player player)
        {
            if (!current.IsTerminal)
            {
                BuildTree(current, deck, player);
            }
            
            return null;


            
        }

        /*
         * play a random playout from node C. This step is sometimes also
         * called playout or rollout
         */
        public static void Simulate(Node current)
        {
            Random r = new Random(System.DateTime.Now.Millisecond);

            //helper.CopyBytes(game.board, current.state);
            //int player = Opponent(current.PlayerTookAction);

            ////Do the policy until a winner is found for the first (change?) node added
            //while (game.GetWinner() == 0)
            //{
            //    //Random
            //    List<byte> moves = game.GetValidMoves();
            //    byte move = moves[r.Next(0, moves.Count)];
            //    game.Mark(player, move);
            //    player = Opponent(player);
            //}

            //if (game.GetWinner() == startPlayer)
            //    return 1;

            //return 0;
        }


        /*
         * use result of the playout to update information in the nodes on the path from C to R
         * The updating of the number of wing in each node during this phase should arise from the player 
         * who made the move that resulted in that node. This ensures that during selection, each player's 
         * choise expand towards the most promising moves for that player, which mirrors the goal of each 
         * player to maximize the value of their move
         * 
         * 
         * Update the current move sequence with the simulation result.
         * Each node must contain two important pieces of information: 
         *      an estimated value based on simulation results and the number of times it has been visited.
         * In its simplest and most memory efficient implementation, MCTS will add one child node per iteration. 
         * Note, however, that it may be beneficial to add more than one child node per iteration depending on the application. 
         *
         */
        public static void BackPropagate(Node current, int value)
        {
            do
            {
                current.Value += value;
                current.VisitsCount++;
                current = current.Parent;
            }
            while (current != null);
        }

        public static void BuildTree(Node node, Deck deck, Player player)
        {
            //var playerCards = node.Hand;
            //var rootNode = new Node();

            
            //foreach (var card in playerCards)
            //{
            //    var availableCardsForOpponent = CardsDeckUtil.InitializeDeck().Cards; //all cards
            //    availableCardsForOpponent.Remove(deck.Cards.Last());      // - deck.OpenedCard
            //    player.Cards.ForEach(x=> availableCardsForOpponent.Remove(x) ); //- current player cards 
            //    deck.ThrownCards.ForEach(x=> availableCardsForOpponent.Remove(x) ); //- deck.ThrownCards 
                
            //    var currentNode = new Node()
            //    {
            //        Cards = playerCards,
            //        ChoosenCard = card,
            //        OpenedDeckCard = deck.Cards.Last(),
            //        ThrownFromPlayerCards = player.ThrownCards,
            //        CanBePlayedFromOpponent = availableCardsForOpponent
            //    };
            //    rootNode.Children.Add(currentNode);
            //    GenerateSubTree(currentNode, player);
            //}
        }

        private static void GenerateSubTree(Node node, Player player)
        {
            //current card played from the player
            player.Cards.Remove(node.ChoosenCard); //we should take a card

            //card played from the opponent
            //------------------------------------> opponent should give a card

            //take card from the available to fill missing in player hand
            var newCard = node.CanBePlayedFromOpponent[0];
            node.CanBePlayedFromOpponent.Remove(newCard);
            player.Cards.Add(newCard);
            
//            GenerateSubTree(node, player);
        }

		private static double UCB(Node node,double cons)
		{
			return ((double)node.Value / (double)node.VisitsCount) + cons * Math.Sqrt((2.0 * Math.Log((double)node.Parent.VisitsCount)) / (double)node.VisitsCount);

		}

        private static Node BestChildUCB(Node current, double C)
        {
			Node bestChild = new Node()
			{
				Parent=current.Parent,
				Value=0,
				VisitsCount=0,
				Hand=current.Hand,
				IsTerminal=false,
				ThrumpCard=current.ThrumpCard,
				CanBePlayedFromOpponent=current.CanBePlayedFromOpponent,
				ThrownFromPlayersCards=current.ThrownFromPlayersCards,
				CardOnTable=current.CardOnTable,
				AssignedOpponentCards=current.AssignedOpponentCards,
				Opponent=current.Opponent,
				Children=new List<Node>()
			};
            double best = double.NegativeInfinity;

            foreach (Node child in current.Children)
            {
				double UCB1 = UCB (child,C);
                if (UCB1 > best)
                {
                    bestChild = child;
                    best = UCB1;
                }
            }

            return bestChild;
        }

        /*
 * In the context of this method player1 has always win last game
 * 
 */
        public static void PlayOneDeal(Deck deck, Player player1, Player player2)
        {
            //we have to deal the cards.
            //SixtySixUtil.DealCards(deck, player1, player2);
//            int turnNumber = 1;
            do
            {
                //Console.WriteLine("TURN: {0}", turnNumber++);
                //Console.WriteLine("TRUMP: {0}!!! {1} cards in the deck.", deck.Cards.Count() > 0 ? deck.Cards.Last().ToString() : deck.TrumpSuit.ToString(), deck.Cards.Count());
                //Console.WriteLine("-" + player1.ToString() + " has " + player1.Score + " points");
                //Console.WriteLine("-" + player2.ToString() + " has " + player2.Score + " points");
                //Console.WriteLine();

                if (player1.HasWonLastHand)
                {
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

  //              Console.WriteLine("=============================================================================");
            } while (player1.Cards.Count() > 0 && player2.Cards.Count() > 0);

 //           CardsDeckUtil.CollectCardsInDeck(deck, player1, player2);
 //           Console.Clear();
        }


        private static void MakeTurn(Player player1, Player player2, Deck deck)
        {
            //give card
            var card = AIMovementUtil.MakeTurn(player1, deck, null);

            //Check for additional point -> (20 or 40)
            //TODO Idea for modification: Player choose if he wants to call his announce. If has more than one announce can choose which one wants to play.
            if (SixtySixUtil.HasForty(player1.Cards, card, deck))
            {
                SixtySixUtil.CallForty(player1);
            }
            else if (SixtySixUtil.HasTwenty(player1.Cards, card, deck))
            {
                SixtySixUtil.CallTwenty(player1);
            }

            var otherCard = AIMovementUtil.MakeTurn(player2, deck, card);
            var handScore = (int)card.Value + (int)otherCard.Value;

            deck.ThrownCards.Add(card);
            deck.ThrownCards.Add(otherCard);

            // player1 plays first, so if first card wins, then the first player wins
            if (SixtySixUtil.WinsFirstCard(card, otherCard, deck.TrumpSuit))
            {
   //             Console.WriteLine("Winning card {0}", card);
                player1.Score += handScore;
                player1.HasWonLastHand = true;
                player2.HasWonLastHand = false;
                SixtySixUtil.DrawCard(player1, deck);
                SixtySixUtil.DrawCard(player2, deck);
            }
            else
            {
     //           Console.WriteLine("Winning card {0}", otherCard);
                player2.Score += handScore;
                player2.HasWonLastHand = true;
                player1.HasWonLastHand = false;
                SixtySixUtil.DrawCard(player2, deck);
                SixtySixUtil.DrawCard(player1, deck);
            }
        }

        public List<Card> ValidMoves(Player player, Deck deck, Card playedFromOther)
        {
            List<Card> validMoves = new List<Card>();
            foreach (var card in player.Cards)
            {
                //need to andswer
                if (playedFromOther != null && SixtySixUtil.HasToAnswerWithMatching(deck))
                {
                    if (card.Suit.Equals(playedFromOther.Suit))
                    {
                        validMoves.Add(card);

                    }

                }
            }
            return validMoves;
        }


        //from the current leaf with the thrown card generate the child node
        public static Node Action(Node parrent)
        {
            
            List<Card> tmpCanBePlayedByOpponent = new List<Card>();
            List<Card> tmpHand = new List<Card>();
            List<Card> tmpAssignedOpponentCards = new List<Card>();
            tmpHand = parrent.Hand;
            tmpCanBePlayedByOpponent = parrent.CanBePlayedFromOpponent;
            tmpAssignedOpponentCards = parrent.AssignedOpponentCards;
			if (tmpAssignedOpponentCards.Count () == 0 && tmpCanBePlayedByOpponent.Count () > 6
						&&parrent.OurTurn==false) 
			{
				parrent.AssignOponentsCards ();
			}

			Random rand = new Random (System.DateTime.Now.Millisecond);
            Card tmpTrownCard = new Card();
			Card card = new Card ();
            if (parrent.OurTurn)
	        {
				card = parrent.Hand.ElementAt (rand.Next (parrent.Hand.Count()));
						
                tmpHand.Remove(card);
                tmpTrownCard = null;

	        }
            else
            {
				card=parrent.AssignedOpponentCards.ElementAt(rand.Next(parrent.AssignedOpponentCards.Count()));
                tmpAssignedOpponentCards.Remove(card);
                tmpTrownCard = card;
            }
		 
            Node child = new Node()
            {
                Parent=parrent,
                Value=0,
                VisitsCount=0,
                Hand=tmpHand,
                IsTerminal=false,
                ThrumpCard=parrent.ThrumpCard,
                CanBePlayedFromOpponent=tmpCanBePlayedByOpponent,
                ThrownFromPlayersCards=parrent.ThrownFromPlayersCards,
                CardOnTable=tmpTrownCard,
                AssignedOpponentCards=tmpAssignedOpponentCards,
                Opponent=parrent.Opponent,
                Children=new List<Node>()
            };
            child.ThrownFromPlayersCards.Add(card);
            child.AddCard(child.Hand);
            return child;
        }


		public Card MCTS(Player AIPlayer, Player faggot, Deck deck,Card cardOnTable)
		{
			var tmpCanBePlayedByOpponent = new List<Card> ();
			var tmpAssignedOpponentCards = new List<Card> ();

			tmpCanBePlayedByOpponent.AddRange (deck.Cards);
			tmpCanBePlayedByOpponent.AddRange (faggot.Cards);
			tmpCanBePlayedByOpponent.Remove (deck.Cards.Last ());
			tmpAssignedOpponentCards = new List<Card> ();
			Node root=new Node()
			{
				Parent=null,
				Value=0,
				VisitsCount=0,
				Hand=AIPlayer.Cards,
				IsTerminal=false,
				ThrumpCard=deck.Cards.Last(),
				CanBePlayedFromOpponent=tmpCanBePlayedByOpponent,
				ThrownFromPlayersCards=deck.ThrownCards,
				CardOnTable=cardOnTable,
				AssignedOpponentCards=tmpAssignedOpponentCards,
				Opponent=faggot,
				Children=new List<Node>()
			};
			var currTime = System.DateTime.Now.Millisecond;
			var startTime = System.DateTime.Now.Millisecond;

			while(currTime-startTime<1000)
			{
				Select (root);
				Expand (null, null, null);
				Simulate (null);
				BackPropagate (null, 0);
				currTime = System.DateTime.Now.Millisecond;

			}

			double max = -1;
			Node tmpNode = null;;
			foreach (var child in root.Children) {
				if (max < UCB (child, 1.44)) 
				{
					tmpNode = child;
					max = UCB (child, 1.44);
				}
			}

			return tmpNode.ChoosenCard;
		}


    }
}

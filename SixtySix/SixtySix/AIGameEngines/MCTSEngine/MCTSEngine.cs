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
		private static Node Select(Node root)
        {
			if (root.Children.Count () == 0)
				return Action (root);

			Node chosen = BestChildUCB (root, 1.44);
			return Select (chosen);
        }

        /*
         * play a random playout from node C. This step is sometimes also
         * called playout or rollout
         */
        private static int Simulate(Node current)
        {
			Player playerOne = new Player (true, true, PlayStrategy.Random);
			playerOne.Cards = current.Hand;
			Player playerTwo = new Player (true, true, PlayStrategy.Random);
			playerTwo.Cards = current.AssignedOpponentCards;
			Deck deck = new Deck ()
			{
				Cards=current.CanBePlayedFromOpponent,
				ThrownCards=current.ThrownFromPlayersCards,
				IsClosed = false,
				IsEndOfGame = false,
				HasOpenedCard = true
			};
			deck.Cards.Add (current.ThrumpCard);
			return MCTSEngine.PlayOneDeal(deck, playerOne, playerTwo);
        }

        private static void BackPropagate(Node current, int value)
        {
            do
            {
                current.Value += value;
                current.VisitsCount++;
                current = current.Parent;
            }
            while (current != null);
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
        private static int PlayOneDeal(Deck deck, Player player1, Player player2)
        {
            
            do
            {
                
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
			} while (player1.Cards.Count() > 0 && player2.Cards.Count() > 0);
			return player1.WinsCount - player2.WinsCount;
        }

        private static void MakeTurn(Player player1, Player player2, Deck deck)
        {
            //give card
            var card = AIMovementUtil.MakeTurn(player1,player2, deck, null);

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

            var otherCard = AIMovementUtil.MakeTurn(player2, player1, deck, card);
            var handScore = (int)card.Value + (int)otherCard.Value;

            deck.ThrownCards.Add(card);
            deck.ThrownCards.Add(otherCard);

            // player1 plays first, so if first card wins, then the first player wins
            if (SixtySixUtil.WinsFirstCard(card, otherCard, deck.TrumpSuit))
            {
                player1.Score += handScore;
                player1.HasWonLastHand = true;
                player2.HasWonLastHand = false;
                SixtySixUtil.DrawCard(player1, deck);
                SixtySixUtil.DrawCard(player2, deck);
            }
            else
            {
                player2.Score += handScore;
                player2.HasWonLastHand = true;
                player1.HasWonLastHand = false;
                SixtySixUtil.DrawCard(player2, deck);
                SixtySixUtil.DrawCard(player1, deck);
            }
        }

        //from the current leaf with the thrown card generate the child node
        private static Node Action(Node parrent)
        {
            List<Card> tmpCanBePlayedByOpponent = new List<Card>();
            List<Card> tmpHand = new List<Card>();
            List<Card> tmpAssignedOpponentCards = new List<Card>();
            tmpHand.AddRange(parrent.Hand);
            tmpCanBePlayedByOpponent.AddRange(parrent.CanBePlayedFromOpponent);
            tmpAssignedOpponentCards.AddRange(parrent.AssignedOpponentCards);

			if (tmpAssignedOpponentCards.Count () == 0 && tmpCanBePlayedByOpponent.Count () > 6 && parrent.OurTurn == false) 
			{
				parrent.AssignOponentsCards();
			}

			Random rand = new Random (System.DateTime.Now.Millisecond);
            Card tmpTrownCard = null;
			Card card = new Card ();
            if (parrent.OurTurn)
	        {
				card = parrent.Hand.ElementAt(rand.Next(parrent.Hand.Count()));
                tmpHand.Remove(card);
	        }
            else
            {
				card = parrent.AssignedOpponentCards.ElementAt(rand.Next(parrent.AssignedOpponentCards.Count()));
                tmpAssignedOpponentCards.Remove(card);
                tmpTrownCard = card;
            }
		 
            Node child = new Node()
            {
                Parent = parrent,
                Value = 0,
                VisitsCount = 0,
                Hand = tmpHand,
                IsTerminal = false,
                ThrumpCard = parrent.ThrumpCard,
                CanBePlayedFromOpponent = tmpCanBePlayedByOpponent,
                ThrownFromPlayersCards = parrent.ThrownFromPlayersCards,
                CardOnTable = tmpTrownCard,
                AssignedOpponentCards = tmpAssignedOpponentCards,
                Opponent = parrent.Opponent,
                Children = new List<Node>(),
				ChoosenCard = card
            };
            child.ThrownFromPlayersCards.Add(card);
            child.AddCard(child.Hand);
            return child;
        }


		public static Card MCTS(Player AIPlayer, Player faggot, Deck deck,Card cardOnTable)
		{
			var tmpCanBePlayedByOpponent = new List<Card> ();
			var tmpAssignedOpponentCards = new List<Card> ();

			tmpCanBePlayedByOpponent.AddRange(deck.Cards);
			tmpCanBePlayedByOpponent.AddRange(faggot.Cards);
			tmpCanBePlayedByOpponent.Remove(deck.Cards.Last ());

            //TODO We dont put any cards in tmpAssignedOpponentCards list, so it brokes down to the methods

			Node root=new Node()
			{
				Parent = null,
				Value = 0,
				VisitsCount = 0,
				Hand = AIPlayer.Cards,
				IsTerminal = false,
				ThrumpCard = deck.Cards.Last(),
				CanBePlayedFromOpponent = tmpCanBePlayedByOpponent,
				ThrownFromPlayersCards = deck.ThrownCards,
				CardOnTable=cardOnTable,
				AssignedOpponentCards = tmpAssignedOpponentCards,
				Opponent = faggot,
				Children = new List<Node>()
			};
			var currTime = System.DateTime.Now.Millisecond;
			var startTime = System.DateTime.Now.Millisecond;

			while(currTime - startTime < 1000)
			{
				var current = Select (root);

				int value = Simulate (current);
				BackPropagate(current, value);
				currTime = System.DateTime.Now.Millisecond;
			}

			double max = double.NegativeInfinity;;
			Node tmpNode = null;

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

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
            if (root.IsTerminal)
                return root;

            if (root.Children.Count() == 0)
                return Action(root);

            Node chosen = BestChildUCB(root, 1.44);
            return Select(chosen);
        }

        /*
         * play a random playout from node C. This step is sometimes also
         * called playout or rollout
         */
        private static int Simulate(Node current)
        {
            Player playerOne = new Player(true, true, PlayStrategy.Random);
            playerOne.Cards = current.Hand;
            playerOne.Score = current.OurScore;
            Player playerTwo = new Player(true, true, PlayStrategy.Random);
            playerTwo.Cards = current.AssignedOpponentCards;
            playerTwo.Score = current.Opponent.Score;
            Deck deck = new Deck()
            {
                Cards = new List<Card>(),
                ThrownCards = new List<Card>(),
                IsClosed = false,
                IsEndOfGame = false,
                HasOpenedCard = true
            };

            deck.Cards.AddRange(current.CanBePlayedFromOpponent);
            deck.ThrownCards.AddRange(current.ThrownFromPlayersCards);
            deck.Cards.Add(current.ThrumpCard);

            if (playerOne.HasWonLastDeal || playerOne.HasWonLastHand)
            {
                current.OurTurn = true;
            }
            else
            {
                current.OurTurn = false;
            }

            if (current.CardOnTable != null)
            {
                if (current.OurTurn)
                    MakeTurn(playerTwo, playerOne, deck, current.CardOnTable);
                else
                    MakeTurn(playerOne, playerTwo, deck, current.CardOnTable);
            }
            return MCTSEngine.PlayOneDeal(deck, playerOne, playerTwo, current.CardOnTable);
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

        private static double UCB(Node node, double cons)
        {
            if (node.Parent == null)
                return double.NegativeInfinity;
            return ((double)node.Value / (double)node.VisitsCount) + cons * Math.Sqrt((2.0 * Math.Log((double)node.Parent.VisitsCount)) / (double)node.VisitsCount);
        }

        private static Node BestChildUCB(Node current, double C)
        {
            Node bestChild = null;

            double best = double.NegativeInfinity;
            if (current.AlreadyUsedForChild.Count() < current.CanBePlayedFromOpponent.Count())
            {
                bestChild = new Node()
                {
                    Parent = current.Parent,
                    IsTerminal = false,
                    ThrumpCard = current.ThrumpCard,
                    CardOnTable = current.CardOnTable,
                    Opponent = current.Opponent,
                    OurScore = current.OurScore
                };

                bestChild.CanBePlayedFromOpponent.AddRange(current.CanBePlayedFromOpponent);
                bestChild.ThrownFromPlayersCards.AddRange(current.ThrownFromPlayersCards);
                bestChild.AssignedOpponentCards.AddRange(current.AssignedOpponentCards);
                bestChild.Hand = current.Hand;

                best = UCB(bestChild, C);
            }

            foreach (Node child in current.Children)
            {
                double UCB1 = UCB(child, C);
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
        public static int PlayOneDeal(Deck deck, Player player1, Player player2, Card cardOnTable)
        {
            while (player1.Cards.Count() > 0 && player2.Cards.Count() > 0)
            {
                if (player1.HasWonLastHand)
                {
                    MakeTurn(player1, player2, deck, cardOnTable);

                    if (SixtySixUtil.IsSixtySixReached(player1, player2))
                    {
                        break;
                    }
                }
                else if (player2.HasWonLastHand)
                {
                    MakeTurn(player2, player1, deck, cardOnTable);

                    if (SixtySixUtil.IsSixtySixReached(player2, player1))
                    {
                        break;
                    }
                }
            }
            return player1.WinsCount - player2.WinsCount;
        }

        private static void MakeTurn(Player player1, Player player2, Deck deck, Card cardOnTable)
        {
            var card = new Card();
            var otherCard = new Card();
            int handScore;
            //give card
            if (cardOnTable != null)
            {
                card = AIMovementUtil.MakeTurn(player1, player2, deck, cardOnTable);
                otherCard = cardOnTable;
            }
            else
            {
                card = AIMovementUtil.MakeTurn(player1, player2, deck, cardOnTable);

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
                otherCard = AIMovementUtil.MakeTurn(player2, player1, deck, card);
            }

            handScore = (int)card.Value + (int)otherCard.Value;

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
            if (parrent.IsTerminal)
            {
                return null;
            }

            List<Card> tmpCanBePlayedByOpponent = new List<Card>();
            List<Card> tmpHand = new List<Card>();
            List<Card> tmpAssignedOpponentCards = new List<Card>();
            tmpHand.AddRange(parrent.Hand);
            tmpCanBePlayedByOpponent.AddRange(parrent.CanBePlayedFromOpponent);
            tmpAssignedOpponentCards.AddRange(parrent.AssignedOpponentCards);

            if (tmpAssignedOpponentCards.Count() == 0 && tmpCanBePlayedByOpponent.Count() > 6 && parrent.OurTurn == false)
            {
                parrent.AssignOponentsCards();
            }

            Random rand = new Random(System.DateTime.Now.Millisecond);
            Card tmpTrownCard = null;
            Card card = null;

            if (parrent.OurTurn)
            {
                card = parrent.Hand.ElementAt(rand.Next(parrent.Hand.Count()));
                tmpHand.Remove(card);
            }
            else
            {
                if (parrent.AssignedOpponentCards.Count != 0)
                {
                    if (parrent.AssignedOpponentCards == null || parrent.AssignedOpponentCards.Count == 0)
                    {
                        parrent.AssignOponentsCards();
                    }
                    card = parrent.AssignedOpponentCards.ElementAt(rand.Next(parrent.AssignedOpponentCards.Count));
                }
                tmpAssignedOpponentCards.Remove(card);
                tmpTrownCard = card;
            }

            Node child = new Node()
            {
                Parent = parrent,
                Hand = tmpHand,
                ThrumpCard = parrent.ThrumpCard,
                CanBePlayedFromOpponent = tmpCanBePlayedByOpponent,
                ThrownFromPlayersCards = parrent.ThrownFromPlayersCards,
                CardOnTable = tmpTrownCard,
                AssignedOpponentCards = tmpAssignedOpponentCards,
                Opponent = parrent.Opponent,
                ChoosenCard = card,
                OurScore = parrent.OurScore
            };

            parrent.Children.Add(child);

            if (parrent.Opponent.HasWonLastDeal || parrent.Opponent.HasWonLastHand)
            {
                child.OurTurn = false;
            }
            else
            {
                child.OurTurn = true;
            }

            child.ThrownFromPlayersCards.Add(card);
            child.AddCard(child.Hand);

            return child;
        }

        public static double getTime()
        {
            return System.DateTime.Now.Hour * 60 * 60 * 1e4 +
                   System.DateTime.Now.Minute * 60 * 1e4 +
                   System.DateTime.Now.Second * 1e4 +
                   System.DateTime.Now.Millisecond;

        }


        public static Card MCTS(Player AIPlayer, Player opponent, Deck deck, Card cardOnTable)
        {
            var tmpCanBePlayedByOpponent = new List<Card>();
            var tmpAssignedOpponentCards = new List<Card>();

            tmpCanBePlayedByOpponent.AddRange(deck.Cards);
            tmpCanBePlayedByOpponent.AddRange(opponent.Cards);
            if (deck.Cards.Count > 0)
                tmpCanBePlayedByOpponent.Remove(deck.Cards.Last());

            Node root = new Node()
            {
                Hand = AIPlayer.Cards,
                ThrumpCard = deck.Cards.LastOrDefault(),
                CanBePlayedFromOpponent = tmpCanBePlayedByOpponent,
                ThrownFromPlayersCards = deck.ThrownCards,
                CardOnTable = cardOnTable,
                AssignedOpponentCards = tmpAssignedOpponentCards,
                Opponent = opponent,
                OurScore = AIPlayer.Score,
                OurTurn = true
            };

            var currTime = getTime();
            var startTime = currTime;

            while (currTime - startTime < 1000)
            {
                var current = Select(root);
                int value = Simulate(current);
                BackPropagate(current, value);
                currTime = getTime();
            }

            double max = double.NegativeInfinity; ;
            Node tmpNode = null;

            foreach (var child in root.Children)
            {
                if (max < UCB(child, 1.44))
                {
                    tmpNode = child;
                    max = UCB(child, 1.44);
                }
            }

            return tmpNode.ChoosenCard;
        }
    }
}

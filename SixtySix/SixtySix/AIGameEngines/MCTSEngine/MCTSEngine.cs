using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class MCTSEngine
    {
        /*
         * Start from the root R and select successive child nodes down to the
         * leaf node L. We have to choose most promising moves 
         */
        public static Card Select(Deck deck, Player player, Card playedFromOther=null)
        {
            Node current = new Node()
            {
                
            };
            while (!current.IsTerminal)
            {
                List<Card> validMoves = current.Cards;

                if (validMoves.Count > current.Children.Count())
                    return Expand(current, deck, player);
                else
                    current = BestChildUCB(current, 1.44);
            }

            return current.ChoosenCard;
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
            //return ChooseCardFromConfiguration(current);
            return null;


            //Copy current state to the game
            //helper.CopyBytes(game.board, current.state);

            //List<byte> validMoves = game.GetValidMoves(current.state);

            //for (int i = 0; i < validMoves.Count; i++)
            //{
            //    //We already have evaluated this move
            //    if (current.children.Exists(a => a.action == validMoves[i]))
            //        continue;

            //    int playerActing = Opponent(current.PlayerTookAction);

            //    Node node = new Node(current, validMoves[i], playerActing);
            //    current.children.Add(node);

            //    //Do the move in the game and save it to the child node
            //    game.Mark(playerActing, validMoves[i]);
            //    helper.CopyBytes(node.state, game.board);

            //    //Return to the previous game state
            //    helper.CopyBytes(game.board, current.state);

            //    return node;
            //}

            //throw new Exception("Error");
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
            var playerCards = node.Cards;
            var rootNode = new Node();

            
            foreach (var card in playerCards)
            {
                var availableCardsForOpponent = CardsDeckUtil.InitializeDeck().Cards; //all cards
                availableCardsForOpponent.Remove(deck.Cards.Last());      // - deck.OpenedCard
                player.Cards.ForEach(x=> availableCardsForOpponent.Remove(x) ); //- current player cards 
                deck.ThrownCards.ForEach(x=> availableCardsForOpponent.Remove(x) ); //- deck.ThrownCards 
                
                var currentNode = new Node()
                {
                    Cards = playerCards,
                    ChoosenCard = card,
                    OpenedDeckCard = deck.Cards.Last(),
                    ThrownFromPlayerCards = player.ThrownCards,
                    CanBePlayedFromOpponent = availableCardsForOpponent
                };
                rootNode.Children.Add(currentNode);
                GenerateSubTree(currentNode, player);
            }
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
            
            GenerateSubTree(node, player);
        }

        private static Node BestChildUCB(Node current, double C)
        {
            Node bestChild = null;
            double best = double.NegativeInfinity;

            foreach (Node child in current.Children)
            {
                double UCB1 = ((double)child.Value / (double)child.VisitsCount) + C * Math.Sqrt((2.0 * Math.Log((double)current.VisitsCount)) / (double)child.VisitsCount);

                if (UCB1 > best)
                {
                    bestChild = child;
                    best = UCB1;
                }
            }

            return bestChild;
        }
    }
}

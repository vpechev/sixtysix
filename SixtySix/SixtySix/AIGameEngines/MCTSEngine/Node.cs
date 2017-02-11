using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class Node
    {
        public Node()
        {
            Children = new List<Node>();
        }
    
        public List<Card> Hand { get; set; }
        public Card ChoosenCard { get; set; }
        public List<Card> ThrownFromPlayersCards { get; set; }
        public Card ThrumpCard { get; set; }
        public bool IsTerminal { get; set; }

        public Card CardOnTable { get; set; }
        public List<Card> CanBePlayedFromOpponent { get; set; }

        public int Value { get; set; }                              //score from the trick
        public int VisitsCount { get; set; }
        public List<Node> Children { get; set; }
        public Node Parent { get; set; }
        public Player Opponent { get; set; }
        public List<Card> AssignedOpponentCards { get; set; }

        public Boolean OurTurn { get; set; }

		public List<Card> AssignOppCards(){
			List<Card> opponentsCards=new List<Card>();
			CardsDeckUtil.ShuffleCards(CanBePlayedFromOpponent);
			List<Card> hasAnons = Opponent.HasTwentyForty;
			foreach (var card in hasAnons)
			{
				if (CanBePlayedFromOpponent.First(x => x.Value == card.Value && x.Suit == card.Suit) != null)
				{
					opponentsCards.Add(card);
					CanBePlayedFromOpponent.Remove(card);
				}
			}
			
			while (opponentsCards.Count()<6)
			{
				opponentsCards.Add(opponentsCards.First());
				CanBePlayedFromOpponent.Remove(opponentsCards.First());
			}
			return opponentsCards;
		}



		public void AssignOponentsCards()
		{
			AssignedOpponentCards = AssignOppCards();
        }

        public void AddChildren(Node node) 
        {
            Children.Add(node);
            node.Parent=this;
        }

        public void DetermineTerminal()
        {
            if (Value == 66)
            {
                IsTerminal = true;
            }
        }

        public void AddCard(List<Card> hand)
        {
            CardsDeckUtil.ShuffleCards(CanBePlayedFromOpponent);
            hand.Add(CanBePlayedFromOpponent.First());
            CanBePlayedFromOpponent.Remove(CanBePlayedFromOpponent.First());
            
        }
    }
}

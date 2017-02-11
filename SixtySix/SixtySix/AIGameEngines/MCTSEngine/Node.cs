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
			AlreadyUsedForChild = new List<Card> ();
			DetermineTerminal ();
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
		public List<Card> AlreadyUsedForChild { get; set; }
		public int OurScore{ get; set; }

        public Boolean OurTurn { get; set; }

		public List<Card> AssignOppCards(){
			List<Card> opponentsCards=new List<Card>();
			CardsDeckUtil.ShuffleCards(CanBePlayedFromOpponent);
			List<Card> hasAnons = Opponent.HasTwentyForty;
			foreach (var card in hasAnons)
			{
				if (CanBePlayedFromOpponent != null &&
					CanBePlayedFromOpponent.Count() !=0 &&
					CanBePlayedFromOpponent.First(x => x.Value == card.Value && x.Suit == card.Suit) != null)
				{
					opponentsCards.Add(card);
					CanBePlayedFromOpponent.Remove(card);
				}
			}
			
			while (opponentsCards.Count() < 6 && CanBePlayedFromOpponent.Count() > 0)
			{
				opponentsCards.Add(CanBePlayedFromOpponent.First());
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
			if (OurScore == 66)
            {
                IsTerminal = true;
            }
        }
			
        public void AddCard(List<Card> hand)
        {
			if (CanBePlayedFromOpponent.Count == 0)
				return;
			List<Card> tmp = CanBePlayedFromOpponent;
			foreach (var x in Parent.AlreadyUsedForChild) 
				tmp.Remove (x);
			
            CardsDeckUtil.ShuffleCards(tmp);
            hand.Add(tmp.First());
            CanBePlayedFromOpponent.Remove(tmp.First());
        }
    }
}

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

        public List<Card> Cards { get; set; }
        public Card ChoosenCard { get; set; }
        public List<Card> ThrownFromPlayerCards { get; set; }
        public Card OpenedDeckCard { get; set; }
        public bool IsTerminal { get; set; }

        public int Value { get; set; }
        public int VisitsCount { get; set; }
        public List<Node> Children { get; set; }
        public Node Parent { get; set; }
    }
}

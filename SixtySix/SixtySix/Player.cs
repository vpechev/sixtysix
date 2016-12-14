using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class Player
    {
        public List<Card> Cards { get; set; }
        public int Score { get; set; }
        public int WinsCount { get; set; }
        public int LossCount { get; set; }
        public bool HasWonLastDeal { get; set; }
    }
}

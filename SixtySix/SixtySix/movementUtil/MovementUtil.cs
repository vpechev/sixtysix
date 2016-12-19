using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySix
{
    public class MovementUtil
    {
        public static Card MakeTurn(Player player, Player other, Deck deck, Card playedFromOther=null)
        {
            player.IsOnTurn = false;
            other.IsOnTurn = true;

            if (player.IsAIPlayer)
                return AIMovementUtil.MakeTurn(player, deck, playedFromOther);
            else
                return InputPlayerMovementUtil.MakeTurn(player, deck, playedFromOther);
        }

        public static int GetDeckSplittingIndex(Player player)
        {
            if (player.IsAIPlayer)
            {
                return AIMovementUtil.GetDeckSplittingIndex();
            }
            else
            {
                return InputPlayerMovementUtil.GetDeckSplittingIndex();
            }
        }
    }
}

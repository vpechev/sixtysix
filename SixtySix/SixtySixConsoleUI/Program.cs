using SixtySix;
using SixtySix.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySixConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var player1 = new Player(); //human player
            var player2 = new Player(true, PlayStrategy.Random); //random AI player
            //var player2 = new Player(true, PlayStrategy.RuleBased); //ES AI player
            //var player2 = new Player(true, PlayStrategy.MCTS); //Monte Carlo AI player
            GameEngine.PlaySixtySix(player1, player2);
            Console.ReadLine();
        }
    }
}

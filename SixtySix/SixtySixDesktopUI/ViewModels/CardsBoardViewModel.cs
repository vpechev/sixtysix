using SixtySix;
using SixtySix.enums;
using SixtySixDesktopUI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SixtySixDesktopUI.ViewModels
{
    public class CardsBoardViewModel : ViewModelBase
    {
        public CardsBoardViewModel()
        {
            this.Player = new Player(false);
            this.Opponent = new Player(true, PlayStrategy.Random);
            this.Deck = CardsDeckUtil.InitializeDeck();
            CardsDeckUtil.ShuffleDeck(this.Deck);
            SixtySixUtil.DealCards(Deck, Player, Opponent);
        }

        public Deck Deck { get; set; }
        public Card TrumpCard { get { return Deck.Cards.Last();  } }
        public Player Player { get; set; }
        public Player Opponent { get; set; }

        public int CurrentDealPlayerScore { get; set; }

        public ICommand giveCardCommand { get; set; }

        public Card CurrentCard { get; set; }

        public ICommand GiveCard {
            get
            {
                if (this.giveCardCommand == null)
                {
                    this.giveCardCommand = new RelayCommand(this.HandleGiveCardCommand);
                }
                return this.giveCardCommand;
            }
        }

        //TODO: to be removed;
        private string message;
        public string TestMessage {
            get { return this.message; }
            set {
                this.message = value; 
                this.OnPropertyChanges("TestMessage"); 
            } 
        }

        private void HandleGiveCardCommand(object parameter)
        {
            this.TestMessage = string.Format("Current card: {0}", CurrentCard);
        }

    }
}

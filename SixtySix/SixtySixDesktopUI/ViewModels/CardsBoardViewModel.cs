using SixtySix;
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
        public List<CardViewModel> PlayerCards { get; set; }

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

        public CardsBoardViewModel()
        {
            PlayerCards = new List<CardViewModel>();
            PlayerCards.Add(new CardViewModel()
            {
                CardValue = CardValue.TEN,
                CardSuit = CardSuit.CLUB
            });
        }

        private void HandleGiveCardCommand(object parameter)
        {
            this.TestMessage = string.Format("Current card: {0}", CurrentCard);
        }

    }
}

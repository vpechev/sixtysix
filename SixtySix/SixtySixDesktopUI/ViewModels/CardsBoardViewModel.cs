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

        #region Properties
        public Deck Deck { get; set; }
        public Card TrumpCard { get { return Deck.Cards.Last();  } }
        public Player Player { get; set; }
        public Player Opponent { get; set; }
        //TODO Temp variable to be removed
        public Card CurrentCard { get; set; }
        //TODO: to be removed;
        private Card playerSelectedCard;
        public Card PlayerSelectedCard
        {
            get
            {
                return playerSelectedCard;
            }
            set
            {
                this.playerSelectedCard = value;
                this.OnPropertyChanged("PlayerSelectedCard");
            }
        }
        private Card opponentSelectedCard;
        public Card OpponentSelectedCard
        {
            get
            {
                return opponentSelectedCard;
            }
            set
            {
                this.opponentSelectedCard = value;
                this.OnPropertyChanged("OpponentSelectedCard");
            }
        }
        private string testMessage;
        public string TestMessage
        {
            get { return this.testMessage; }
            set
            {
                this.testMessage = value;
                this.OnPropertyChanged("TestMessage");
            }
        }
        #endregion

        #region Commands
        public ICommand giveCardCommand { get; set; }
       
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
        #endregion

        #region Event handlers
        private void HandleGiveCardCommand(object parameter)
        {
            this.PlayerSelectedCard = (Card)parameter;
            //TODO should update the view and remove the card from the player hand
            this.Player.Cards.Remove(this.PlayerSelectedCard);
            //other player has to give a card;
            //TODO Sleep current thread for a second 
            //this.OpponentSelectedCard = AIMovementUtil.MakeTurn(this.Opponent, this.Deck, this.PlayerSelectedCard);
        }
        #endregion

    }
}

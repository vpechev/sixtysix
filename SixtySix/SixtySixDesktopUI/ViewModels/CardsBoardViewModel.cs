using SixtySix;
using SixtySix.enums;
using SixtySixDesktopUI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace SixtySixDesktopUI.ViewModels
{
    public class CardsBoardViewModel : ViewModelBase
    {
        public CardsBoardViewModel()
        {
            this.Player = new PlayerViewModel(false);
            this.Opponent = new PlayerViewModel(true, PlayStrategy.Random);
            this.Deck = CardsDeckUtil.InitializeDeck();
            CardsDeckUtil.ShuffleDeck(this.Deck);
            var player = this.Player.ToPlayer();
            var opponent = this.Opponent.ToPlayer();
            SixtySixUtil.DealCards(this.Deck, player, opponent);
            this.Player = PlayerViewModel.ConvertToPlayerViewModel(player);
            this.Opponent = PlayerViewModel.ConvertToPlayerViewModel(opponent);
        }

        #region Properties
        public Deck Deck { get; set; }
        public Card TrumpCard { 
            get {
                if (Deck.Cards.Count == 0)
                    return null;
                return Deck.Cards.Last();
            } 
            set
            {
                this.OnPropertyChanged("TrumpCard");
            }
        }
        public PlayerViewModel Player { get; set; }
        public PlayerViewModel Opponent { get; set; }
        //TODO Temp variable to be removed
        public Card CurrentCard { get; set; }
        //TODO: to be removed;
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
        public ICommand changeTrumpCardCommand { get; set; }
        public ICommand closeCommand { get; set; }

        public ICommand GiveCard
        {
            get
            {
                if (this.giveCardCommand == null)
                {
                    this.giveCardCommand = new RelayCommand(this.HandleGiveCardCommand);
                }
                return this.giveCardCommand;
            }
        }

        public ICommand ChangeTrumpCard
        {
            get
            {
                if (this.changeTrumpCardCommand == null)
                {
                    this.changeTrumpCardCommand = new RelayCommand(this.HandleChangeTrumpCardCommand);
                }
                return this.changeTrumpCardCommand;
            }
        }

        public ICommand Close
        {
            get
            {
                if (this.closeCommand == null)
                {
                    this.closeCommand = new RelayCommand(this.HandleCloseCommand);
                }
                return this.closeCommand;
            }
        }
        #endregion

        #region Event handlers
        /*
         * TODO 
         *      Handle reaching 66
         *             calling 20
         *             calling 40
         */
        private void HandleGiveCardCommand(object parameter)
        {
            if (this.Opponent.HasWonLastHand && this.Opponent.SelectedCard == null)
            {
                return;
            }

            this.Player.SelectedCard = (CardViewModel)parameter;
            this.Player.Cards.Remove(this.Player.SelectedCard);

            if (Deck.Cards.Count == 0)
            {
                this.TrumpCard = null;
            }

            if (this.Opponent.SelectedCard == null)
            {
                var opponentCard = CardViewModel.ConvertToCardViewModel(AIMovementUtil.MakeTurn(this.Opponent.ToPlayer(), this.Deck, this.Player.SelectedCard.ToCard()));
                this.Opponent.SelectedCard = opponentCard;
                this.Opponent.Cards.Remove(this.Opponent.SelectedCard);
            }
            
            var handScore = (int)this.Player.SelectedCard.Value + (int)this.Opponent.SelectedCard.Value;

            if (SixtySixUtil.WinsFirstCard(this.Player.SelectedCard.ToCard(), this.Opponent.SelectedCard.ToCard(), this.Deck.TrumpSuit))
            {
                this.Player.HasWonLastHand = true;
                this.Opponent.HasWonLastHand = false;

                this.TestMessage = "Player wins";
                //the player holds the hand
                this.Player.Score = this.Player.Score + handScore;
                
                var newPlayerCard = SixtySixUtil.DrawCard(this.Player.ToPlayer(), this.Deck);
                if(newPlayerCard != null){
                    this.Player.Cards.Add(CardViewModel.ConvertToCardViewModel(newPlayerCard));
                    var newOpponentCard = SixtySixUtil.DrawCard(this.Opponent.ToPlayer(), this.Deck);
                    if (newOpponentCard != null)
                    {
                        this.Opponent.Cards.Add(CardViewModel.ConvertToCardViewModel(newOpponentCard));
                    }
                }

                Task.Delay(1000).ContinueWith(_ =>
                {
                    this.Player.SelectedCard = null;
                    this.Opponent.SelectedCard = null;
                } );
                
            } else {
                //the opponent hold the hand
                this.Opponent.HasWonLastHand = true;
                this.Player.HasWonLastHand = false;

                this.TestMessage = "Opponent wins";
                this.Opponent.Score = this.Opponent.Score + handScore;

                var newOpponentCard = SixtySixUtil.DrawCard(this.Opponent.ToPlayer(), this.Deck);
                if (newOpponentCard != null)
                {
                    this.Opponent.Cards.Add(CardViewModel.ConvertToCardViewModel(newOpponentCard));
                    var newPlayerCard = SixtySixUtil.DrawCard(this.Player.ToPlayer(), this.Deck);
                    if (newPlayerCard != null)
                    {
                        this.Player.Cards.Add(CardViewModel.ConvertToCardViewModel(newPlayerCard));
                    }
                }

                CardViewModel opponentCard = null;

                Task.Delay(1000).ContinueWith(_ =>
                {
                    this.Player.SelectedCard = null;
                    this.Opponent.SelectedCard = null;
                });

                opponentCard = CardViewModel.ConvertToCardViewModel(AIMovementUtil.MakeTurn(this.Opponent.ToPlayer(), this.Deck));
                this.Opponent.Cards.Remove(opponentCard);
            
                Task.Delay(1500).ContinueWith(_ =>
                {
                    this.Opponent.SelectedCard = opponentCard;
                });
            }
        }

        /*
         * TODO 
         *      changing the trumpCard - implement the method
         */
        private void HandleChangeTrumpCardCommand(object parameter)
        {
            
        }

        /*
         * TODO 
         *      close trumpCard - implement the method
         */
        private void HandleCloseCommand(object parameter)
        {

        }

        public void asd()
        {
            this.Player.SelectedCard = null;
            this.Opponent.SelectedCard = null;
        }
        #endregion

    }
}

using SixtySix;
using SixtySix.enums;
using SixtySixDesktopUI.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            this.Opponent = new PlayerViewModel(true, PlayStrategy.MCTS);
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

        //private CardViewModel trumpCard;
        public CardViewModel TrumpCard
        {
            get
            {
                if (Deck.Cards == null || Deck.Cards.Count == 0 || Deck.IsEndOfGame)
                    return null;
                return CardViewModel.ConvertToCardViewModel(Deck.Cards.Last());
            }
            set
            {
                if (value != null)
                {
                    Deck.Cards.Add(((CardViewModel)value).ToCard());
                }
                this.OnPropertyChanged("TrumpCard");
            }
        }
        public PlayerViewModel Player { get; set; }
        public PlayerViewModel Opponent { get; set; }
        //TODO Temp variable to be removed
        public Card CurrentCard { get; set; }
        //TODO: to be removed;
        private string boardMessage;
        public string BoardMessage
        {
            get { return this.boardMessage; }
            set
            {
                this.boardMessage = value;
                this.OnPropertyChanged("BoardMessage");
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
         *      Handle reaching 66 (Note: handle end of game by cannling) 
         */
        private void HandleGiveCardCommand(object parameter)
        {
            //This check handle the case in which input player gives card before the AI player, when the AI player is on turn
            if (this.Opponent.HasWonLastHand && this.Opponent.SelectedCard == null)
            {
                return;
            }

            if (Deck.Cards.Count == 0)
            {
                this.TrumpCard = null;
            }

            this.Player.Messages = null;
            this.Opponent.Messages = null;


            if (parameter != null)
            {
                var playerCard = (CardViewModel)parameter;

                if (this.Opponent.SelectedCard != null
                    && SixtySixUtil.HasToAnswerWithMatching(this.Deck, this.Opponent.SelectedCard.ToCard())
                    && SixtySixUtil.HasAnsweringCard(this.Player.ToPlayer(), this.Opponent.SelectedCard.ToCard()))
                {
                    var playedFromOther = this.Opponent.SelectedCard.ToCard();
                    var answeringCards = SixtySixUtil.GetHandAnsweringCards(this.Player.ToPlayer(), playedFromOther);
                    if (!answeringCards.Contains(playerCard.ToCard()))
                    {
                        this.BoardMessage = "Player, you have to answer with matching card!!!";
                        return;
                    }
                }

                this.Player.SelectedCard = (CardViewModel)parameter;
                if (this.Opponent.SelectedCard == null)
                {
                    HandleCallingAnnounce(this.Player, this.Deck);
                    if (HandleEndOfDeal(this.Player, this.Opponent, this.Deck))
                    {
                        return;
                    }
                }
            }

            bool winsFirstCard;
            if (this.Opponent.SelectedCard == null)
            {
                var opponentCard = CardViewModel.ConvertToCardViewModel(AIMovementUtil.MakeTurn(this.Opponent.ToPlayer(), this.Player.ToPlayer(), this.Deck, this.Player.SelectedCard.ToCard()));
                this.Opponent.SelectedCard = opponentCard;
                winsFirstCard = SixtySixUtil.WinsFirstCard(this.Player.SelectedCard.ToCard(), this.Opponent.SelectedCard.ToCard(), this.Deck.TrumpSuit);
            }
            else
            {
                winsFirstCard = !SixtySixUtil.WinsFirstCard(this.Opponent.SelectedCard.ToCard(), this.Player.SelectedCard.ToCard(), this.Deck.TrumpSuit);
            }

            var handScore = (int)this.Player.SelectedCard.Value + (int)this.Opponent.SelectedCard.Value;



            if (winsFirstCard)
            {
                this.Player.HasWonLastHand = true;
                this.Opponent.HasWonLastHand = false;


                this.BoardMessage = "Player wins";
                //the player holds the hand
                this.Player.Score = this.Player.Score + handScore;

                var newPlayerCard = SixtySixUtil.DrawCard(this.Player.ToPlayer(), this.Deck);
                if (newPlayerCard != null)
                {
                    this.Player.Cards.Add(CardViewModel.ConvertToCardViewModel(newPlayerCard));
                    var newOpponentCard = SixtySixUtil.DrawCard(this.Opponent.ToPlayer(), this.Deck);
                    if (newOpponentCard != null)
                    {
                        this.Opponent.Cards.Add(CardViewModel.ConvertToCardViewModel(newOpponentCard));
                    }
                }

                Task.Delay(1000).ContinueWith(_ =>
                {
                    if (HandleEndOfDeal(this.Player, this.Opponent, this.Deck))
                    {
                        this.Player.SelectedCard = null;
                        this.Opponent.SelectedCard = null;
                        return;
                    }
                    this.Player.SelectedCard = null;
                    this.Opponent.SelectedCard = null;
                });

            }
            else
            {
                //the opponent hold the hand
                this.Opponent.HasWonLastHand = true;
                this.Player.HasWonLastHand = false;

                this.BoardMessage = "Opponent wins";
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

                if (HandleEndOfDeal(this.Opponent, this.Player, this.Deck))
                {
                    return;
                }

                Task.Delay(1000).ContinueWith(_ =>
                {
                    this.Player.SelectedCard = null;
                    this.Opponent.SelectedCard = null;
                });

                if (this.Player.SelectedCard == null)
                {
                    ChangeTrumpCardLogic(this.Opponent);
                }

                opponentCard = CardViewModel.ConvertToCardViewModel(AIMovementUtil.MakeTurn(this.Opponent.ToPlayer(), this.Player.ToPlayer(), this.Deck));
                this.Opponent.Cards.Remove(opponentCard);


                Task.Delay(1500).ContinueWith(_ =>
                {
                    if (Deck.Cards.Count == 0)
                    {
                        this.TrumpCard = null;
                    }

                    this.Opponent.SelectedCard = opponentCard;
                    if (this.Player.SelectedCard == null)
                    {
                        HandleCallingAnnounce(this.Opponent, this.Deck);
                        HandleEndOfDeal(this.Opponent, this.Player, this.Deck);
                    }
                });
            }
        }

        private void HandleChangeTrumpCardCommand(object parameter)
        {
            ChangeTrumpCardLogic(this.Player);
        }

        private void ChangeTrumpCardLogic(PlayerViewModel player)
        {
            if (SixtySixUtil.CanSwap(CardViewModel.ConvertListOfCardViewModelsToListOFCard(player.Cards), this.Deck))
            {
                var card = player.Cards.FirstOrDefault(x => { return x.Suit == this.Deck.TrumpSuit && x.Value == CardValue.NINE; });
                if (card != null && player.Cards.Contains(card))
                {
                    player.Cards.Add(this.TrumpCard);
                    this.Deck.Cards.Remove(this.TrumpCard.ToCard());
                    this.TrumpCard = card;
                    player.Cards.Remove(card);
                    player.Messages = "Change Trump card!!!";
                }
            }
            else
            {
                this.BoardMessage = "Cannot Change The Trump card!!!";
            }
        }

        /*
         * TODO 
         *      close trumpCard - implement the method
         */
        private void HandleCloseCommand(object parameter)
        {

        }

        private static void HandleCallingAnnounce(PlayerViewModel player, Deck deck)
        {
            var card = player.SelectedCard;
            if (card != null)
            {
                if (SixtySixUtil.HasForty(player.ToPlayer().Cards, card.ToCard(), deck))
                {
                    player.Score += Constants.FORTY_ANNOUNCEMENT;
                    player.Messages = "Forty!!!";
                }
                else if (SixtySixUtil.HasTwenty(player.ToPlayer().Cards, card.ToCard(), deck))
                {
                    player.Score += Constants.TWENTY_ANNOUNCEMENT;
                    player.Messages = "Twenty!!!";
                }
            }
        }

        private bool HandleEndOfDeal(PlayerViewModel player, PlayerViewModel opponent, Deck deck)
        {
            if (player.HasWonLastHand && player.Score >= Constants.TOTAL_SCORE) {
                deck.IsEndOfGame = true;
                
                var enginePlayer = player.ToPlayer();
                var engineOpponent = opponent.ToPlayer();
                CardsDeckUtil.CollectCardsInDeck(deck, enginePlayer, engineOpponent);

                player.Cards = new ObservableCollection<CardViewModel>();

                player.WinsCount += SixtySixUtil.GetNumberOfWins(opponent.ToPlayer());
                player.Score = 0;
                opponent.Score = 0;
                this.TrumpCard = null;
                player.SelectedCard = null;
                opponent.SelectedCard = null;
                
                player.HasWonLastDeal = true;
                opponent.HasWonLastDeal = false;
                player.ThrownCards = new ObservableCollection<CardViewModel>();
                opponent.Cards = new ObservableCollection<CardViewModel>();
                opponent.ThrownCards = new ObservableCollection<CardViewModel>();


                if (enginePlayer.HasWonLastDeal)
                {
                    var splitIndex = AIMovementUtil.GetDeckSplittingIndex();
                    CardsDeckUtil.SplitDeck(deck, splitIndex); //one of the players should split the deck
                    SixtySixUtil.DealCards(deck, engineOpponent, enginePlayer);
                }
                else if (engineOpponent.HasWonLastDeal)
                {
                    //TODO Get User Input
                    var splitIndex = 10; //MovementUtil.GetDeckSplittingIndex(engineOpпonent);
                    CardsDeckUtil.SplitDeck(deck, splitIndex); //one of the players should split the deck
                    SixtySixUtil.DealCards(deck, enginePlayer, engineOpponent);
                }

                player = PlayerViewModel.ConvertToPlayerViewModel(enginePlayer);
                opponent = PlayerViewModel.ConvertToPlayerViewModel(engineOpponent);

                player.Messages = "WIN";
                opponent.Messages = "LOSE";

                return true;
            }
            return false;
        }

        public void asd()
        {
            this.Player.SelectedCard = null;
            this.Opponent.SelectedCard = null;
        }
        #endregion

    }
}

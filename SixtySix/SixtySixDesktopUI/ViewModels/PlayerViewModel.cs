using SixtySix;
using SixtySix.enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySixDesktopUI.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        private CardViewModel selectedCard;
        private ObservableCollection<CardViewModel> cards;
        private int score;

        public ObservableCollection<CardViewModel> Cards
        {
            get { return this.cards; }
            set{
                this.cards = value;
                this.OnPropertyChanged("cards");
            }
        }
        public int Score {
            get { return score; }
            set
            {
                this.score = value;
                this.OnPropertyChanged("score");
            } 
        }
        public int WinsCount { get; set; }
        public bool HasWonLastDeal { get; set; }
        public bool HasWonLastHand { get; set; }
        public bool IsAIPlayer { get; set; }
        public PlayStrategy PlayStrategy { get; set; }
        public ObservableCollection<CardViewModel> ThrownCards { get; set; }
        public CardViewModel SelectedCard
        {
            get
            {
                return selectedCard;
            }
            set
            {
                this.selectedCard = value;
                this.OnPropertyChanged("SelectedCard");
            }
        }

        private string messages;
        public string Messages
        {
            get { return this.messages; }
            set
            {
                this.messages = value;
                this.OnPropertyChanged("Messages");
            }
        }

        public PlayerViewModel(bool isAIPlayer=false)
        {
            Cards = new ObservableCollection<CardViewModel>();
            ThrownCards = new ObservableCollection<CardViewModel>();
            IsAIPlayer = isAIPlayer;
        }

        public PlayerViewModel(bool isAIPlayer, PlayStrategy playStrategy)
             : this(isAIPlayer)
        {
            PlayStrategy = playStrategy;
        }

        public Player ToPlayer()
        {
            return new Player()
            {
                Cards = CardViewModel.ConvertListOfCardViewModelsToListOFCard(this.Cards),
                Score = this.Score,
                WinsCount = this.WinsCount,
                HasWonLastDeal = this.HasWonLastDeal,
                HasWonLastHand = this.HasWonLastHand,
                IsAIPlayer = this.IsAIPlayer,
                PlayStrategy = this.PlayStrategy,
                ThrownCards = CardViewModel.ConvertListOfCardViewModelsToListOFCard(this.ThrownCards)
            };
        }

        public static PlayerViewModel ConvertToPlayerViewModel(Player player)
        {
            return new PlayerViewModel()
            {
                Cards = CardViewModel.ConvertListOfCardsToListOfCardViewModels(player.Cards),
                Score = player.Score,
                WinsCount = player.WinsCount,
                HasWonLastDeal = player.HasWonLastDeal,
                HasWonLastHand = player.HasWonLastHand,
                IsAIPlayer = player.IsAIPlayer,
                PlayStrategy = player.PlayStrategy,
                ThrownCards = CardViewModel.ConvertListOfCardsToListOfCardViewModels(player.ThrownCards)
            };
        }
    }
}
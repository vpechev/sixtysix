using SixtySix;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySixDesktopUI.ViewModels
{
    public class CardViewModel
    {
        public CardValue Value { get; set; }
        public CardSuit Suit { get; set; }
        public string CardImageSrc
        {
            get
            {
                return @"..\CardImageFiles\PNG-cards-1.3\" + this.ToString() + ".png".ToLower();
            }
        }
        public string CardBackImageSrc
        {
            get { return @"..\CardImageFiles\PNG-cards-1.3\red_joker.png"; }
        }

        public override string ToString()
        {
            return String.Format("{0}_of_{1}s", Value.ToString(), Suit.ToString());
        }

        public Card ToCard(){
            return new Card(){
                Value = this.Value,
                Suit = this.Suit
            };
        }

        public static CardViewModel ConvertToCardViewModel(Card card)
        {
            if (card == null)
                return null;

            return new CardViewModel()
            {
                Value = card.Value,
                Suit = card.Suit
            };
        }

        public static List<Card> ConvertListOfCardViewModelsToListOFCard(ObservableCollection<CardViewModel> cardsViewModels)
        {
            var cards = new List<Card>();
            foreach (var card in cardsViewModels)
            {
                if (card != null)
                {
                    cards.Add(card.ToCard());
                }
            }
            return cards;
        }

        public static ObservableCollection<CardViewModel> ConvertListOfCardsToListOfCardViewModels(List<Card> cards)
        {
            var cardsViewModels = new ObservableCollection<CardViewModel>();
            foreach (var card in cards)
            {
                cardsViewModels.Add(CardViewModel.ConvertToCardViewModel(card));
            }
            return cardsViewModels;
        }

        public override bool Equals(object obj)
        {
            var otherCard = obj as CardViewModel;

            if (otherCard == null)
            {
                return false;
            }

            return this.Suit.Equals(otherCard.Suit) && this.Value.Equals(otherCard.Value);
        }

        public override int GetHashCode()
        {
            var primeNumber = 31;
            return primeNumber * this.Suit.GetHashCode() * this.Value.GetHashCode();
        }
    }
}

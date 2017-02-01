using SixtySix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtySixDesktopUI.ViewModels
{
    public class CardViewModel
    {
        private string _cardImageSrc;
        public CardValue CardValue { get; set; }
        public CardSuit CardSuit { get; set; }
        public string CardImageSrc { 
            get {
                _cardImageSrc = @"..\CardImageFiles\PNG-cards-1.3\ace_of_spades.png"; 
                                //+ this.CardValue.ToString() 
                                //+ "_of_" 
                                //+ this.CardSuit 
                                //+ "s.png";
                _cardImageSrc = _cardImageSrc.ToLower();
                return _cardImageSrc;
            }
            set { _cardImageSrc = value; }
        }
    }
}

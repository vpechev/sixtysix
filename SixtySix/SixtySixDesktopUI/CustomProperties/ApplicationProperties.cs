using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SixtySixDesktopUI.CustomProperties
{
    public static class ApplicationProperties
    {
        #region Dependency Properties Declarations
        public static readonly DependencyProperty MarginRightProperty = DependencyProperty.RegisterAttached(
           "MarginRight",
           typeof(string),
           typeof(ApplicationProperties),
           new UIPropertyMetadata(OnMarginRightPropertyChanged));

        public static readonly DependencyProperty MarginLeftProperty = DependencyProperty.RegisterAttached(
           "MarginLeft",
           typeof(string),
           typeof(ApplicationProperties),
           new UIPropertyMetadata(OnMarginLeftPropertyChanged));
        #endregion

        #region Public Getters and Setters
        public static string GetMarginRight(FrameworkElement element)
        {
            return (string)element.GetValue(MarginRightProperty);
        }

        public static void SetMarginRight(FrameworkElement element, string value)
        {
            element.SetValue(MarginRightProperty, value);
        }

        public static string GetMarginLeft(FrameworkElement element)
        {
            return (string)element.GetValue(MarginLeftProperty);
        }

        public static void SetMarginLeft(FrameworkElement element, string value)
        {
            element.SetValue(MarginLeftProperty, value);
        }
        #endregion

        #region Private Event Handler
        private static void OnMarginRightPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var element = obj as FrameworkElement;

            if (element != null)
            {
                int value;
                if (Int32.TryParse((string)args.NewValue, out value))
                {
                    var margin = element.Margin;
                    margin.Right = value;
                    element.Margin = margin;
                }
            }
        }

        private static void OnMarginLeftPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var element = obj as FrameworkElement;

            if (element != null)
            {
                int value;
                if (Int32.TryParse((string)args.NewValue, out value))
                {
                    var margin = element.Margin;
                    margin.Left = value;
                    element.Margin = margin;
                }
            }
        }
        #endregion
    }
}

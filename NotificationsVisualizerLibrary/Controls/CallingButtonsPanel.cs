using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NotificationsVisualizerLibrary.Controls
{
    internal class CallingButtonsPanel : Panel
    {
        private static readonly DependencyProperty ColumnSpacingProperty =
            DependencyProperty.Register(
                nameof(ColumnSpacing),
                typeof(double),
                typeof(CallingButtonsPanel),
                new PropertyMetadata(12.0, OnDisplayPropertyChanged));

        public double ColumnSpacing
        {
            get => (double)GetValue(ColumnSpacingProperty);
            set => SetValue(ColumnSpacingProperty, value);
        }

        private static void OnDisplayPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CallingButtonsPanel).OnDisplayPropertyChanged(e);
        }

        private void OnDisplayPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        private static readonly DependencyProperty ButtonSizeProperty =
            DependencyProperty.Register(
                nameof(ButtonSize),
                typeof(double),
                typeof(CallingButtonsPanel),
                new PropertyMetadata(72.0, OnDisplayPropertyChanged));

        public double ButtonSize
        {
            get => (double)GetValue(ButtonSizeProperty);
            set => SetValue(ButtonSizeProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size childAvailableSize = new Size(double.PositiveInfinity, ButtonSize);

            CallingButton answerButton = GetVisibleCallingButtons().LastOrDefault();

            if (answerButton != null)
            {
                answerButton.IsAnswerButton = true;
            }

            bool hasOtherButtons = false;

            foreach (CallingButton child in GetVisibleCallingButtons())
            {
                if (child == answerButton)
                {
                    child.Measure(new Size(availableSize.Width, ButtonSize));
                }
                else
                {
                    hasOtherButtons = true;

                    child.IsAnswerButton = false;

                    child.Measure(childAvailableSize);
                }
            }

            double finalHeight;

            if (hasOtherButtons)
            {
                finalHeight = (ButtonSize * 2) + 24; // 24 px spacing between normal buttons and answer button
            }
            else
            {
                finalHeight = answerButton != null ? ButtonSize : 0;
            }

            return new Size(
                availableSize.Width,
                finalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            CallingButton answerButton = GetVisibleCallingButtons().LastOrDefault();

            int countOfSecondaryButtons = Math.Max(GetVisibleCallingButtons().Count() - 1, 0);

            double secondaryX = 0;

            if (countOfSecondaryButtons > 0)
            {
                double secondaryButtonsTotalWidth = (countOfSecondaryButtons * ButtonSize) + ((countOfSecondaryButtons - 1) * ColumnSpacing);

                // Calculate starting point
                secondaryX = (finalSize.Width - secondaryButtonsTotalWidth) / 2;
            }

            bool hasOtherButtons = false;

            int i = 0;
            foreach (CallingButton child in GetVisibleCallingButtons())
            {
                if (child == answerButton)
                {
                    child.Arrange(new Rect(
                        x: 0,
                        y: hasOtherButtons ? ButtonSize + 24 : 0,
                        width: finalSize.Width,
                        height: ButtonSize));
                }
                else
                {
                    hasOtherButtons = true;

                    child.Arrange(new Rect(
                        new Point(secondaryX, 0),
                        child.DesiredSize));

                    secondaryX += ButtonSize + ColumnSpacing;

                    i++;
                }
            }

            return finalSize;
        }

        private static bool CountsForDisplay(UIElement el)
        {
            return el.Visibility == Visibility.Visible;
        }

        //private double GetWidthOfColumns(Size totalSize)
        //{
        //    return (totalSize.Width - GetTotalColumnSpacing()) / GetNumberOfColumns();
        //}

        //private double GetTotalColumnSpacing()
        //{
        //    int numOfCols = GetNumberOfColumns();

        //    return numOfCols <= 1 ? 0 : ColumnSpacing / (numOfCols - 1);
        //}

        //private int GetNumberOfColumns()
        //{
        //    // Subtract one since the last button is always the "answer" button
        //    return Math.Max(GetVisibleCallingButtons().Count() - 1, 0);
        //}

        private IEnumerable<CallingButton> GetVisibleCallingButtons()
        {
            return Children.OfType<CallingButton>().Where(i => CountsForDisplay(i));
        }
    }
}

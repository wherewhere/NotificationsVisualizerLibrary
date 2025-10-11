using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace NotificationsVisualizerLibrary
{
    public sealed partial class PreviewTileVisualElements : INotifyPropertyChanged
    {
        public Color BackgroundColor
        {
            get => field;
            set => SetProperty(ref field, value);
        } = Colors.Blue;

        public bool ShowNameOnSquare150x150Logo
        {
            get;
            set => SetProperty(ref field, value);
        } = false;

        public bool ShowNameOnSquare310x310Logo
        {
            get;
            set => SetProperty(ref field, value);
        } = true;

        public bool ShowNameOnWide310x150Logo
        {
            get;
            set => SetProperty(ref field, value);
        } = true;

        public Uri? Square150x150Logo
        {
            get;
            set => SetProperty(ref field, value);
        }

        public Uri? Square310x310Logo
        {
            get;
            set => SetProperty(ref field, value);
        }

        public Uri? Square44x44Logo
        {
            get;
            set => SetProperty(ref field, value);
        }

        public Uri? Square71x71Logo
        {
            get;
            set => SetProperty(ref field, value);
        }

        public Uri? Wide310x150Logo
        {
            get;
            set => SetProperty(ref field, value);
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// From http://danrigby.com/2012/04/01/inotifypropertychanged-the-net-4-5-way-revisited/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;

            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        //private void Blah()
        //{
        //    SecondaryTileVisualElements sample = new Windows.UI.StartScreen.SecondaryTile().VisualElements;

        //    sample.BackgroundColor = Colors.Red;

        //    sample.ShowNameOnSquare150x150Logo = true;
        //    sample.ShowNameOnSquare310x310Logo = true;
        //    sample.ShowNameOnWide310x150Logo = true;

        //    sample.Square150x150Logo = new Uri("");
        //    sample.Square310x310Logo = new Uri("");
        //    sample.Square71x71Logo = new Uri("");
        //    sample.Wide310x150Logo = new Uri("");
        //}
    }
}

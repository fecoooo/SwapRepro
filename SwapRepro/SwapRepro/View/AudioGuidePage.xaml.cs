using AudioGuideModule.ViewHolder;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace AudioGuideModule.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AudioGuidePage : ContentPage
    {
        private AudioGuidePageVM viewHolder;
        const double AudioStep = 10.0;

        public AudioGuidePage()
        {
            viewHolder = new AudioGuidePageVM();
            BindingContext = viewHolder;

            InitializeComponent();
        }

        private void StartScanQRCode(object sender, EventArgs e)
        {
            ScanQRCode();
        }

        public async void ScanQRCode()
        {
            ZXingScannerPage ScannerPage = viewHolder.Scan();

            ScannerPage.OnScanResult += (result) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };

            await Navigation.PushAsync(ScannerPage);
        }

        private void PlayOrPause(object sender, EventArgs e)
        {
            viewHolder.PlayOrPause();     
        }

        private void FastBackward(object sender, EventArgs e)
        {
            viewHolder.SetCurrentAudioPosition(viewHolder.CurrentAudioPostion - AudioStep);
        }

        private void FastForward(object sender, EventArgs e)
        {
            viewHolder.SetCurrentAudioPosition(viewHolder.CurrentAudioPostion + AudioStep);
        }

        private void ProgressSlider_DragCompleted(object sender, EventArgs e)
        {
            viewHolder.SetCurrentAudioPosition(ProgressSlider.Value);
        }

        public void SetPlayAndPauseBtnVisibility()
        {
        }

        private void PreviousTrack(object sender, EventArgs e)
        {
            viewHolder.PreviousTrack();
        }

        private void NextTrack(object sender, EventArgs e)
        {
            viewHolder.NextTrack();
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class DoubleSecondsToMMSSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = (int)((double)value); //don't even ask....
            return new TimeSpan(0, 0, 0, intValue).ToString(@"mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
using AudioGuideModule.View;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using ZXing.Net.Mobile.Forms;

namespace AudioGuideModule.ViewHolder
{
    class AudioGuidePageVM : INotifyPropertyChanged
    {
        private string _scanResult =  "sample.mp3";
        public string ScanResult {
            get => _scanResult;
            set 
            {
                _scanResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScanResult"));
            }
        }
        
        public double CurrentAudioPostion
        {
            get => player.CurrentPosition;
        }

        public bool IsAudioPlaying
        {
            get => player.IsPlaying;
        }


        readonly FileImageSource PlayImgSource  = (FileImageSource)ImageSource.FromFile("play.png");
        readonly FileImageSource PauseImgSource = (FileImageSource)ImageSource.FromFile("pause.png");
        
        public FileImageSource PlayPauseBtnImageSource
        { 
            get => player.IsPlaying ? PauseImgSource : PlayImgSource; 
        } 


        //!!!!!!!!!!!!!!!!!!!!!!!!  Slider: If Minimum or Maximum are ever set so that Minimum is not less than Maximum, an exception is raised. !!!!!!!!!!!!!!!!!!!!!!!!
        //!!!!!!!!!!!!!!!!!!!!!!!!  this property is set as a Maximum property of a slider, so cant be 0 !!!!!!!!!!!!!!!!!!!!!!!!
        public double AudioDuration
        {
            get => player.Duration <= 0.001 ? 0.001 : player.Duration;
        }

        private Timer DuringAudioPlayTimer;
        private const double Interval_duringAudioplayTimer = 200;

        ISimpleAudioPlayer player;

        List<string> playList = new List<string>();
        int playListIndex = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public AudioGuidePageVM()
        {
            player = CrossSimpleAudioPlayer.Current;

            LoadPlayList("Dummy");
            //TODO: what happens if we first open the page, should the controls be disabled or load a file defaultly
            //now: load a file defaultly
            if (player.Duration <= 0)
                LoadAudio(playList[playListIndex]);

            DuringAudioPlayTimer = new Timer(Interval_duringAudioplayTimer);
            DuringAudioPlayTimer.Elapsed += JobDuringAudioPlay;
            DuringAudioPlayTimer.Start();

            void Handler(object sender, EventArgs e)
            {
                SetCurrentAudioPosition(0);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAudioPlaying"));
            }

            player.PlaybackEnded += Handler;
        }

        public ZXingScannerPage Scan()
        {
            ZXingScannerPage scannerPage = new ZXingScannerPage();

            scannerPage.OnScanResult += (result) => {
                // stop scannin
                scannerPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() => {
                    LoadAudio("sample.mp3");
                });
            };

            return scannerPage;
        }

        public void LoadAudio(string path)
        {
            ScanResult = path;
            player.Load(path);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentAudioPostion"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AudioDuration"));
        }

        public void PlayOrPause()
        {
            if (player.IsPlaying)
                player.Pause();
            else
                player.Play();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAudioPlaying"));
        }

        private void JobDuringAudioPlay(Object source, ElapsedEventArgs e)
        {
            if (player.IsPlaying)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentAudioPostion"));
        }

        public void SetCurrentAudioPosition(double value)
        {
            player.Seek(value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentAudioPostion"));
        }

        public void LoadPlayList(string playListName)
        {
            //TODO: load this from actual database
            playList.Clear();
            playList.Add("Samples/funny.mp3");
            playList.Add("Samples/sample.mp3");
            playList.Add("Samples/edm.mp3");
        }

        public void NextTrack()
        {
            bool wasPlaying = player.IsPlaying;

            playListIndex = (playListIndex + 1) % playList.Count;

            LoadAudio(playList[playListIndex]);

            if (wasPlaying)
                PlayOrPause();
        }

        public void PreviousTrack()
        {
            bool wasPlaying = player.IsPlaying;

            playListIndex = playListIndex - 1;
            if (playListIndex < 0)
                playListIndex = playList.Count - 1;

            LoadAudio(playList[playListIndex]);

            if (wasPlaying)
                PlayOrPause();
        }
    }
}

using AudioGuideModule.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SwapRepro
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AudioGuidePage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

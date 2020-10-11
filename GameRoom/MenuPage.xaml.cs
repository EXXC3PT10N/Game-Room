using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GameRoom.Games.Tetris;
using GameRoom.Games.TicTacToe;
using Windows.ApplicationModel.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameRoom
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuPage : Page
    {
        public MenuPage()
        {
            this.InitializeComponent();
        }

        private void BtnTetris_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Tetris_home_frame));
        }

        private void BtnTicTacToe_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TicTacToe_frame));
        }

        private void BtnAboutApp_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutAppFrame));
        }

        private void BtnAboutMe_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutMeFrame));
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }
    }
}

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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameRoom.Games.Tetris
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Tetris_home_frame : Page
    {
        public Tetris_home_frame()
        {
            this.InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Tetris_game_frame));
        }

        private void RankingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ManualButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Tetri_manual_frame));

        }
    }
}

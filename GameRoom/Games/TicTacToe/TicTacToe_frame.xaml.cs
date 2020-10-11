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

namespace GameRoom.Games.TicTacToe
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TicTacToe_frame : Page
    {
        public Game game = new Game();

        public TicTacToe_frame()
        {
            this.InitializeComponent();
            game.SetRuchBlock(RuchBlock);
            game.SetBoardGrid(BoardGrid);
            game.Start();
            
            

        }
        
        public class Game
        {
            private bool kolejka = false;
            private Grid BoardGrid;
            private TextBlock RuchBlock;
            private List<Button> Circles = new List<Button>();
            private List<Point> CirclesPoints = new List<Point>();
            private List<Button> Crosses = new List<Button>();
            private List<Point> CrossesPoints = new List<Point>();

           

            public Game()
            {
                
            }

            //Metoda startująca. Ustawia wszystkie przyciski na aktywne oraz ustawia napis określający kolejkę 
            public void Start()
            {
                foreach (Button b  in BoardGrid.Children)
                {
                    b.IsEnabled = true;
                }
                ChangeRuchBlock();
            }

            //Metoda ustawiająca obiekt typu Grid
            public void SetBoardGrid(Grid BoardGrid)
            {
                this.BoardGrid = BoardGrid;
            }

            //Metoda ustawiająca obiekt typu TextBlock
            public void SetRuchBlock(TextBlock RuchBlock)
            {
                this.RuchBlock = RuchBlock;
            }

            
            //Metoda zmieniajaca wartosc zmiennej kolejka
            private void ChangeKolejka()
            {
                if (this.kolejka)
                {
                    this.kolejka = false;
                }
                else
                {
                    this.kolejka = true;
                }
            }

            //Metoda zmieniajaca napis w zaleznosci od kolejki
            private void ChangeRuchBlock()
            {
                if (kolejka)
                    RuchBlock.Text = "Ruch: X";
                else RuchBlock.Text = "Ruch: O";
            }

            //Metoda wywolywana w przypadku klikniecia w przycisk
            public void MakeMove(Button b)
            {
                
                b.IsEnabled = false;

                //w zaleznosci od kolejki ustawia tresc przycisku na X lub O
                //nastepnie sprawdza czy dany ruch konczy gre (wygrana/remis)
                
                if (kolejka)
                {
                    b.Content = "X";
                    Crosses.Add(b);
                    CrossesPoints.Add(new Point(Grid.GetColumn(b), Grid.GetRow(b)));
                    
                    if (IsGameOver(CrossesPoints))
                    {
                        RuchBlock.Text = "Wygrały: X";
                        foreach (Button button in BoardGrid.Children)
                        {
                            button.IsEnabled = false;
                        }
                    }
                    else if (IsDraw())
                    {
                        RuchBlock.Text = "Remis";
                    }
                    else
                    {
                        
                        ChangeKolejka();
                        ChangeRuchBlock();
                    }
                }
                else
                {
                    b.Content = "O";
                    Circles.Add(b);
                    CirclesPoints.Add(new Point(Grid.GetColumn(b), Grid.GetRow(b)));
                    if (IsGameOver(CirclesPoints))
                    {
                        RuchBlock.Text = "Wygrały: O";
                        foreach (Button button in BoardGrid.Children)
                        {
                            button.IsEnabled = false;
                        }
                    }
                    else if (IsDraw())
                    {
                        RuchBlock.Text = "Remis";
                    }
                    else
                    {
                        
                        ChangeKolejka();
                        ChangeRuchBlock();
                    }
                        
                }
                
                
               
            }

            //Metoda sprawdzajaca czy uzytkownik wygral gre
            private bool IsGameOver(List<Point> points)
            {
                //W tej metodzie zliczane sa koordynaty
                //Jesli znajda sie 3 punkty nalezace do jednej kolumny lub wiersza, albo 3 punkty na skos, metoda zwroci wartosc true
                int[] countColumn = new int[3];
                int[] countRow = new int[3];
                int[] countCross = new int[2];
                foreach (var point in points)
                {
                    countColumn[(int) point.X]++;
                    countRow[(int)point.Y]++;
                    if((point.X == 0 && point.Y == 0) || (point.X == 1 && point.Y == 1) || (point.X == 2 && point.Y == 2))
                    {
                        countCross[0]++;
                    }
                    if ((point.X == 0 && point.Y == 2) || (point.X == 1 && point.Y == 1) || (point.X == 2 && point.Y == 0))
                    {
                        countCross[1]++;
                    }
                }
                foreach (var c in countColumn)
                {
                    if (c == 3)
                        return true;
                }
                foreach (var c in countRow)
                {
                    if (c == 3)
                        return true;
                }
                foreach (var c in countCross)
                {
                    if (c == 3)
                        return true;
                }

                return false;
            }

            //Metoda sprawdzajaca czy doszlo do remisu
            private bool IsDraw()
            {
                //Jesli wszystkie 9 przyciskow beda nieaktywne, metoda zwroci wartosc true
                int countDisabled = 0;
                foreach (Button b in BoardGrid.Children)
                {
                    if (!b.IsEnabled)
                        countDisabled++;
                }
                
                if (countDisabled == 9)
                {
                    return true;
                }
                else return false;
            }
           
        }

        //Zdarzenie wcisniecia przycisku
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Wywoluje metode MakeMove objektu "game" i przekazuje sam siebie (Button) w argumencie
            this.game.MakeMove(sender as Button);
        }

        
    }
}

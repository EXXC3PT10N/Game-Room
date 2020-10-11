using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using DataAccessLibrary;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameRoom.Games.Tetris
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Tetris_game_frame : Page
    {
        private Tetris tetris = new Tetris();
        private bool _isSwiped;

        public Tetris_game_frame()
        {
            this.InitializeComponent();
            
            DataAccess.InitializeDatabase();
            UpdateRank();
              
        }

        //Metoda pobierajaca pobierajaca i wypisujaca aktualne rekordy z bazy danych
        public void UpdateRank()
        {
            RecordsBlock.Text = "Rekordy: ";
            List<string> wyniki = new List<string>();
            wyniki = DataAccess.GetData();
            for (int i = 0; i < wyniki.Count; i++)
            {
                if (i > 9)
                    break;
                else
                {
                    RecordsBlock.Text += "\n" + (i + 1) + ". " + wyniki[i];

                }
            }
        }
        
        //Klasa odpowiadajaca za pojedynczy blok
        public class Tetrimino
        {
            private char type;
            private Point[] coord;
            private Rectangle[] r;
            private readonly int size = 40;
            private SolidColorBrush color;
            private Grid g;
            private Timer timer;
            private int time;
            private bool isMoving = true;
            private bool isGameOver = false;
            public event EventHandler isMovingChanged;
            public event EventHandler isGameOverChanged;
            public List<Point> blockedCoord;
            public bool IsMoving
            {
                get { return isMoving; }
                set
                {
                    isMoving = value;
                    OnIsMovingChange();
                }
            }
            public bool IsGameOver
            {
                get { return isGameOver; }
                set
                {
                    isGameOver = value;
                    OnIsGameOverChange();
                }
            }

            //Przypisanie poszczegolnych zmiennych w konstruktorze, stworzenie bloku i nadanie mu "grawitacji"
            public Tetrimino(char type, Grid g, int time, List<Point> blockedCoord)
            {
                this.type = type;
                this.g = g;
                this.time = time;
                this.blockedCoord = blockedCoord;

                CreateTetrimino();
                Gravity();     
            }

            //Metoda zwracajaca zmienna r
            public Rectangle[] GetRectangles()
            {
                return this.r;
            }

            //Metoda zwracajaca zmienna coord
            public Point[] GetCoord()
            {
                return this.coord;
            }

            //Metoda tworzaca blok
            private async void CreateTetrimino()
            {
                

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    r = new Rectangle[4];
                    
                    coord = new Point[4];
                    for (int i = 0; i < 4; i++)
                    {
                        r[i] = new Rectangle();
                        r[i].Width = this.size;
                        r[i].Height = this.size;
                      
                        
                        switch (type)
                        {
                            case 'I':
                                coord[i] = new Point(i + 3, 1);
                                color = new SolidColorBrush(Windows.UI.Colors.Crimson);
                                r[i].Fill = color;
                                r[i].Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                                g.Children.Add(r[i]);
                                
                                break;
                            case 'T':
                                if (i != 3)
                                    coord[i] = new Point(i + 3, 0);
                                else coord[i] = new Point(4, 1);
                                color = new SolidColorBrush(Windows.UI.Colors.DarkSlateBlue);
                                r[i].Fill = color;
                                r[i].Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                                g.Children.Add(r[i]);
                                
                                break;
                            case 'O':
                                if (i < 2)
                                    coord[i] = new Point(i + 3, 0);
                                else coord[i] = new Point(i + 1, 1);
                                color = new SolidColorBrush(Windows.UI.Colors.LightBlue);
                                r[i].Fill = color;
                                r[i].Stroke = new SolidColorBrush(Windows.UI.Colors.DarkCyan);
                                g.Children.Add(r[i]);
                                
                                break;
                            case 'L':
                                if (i < 3)
                                    coord[i] = new Point(5, i);
                                else coord[i] = new Point(6, i - 1);
                                color = new SolidColorBrush(Windows.UI.Colors.Goldenrod);
                                r[i].Fill = color;
                                r[i].Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                                g.Children.Add(r[i]);
                                
                                break;
                            case 'J':
                                if (i < 3)
                                    coord[i] = new Point(5, i);
                                else coord[i] = new Point(4, i - 1);
                                color = new SolidColorBrush(Windows.UI.Colors.HotPink);
                                r[i].Fill = color;
                                r[i].Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                                g.Children.Add(r[i]);
                                
                                break;
                            case 'S':
                                if (i < 2)
                                    coord[i] = new Point(i + 3, 0);
                                else coord[i] = new Point(i, 1);
                                color = new SolidColorBrush(Windows.UI.Colors.Navy);
                                r[i].Fill = color;
                                r[i].Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                                g.Children.Add(r[i]);
                                
                                break;
                            case 'Z':
                                if (i < 2)
                                    coord[i] = new Point(i + 3, 0);
                                else coord[i] = new Point(i + 2, 1);
                                color = new SolidColorBrush(Windows.UI.Colors.YellowGreen);
                                r[i].Fill = color;
                                r[i].Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                                g.Children.Add(r[i]);

                                
                                break;
                        }
                        if (blockedCoord.Contains(coord[i]))
                            IsGameOver = true;
                            
                           
                    }
                    UpdateCoord();
                    
                });
            }

            //Metoda dodajaca grawitacje do bloku (wywoluje metode MoveDown() co okreslony czas
            private async void Gravity()
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    timer = new Timer(new TimerCallback(_ => MoveDown()), null, time, time);
                });

            }

            //Metoda przemieszczajaca tetrimino o jeden blok w dol
            public void MoveDown()
            {
                
                if (!IsColliding(coord[0]) && !IsColliding(coord[1]) && !IsColliding(coord[2]) && !IsColliding(coord[3]))
                {
                    for (int i = 0; i < 4; i++)
                    {

                        coord[i].Y++;

                    }
                    if (!IsColliding(coord[0]) && !IsColliding(coord[1]) && !IsColliding(coord[2]) && !IsColliding(coord[3]))
                        UpdateCoord();
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {

                            coord[i].Y--;

                        }
                        IsMoving = false;
                        timer.Dispose();
                    }

                }
                else
                {

                    IsMoving = false;
                    timer.Dispose();

                }
            }

            //Metoda ktora przemieszcza tetrimino na sam dol 
            public void MoveToGround()
            {
                
                while (isMoving)
                {
                    MoveDown();
                }
            }

            //Metoda przemieszczajaca tetrimino w lewo
            public void MoveLeft()
            {
                if (coord[0].X > 0 && coord[1].X > 0 && coord[2].X > 0 && coord[3].X > 0)
                {
                    for (int i = 0; i < 4; i++)
                    {

                        coord[i].X--;

                    }
                    if (!blockedCoord.Contains(coord[0]) && !blockedCoord.Contains(coord[1]) && !blockedCoord.Contains(coord[2]) && !blockedCoord.Contains(coord[3]))
                        UpdateCoord();
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {

                            coord[i].X++;

                        }   
                    }
                }  
            }

            //Metoda przemieszczajaca tetrimino w prawo
            public void MoveRight()
            {
                if (coord[0].X < 9 && coord[1].X < 9 && coord[2].X < 9 && coord[3].X < 9)
                {
                    for (int i = 0; i < 4; i++)
                    {

                        coord[i].X++;

                    }
                    if (!blockedCoord.Contains(coord[0]) && !blockedCoord.Contains(coord[1]) && !blockedCoord.Contains(coord[2]) && !blockedCoord.Contains(coord[3]))
                        UpdateCoord();
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {

                            coord[i].X--;

                        }
                    }
                }
            }

            //Metoda obracajaca tetrimino
            public void Rotate()
            {
                Point[] tmpCoord = new Point[4];
                
                for(int i = 0; i < 4; i++)
                {
                    tmpCoord[i] = new Point(coord[i].X, coord[i].Y);
                }
                
                switch (type)
                {
                    case 'I' when coord[0].Y == coord[1].Y:
                        for(int i = 0; i < 4; i++)
                        {
                                coord[i].X = coord[0].X;
                                coord[i].Y = coord[0].Y + i;
                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }    
                            
                        }
                        break;
                    case 'I' when coord[0].X == coord[1].X:
                        for (int i = 0; i < 4; i++)
                        {
                            
                            coord[i].X = coord[0].X + i;
                            coord[i].Y = coord[0].Y;
                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'T' when coord[0].Y == coord[1].Y && coord[0].Y == coord[2].Y && coord[3].Y > coord[2].Y:
                        for (int i = 0; i < 4; i++)
                        {
                            if(i < 3)
                            {
                                coord[i].X = tmpCoord[1].X;
                                coord[i].Y = coord[i].Y - i + 1;
                            }
                            else
                            {
                                coord[i].X++;
                                coord[i].Y--;
                            }
                            
                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'T' when coord[0].X == coord[1].X && coord[0].X == coord[2].X && coord[3].X > coord[2].X:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[i].X - i + 1;
                                coord[i].Y = tmpCoord[1].Y;
                            }
                            else
                            {
                                coord[i].X--;
                                coord[i].Y--;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'T' when coord[0].Y == coord[1].Y && coord[0].Y == coord[2].Y && coord[3].Y < coord[2].Y:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[1].X;
                                coord[i].Y = tmpCoord[1].Y - i + 1;
                            }
                            else
                            {
                                coord[i].X--;
                                coord[i].Y++;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'T' when coord[0].X == coord[1].X && coord[0].X == coord[2].X && coord[3].X < coord[2].X:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[1].X + i - 1;
                                coord[i].Y = tmpCoord[1].Y;
                            }
                            else
                            {
                                coord[i].X++;
                                coord[i].Y++;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'L' when coord[0].X == coord[1].X && coord[0].X == coord[2].X && coord[3].X > coord[2].X:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[1].X + i - 1;
                                coord[i].Y = tmpCoord[1].Y;
                            }
                            else
                            {
                                coord[i].Y -= 2;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'L' when coord[0].Y == coord[1].Y && coord[0].Y == coord[2].Y && coord[3].Y < coord[2].Y:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[1].X;
                                coord[i].Y = coord[i].Y - i + 1;
                            }
                            else
                            {
                                coord[i].X -= 2;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'L' when coord[0].X == coord[1].X && coord[0].X == coord[2].X && coord[3].X < coord[2].X:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[i].X - i + 1;
                                coord[i].Y = tmpCoord[1].Y;
                            }
                            else
                            {
                                coord[i].Y += 2;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'L' when coord[0].Y == coord[1].Y && coord[0].Y == coord[2].Y && coord[3].Y > coord[2].Y:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[1].X;
                                coord[i].Y = tmpCoord[1].Y - i + 1;
                            }
                            else
                            {
                                coord[i].X += 2;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'J' when coord[0].X == coord[1].X && coord[0].X == coord[2].X && coord[3].X < coord[2].X:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[1].X + i - 1;
                                coord[i].Y = tmpCoord[1].Y;
                            }
                            else
                            {
                                coord[i].X += 2;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'J' when coord[0].Y == coord[1].Y && coord[0].Y == coord[2].Y && coord[3].Y > coord[2].Y:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[1].X;
                                coord[i].Y = coord[i].Y - i + 1;
                            }
                            else
                            {
                                coord[i].Y -= 2;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'J' when coord[0].X == coord[1].X && coord[0].X == coord[2].X && coord[3].X > coord[2].X:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[i].X - i + 1;
                                coord[i].Y = tmpCoord[1].Y;
                            }
                            else
                            {
                                coord[i].X -= 2;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'J' when coord[0].Y == coord[1].Y && coord[0].Y == coord[2].Y && coord[3].Y < coord[2].Y:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 3)
                            {
                                coord[i].X = tmpCoord[1].X;
                                coord[i].Y = tmpCoord[1].Y - i + 1;
                            }
                            else
                            {
                                coord[i].Y += 2;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'S' when coord[0].Y == coord[1].Y && coord[0].Y < coord[2].Y:

                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 2)
                            {
                                coord[i].X = tmpCoord[0].X;
                                coord[i].Y = tmpCoord[i].Y - i + 1;
                            }
                            else
                            {
                                coord[i].X = tmpCoord[1].X;
                                coord[i].Y = tmpCoord[i].Y - i + 3;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'S' when coord[0].X == coord[1].X && coord[0].X < coord[2].X:
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 2)
                            {
                                coord[i].X = tmpCoord[1].X + i;
                                coord[i].Y = tmpCoord[1].Y;
                            }
                            else
                            {
                                coord[i].X = tmpCoord[1].X + i - 3;
                                coord[i].Y = tmpCoord[0].Y;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'Z' when coord[0].Y == coord[1].Y && coord[0].Y < coord[2].Y:

                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 2)
                            {
                                coord[i].X = tmpCoord[0].X;
                                coord[i].Y = tmpCoord[i].Y - i + 2;
                            }
                            else
                            {
                                coord[i].X = tmpCoord[1].X;
                                coord[i].Y = tmpCoord[i].Y - i + 2;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;
                    case 'Z' when coord[0].X == coord[1].X:

                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 2)
                            {
                                coord[i].X = tmpCoord[i].X + i;
                                coord[i].Y = tmpCoord[3].Y;
                            }
                            else
                            {
                                coord[i].X = tmpCoord[i].X + i - 2;
                                coord[i].Y = tmpCoord[2].Y;
                            }

                            if (IsColliding(coord[i]))
                            {
                                coord = tmpCoord;
                                break;
                            }
                        }
                        break;

                }
                
                UpdateCoord();
            }

            //Metoda sprawdzajaca kolizje
            private bool IsColliding(Point coord)
            {
                switch (coord)
                {
                    case Point p when p.X < 0:
                        return true;
                    case Point p when p.X > 9:
                        return true;
                    case Point p when p.Y < 0:
                        return true;
                    case Point p when p.Y > 19:
                        return true;
                    case Point p when blockedCoord.Contains(p):
                        return true;
                    default:
                        return false;
                        
                }
                
            }

            //Metoda aktualizujaca pozycje tetrimino
            private async void UpdateCoord()
            {
                 
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                for (int i = 0; i < 4; i++)
                {
                    Grid.SetColumn(r[i], (int)coord[i].X);
                    Grid.SetRow(r[i], (int)coord[i].Y);
                    
                }
                });
            }

            
            protected virtual void OnIsMovingChange()
            {
                isMovingChanged?.Invoke(this, EventArgs.Empty);
            }

            protected virtual void OnIsGameOverChange()
            {
                isGameOverChanged?.Invoke(this, EventArgs.Empty);
            }

        }
        public class Tetris
        {
            private Grid g;
            private char[] types = "ITOLJSZ".ToCharArray();
            private int time = 1000;
            public List<Tetrimino> tetriminos = new List<Tetrimino>();
            private List<Rectangle> landedRectangles = new List<Rectangle>();
            private int[] rectInRow = new int[20];
            private int score = 0;
            private bool isGameOver = false;
            private string userName;
            private Panel p;
            private TextBlock scoreBlock;
            public event EventHandler gameOver;

            public Tetris()
            {
                
            }

            public void start(Grid g, Panel p, string userName)
            {
                this.g = g;
                this.p = p;
                this.userName = userName;
                MakeBoard();
                
                scoreBlock = p.Children[1] as TextBlock;
                scoreBlock.Text = "Score: " + score.ToString();
                AddTetrimino();
                tetriminos[0].isMovingChanged += Tetris_isMovingChanged;
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            }

            //Metoda przyspieszajaca spadanie kolejnych blokow
            public void UpdateTime()
            {
                this.time = Convert.ToInt32(Math.Floor(time * 0.99)) ;
            }

            //Metoda aktualizujaca TextBlock scoreblock 
            public void UpdateScore(int newScore)
            {
                score = newScore;
                scoreBlock.Text = "Score: " + score.ToString();
            }

            //Metoda wywolywana gdy pojedyncze tetrimino sie zatrzyma
            private async void Tetris_isMovingChanged(object sender, EventArgs e)
            {

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    
                    foreach (var rectangle in tetriminos[tetriminos.Count - 1].GetRectangles())
                    {
                        landedRectangles.Add(rectangle);
                        rectInRow[Grid.GetRow(rectangle)]++;

                    }
                    int loopIteration = 0;
                    Loop:
                    
                    for(int i = rectInRow.Length-1; i > 0; i--)
                    {
                        if (rectInRow[i] == 10)
                        {
                            loopIteration++;
                            rectInRow[i] = 0;
                           
                            

                            foreach (var tmpRect in landedRectangles.FindAll(r => Grid.GetRow(r) == i))
                            {

                                g.Children.Remove(tmpRect);
                                landedRectangles.Remove(tmpRect);
                                

                            }
                           

                            for (int i1 = 0; i1 < landedRectangles.Count; i1++)
                            {
                                try
                                {
                                    Rectangle landedRectangle = landedRectangles[i1];
                                    rectInRow[Grid.GetRow(landedRectangle)]--;
                                    rectInRow[Grid.GetRow(landedRectangle) + 1]++;
                                    Grid.SetRow(landedRectangle, Grid.GetRow(landedRectangle) + 1);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("Błąd: " + ex.Message);
                                }
                                

                            }


                            goto Loop;
                        }
                       
                    }
                    UpdateScore(score + (10 * loopIteration));
                    UpdatePoints();
                    
                    for (int i = 0; i < rectInRow.Length; i++)
                    {
                        int item = rectInRow[i];
                        
                    }
                    UpdateTime();
                    if (!isGameOver)
                    {
                        AddTetrimino();
                        tetriminos.Last().isMovingChanged += Tetris_isMovingChanged;
                        tetriminos.Last().isGameOverChanged += Tetris_isGameOverChanged;
                    }
    
                });
                
            }

            //Metoda wywolywana gdy gra sie zakonczy
            public void Tetris_isGameOverChanged(object sender, EventArgs e)
            {
                var t = p.Children[2] as TextBlock;
                t.Visibility = Visibility.Visible;
                isGameOver = true;
                var usernameBlock = p.Children[4] as TextBox;
                var usernameButton = p.Children[5] as Button;
                DataAccess.AddData(userName, score);
                gameOver?.Invoke(this, EventArgs.Empty);


            }

            //Metoda aktywowana po wcisnieciu klawisza na klawiaturze
            private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
            {
                switch (args.VirtualKey)
                {
                    case Windows.System.VirtualKey.Down:
                        tetriminos.Last().MoveDown();
                        break;
                    case Windows.System.VirtualKey.Left:
                        tetriminos.Last().MoveLeft();
                        break;
                    case Windows.System.VirtualKey.Right:
                        tetriminos.Last().MoveRight();
                        break;
                    case Windows.System.VirtualKey.S:
                        tetriminos.Last().MoveToGround();
                        break;
                    case Windows.System.VirtualKey.A:
                        tetriminos.Last().Rotate();
                        break;
                }
            }

            //Metoda wypelniaca grid borderami
            public void MakeBoard()
            {

                for (int i = 0; i < 10; i++)
                {

                    for (int j = 0; j < 20; j++)
                    {
                        Border r = new Border();
                        r.Width = 40;
                        r.Height = 40;
                        r.Background = new SolidColorBrush(Windows.UI.Colors.DarkSlateGray);
                        r.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                        r.BorderThickness = new Thickness(1);
                       
                        g.Children.Add(r);
                        Grid.SetColumn(r, i);
                        Grid.SetRow(r, j);
                       
                    }
                }
            }

            //Metoda dodajaca tetrimino
            public void AddTetrimino()
            {
                Random random = new Random();
                int index = random.Next(this.types.Length);
                tetriminos.Add(new Tetrimino(this.types[index], this.g, this.time, GetPoints()));
            }

            //Metoda pobierajaca koordynaty z pojedynczych kwadratow, ktore juz wyladowaly
            private List<Point> GetPoints()
            {
                List<Point> points = new List<Point>();
               

                foreach (var landedRectangle in landedRectangles)
                {
                    Point p = new Point(Grid.GetColumn(landedRectangle), Grid.GetRow(landedRectangle));
                    points.Add(p);
                }



                return points;
            }

            
            private void UpdatePoints()
            {
                List<Point> tmpPoints = new List<Point>();
                foreach (var landedRectangle in landedRectangles)
                {
                    tmpPoints.Add(new Point(Grid.GetColumn(landedRectangle), Grid.GetRow(landedRectangle)));
                }
               
                foreach (var item in tetriminos)
                {
                    foreach (var tmpPoint in tmpPoints)
                    {
                        item.blockedCoord = new List<Point>();
                        item.blockedCoord.Add(tmpPoint);
                    }
                    
                }
            }

           
        }

       
        private void ApplyUserNameButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserNameBox.Text;
            ApplyUserNameButton.IsEnabled = false;
            tetris.start(Board, panel, userName);
            tetris.gameOver += Tetris_gameOver;
           
        }

        private void Tetris_gameOver(object sender, EventArgs e)
        {
            UpdateRank();
        }
        private void swipe(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial && !_isSwiped)
            {
                var swipedDistance = e.Cumulative.Translation.X;
                var swipedHeight = e.Cumulative.Translation.Y;

                if (Math.Abs(swipedDistance) <= 2 && Math.Abs(swipedHeight) <= 2) return;

                if(Math.Abs(swipedDistance) > Math.Abs(swipedHeight))
                {
                    if (swipedDistance > 0)
                    {
                        tetris.tetriminos.Last().MoveRight();
                        
                    }
                    else
                    {
                        tetris.tetriminos.Last().MoveLeft();
                        
                    }
                }
                else
                {
                    if(swipedHeight < 0)
                    {
                        tetris.tetriminos.Last().Rotate();
                    }
                    else
                    {
                        tetris.tetriminos.Last().MoveToGround();
                    }
                }

               
                _isSwiped = true;
            }
        }
        
        private void swipeCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _isSwiped = false;
        }

        private void tappedCompleted(object sender, EventArgs e)
        {
            
        }

        private void Board_Tapped(object sender, TappedRoutedEventArgs e)
        {
            tetris.tetriminos.Last().MoveDown();
        }
    }
}

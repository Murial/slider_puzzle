using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace slide_puzzle
{
    /// <summary>
    /// Interaction logic for in_game.xaml
    /// </summary>
    public partial class In_game : Window
    {
        //timer data
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;

        //default state image position
        List<Rectangle> defaultUnallocated = new List<Rectangle>();
        List<Rectangle> defaultAllocated = new List<Rectangle>();

        //image data
        List<Rectangle> unallocatedParts = new List<Rectangle>();
        List<Rectangle> allocatedParts = new List<Rectangle>();
        Image img = new Image();
        BitmapImage image = new BitmapImage(new Uri("file:///D:/KARAJO KULIAH/SEMESTER 4/Bengkel Aplikasi Desktop/PROJEK/slide_puzzle/slide_puzzle/img/ploy.jpg"));
        int imageIndex = 0;

        Rectangle rect = new Rectangle();

        public int ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = value; }
        }

        public In_game()
        {
            InitializeComponent();

            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        //stopwatch ticking if sw running
        void dt_Tick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                timer.Text = currentTime;
                start.IsEnabled = false;
                
            }
        }

        //restart stopwatch
        void dt_Restart()
        {
            if (sw.IsRunning)
            {
                sw = Stopwatch.StartNew();
            }
        }

        //stop stopwatch
        void dt_Stop()
        {
            if (sw.IsRunning)
            {
                sw.Stop();
            }
        }

        //cut image tiles
        private void ImageTiles(double x, double y, double width, double height)
        {
            ImageBrush ib = new ImageBrush();
            ib.Stretch = Stretch.UniformToFill;
            ib.ImageSource = image;
            ib.Viewport = new Rect(0, 0, 1.0, 1.0);
            
            //grab image portion
            ib.Viewbox = new Rect(x, y, width, height);
            ib.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
            ib.TileMode = TileMode.None;

            Rectangle tiles = new Rectangle();
            tiles.Fill = ib;
            tiles.Margin = new Thickness(0);
            tiles.HorizontalAlignment = HorizontalAlignment.Stretch;
            tiles.VerticalAlignment = VerticalAlignment.Stretch;
            tiles.MouseDown += new MouseButtonEventHandler(MovingControl);
            unallocatedParts.Add(tiles);
            defaultUnallocated.Add(tiles);
        }

        //custom image
        private void btnPickImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG" +
                        "|All Files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    image = new BitmapImage(new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
                    img = new Image { Source = image };
                    CreatePuzzleForImage();
                }
                catch
                {
                    MessageBox.Show("File tidak bisa dimuat " + ofd.FileName);
                }
            }
        }

        //arrange image tiles and randomize it
        private void CreatePuzzleForImage()
        {
            preview.Source = image;

            unallocatedParts.Clear();
            allocatedParts.Clear();

            defaultUnallocated.Clear();
            defaultAllocated.Clear();

            //dynamic image tiles will be added soon

            //row0
            ImageTiles(0, 0, 0.33333, 0.33333);
            ImageTiles(0.33333, 0, 0.33333, 0.33333);
            ImageTiles(0.66666, 0, 0.33333, 0.33333);
            //row1
            ImageTiles(0, 0.33333, 0.33333, 0.33333);
            ImageTiles(0.33333, 0.33333, 0.33333, 0.33333);
            ImageTiles(0.66666, 0.33333, 0.33333, 0.33333);
            //row2
            ImageTiles(0, 0.66666, 0.33333, 0.33333);
            ImageTiles(0.33333, 0.66666, 0.33333, 0.33333);

            DefaultState();
            RandomizeTiles();

            CreateBlankRect();

            sw.Start();
            dt.Start();

            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    allocatedParts[index].SetValue(Grid.RowProperty, i);
                    allocatedParts[index].SetValue(Grid.ColumnProperty, j);
                    gridPuzzle.Children.Add(allocatedParts[index]);
                    index++;
                }
            }

            //int defaultIndex = 0;
            //for (int i = 0; i < 3; i++)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        defaultAllocated[defaultIndex].SetValue(Grid.RowProperty, i);
            //        defaultAllocated[defaultIndex].SetValue(Grid.ColumnProperty, j);
            //        gridPuzzle.Children.Add(defaultAllocated[defaultIndex]);
            //        defaultIndex++;
            //    }
            //}
        }

        //randomize tiles
        private void RandomizeTiles()
        {
            Random rand = new Random();
            int allocated = 0;
            while (allocated != 8)
            {
                int index = 0;
                if (unallocatedParts.Count > 1)
                {
                    index = (int)(rand.NextDouble() * unallocatedParts.Count);
                }
                allocatedParts.Add(unallocatedParts[index]);
                unallocatedParts.RemoveAt(index);
                allocated++;
            }
        }

        //default state for comparing later
        private void DefaultState()
        {
            int allocated = 0;
            while (allocated != 8)
            {

                int index = 0;

                defaultAllocated.Add(defaultUnallocated[index]);
                defaultUnallocated.RemoveAt(index);
                allocated++;
            }
        }

        //adding blank rectangle
        private void CreateBlankRect()
        {
            Rectangle tiles = new Rectangle();
            tiles.Fill = new SolidColorBrush(Colors.White);
            tiles.Margin = new Thickness(0);
            tiles.HorizontalAlignment = HorizontalAlignment.Stretch;
            tiles.VerticalAlignment = VerticalAlignment.Stretch;
            allocatedParts.Add(tiles);
        }

        //game controls
        private void MovingControl(object sender, MouseButtonEventArgs e)
        {
            //get the source Rectangle, and the blank Rectangle
            //NOTE : Blank Rectangle never moves, its always the last Rectangle
            //in the allocatedParts List, but it gets re-allocated to 
            //different Gri Row/Column
            Rectangle rectCurrent = sender as Rectangle;
            Rectangle rectBlank = allocatedParts[allocatedParts.Count - 1];

            //mengambil informasi baris dan kolom tiles blank dan tiles yang akan berpindah
            int currentTileRow = (int)rectCurrent.GetValue(Grid.RowProperty);
            int currentTileCol = (int)rectCurrent.GetValue(Grid.ColumnProperty);
            int currentBlankRow = (int)rectBlank.GetValue(Grid.RowProperty);
            int currentBlankCol = (int)rectBlank.GetValue(Grid.ColumnProperty);

            //create possible valid move positions
            List<PossiblePositions> posibilities = new List<PossiblePositions>();
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow - 1, Col = currentBlankCol });
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow + 1, Col = currentBlankCol });
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow, Col = currentBlankCol - 1 });
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow, Col = currentBlankCol + 1 });

            //check for valid move
            bool validMove = false;
            foreach (PossiblePositions position in posibilities)
                if (currentTileRow == position.Row && currentTileCol == position.Col)
                    validMove = true;

            //only allow valid move
            if (validMove)
            {
                rectCurrent.SetValue(Grid.RowProperty, currentBlankRow);
                rectCurrent.SetValue(Grid.ColumnProperty, currentBlankCol);

                rectBlank.SetValue(Grid.RowProperty, currentTileRow);
                rectBlank.SetValue(Grid.ColumnProperty, currentTileCol);
                winStateCheck();
            }
            else
                return;
        }

        //possible possitions constructor
        struct PossiblePositions
        {
            public int Row { get; set; }
            public int Col { get; set; }
        }

        //shuffle image tiles
        private void shuffle(object sender, RoutedEventArgs e)
        {

            CreatePuzzleForImage();

            var rnd = new Random();
            var randomized = allocatedParts.OrderBy(item => rnd.Next());

            foreach (var value in randomized) { allocatedParts.IndexOf(value); }

            dt_Restart();
        }

        //start button event
        private void start_click(object sender, RoutedEventArgs e)
        {
            CreatePuzzleForImage();
        }

        //winning state
        private bool winState()
        {
            int tmp = 0 ;
            bool state = false;
            
            for (int i = 0; i < allocatedParts.Count; i++)
            {
                if (allocatedParts[i] == null)
                    return state;

                if (tmp == 9)
                    state = true;
            }

            return state;
        }

        //winning state checker
        private void winStateCheck()
        {
            if(RectCheck() == true)
            {
                test.Content = imageIndex;
                dt_Stop();
                MessageBox.Show("berhasil");
            }
        }

        //check for default location and actual location
        private bool RectCheck()
        {
            bool[] areEqual = new bool[defaultAllocated.Count];

            for (int i = 0; i < defaultAllocated.Count; i++)
            {
                if (defaultAllocated[i] == allocatedParts[i])
                {
                    areEqual[i] = true;
                }

            }

                if (areEqual.Contains(false))
                    return false;
                else
                    return true;
        }
    }
}

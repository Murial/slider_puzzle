using System;
using System.Collections.Generic;
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

namespace slide_puzzle
{
    /// <summary>
    /// Interaction logic for in_game.xaml
    /// </summary>
    public partial class in_game : Window
    {
        public in_game()
        {
            InitializeComponent();
        }

        private void Shuffle()
        {
            //Add cropped images in exact same order as they would look once the puzzle is completed 
            //(from first to last) 
            List<Bitmap> pictures = new List<Bitmap>()
            {
                  Properties.Resources.row_1_col_2,Properties.Resources.row_1_col_3,null,
                  Properties.Resources.row_2_col_1,Properties.Resources.row_2_col_2,
                                                   Properties.Resources.row_2_col_3,
                  Properties.Resources.row_3_col_1,Properties.Resources.row_3_col_2,
                                                   Properties.Resources.row_3_col_3
            };

            int k = 1;

            foreach (Bitmap pics in pictures)
            {
                if (pics == null) continue;
                pics.Tag = k.ToString();
                k++;
            }

            Random r = new Random();
            int pick;

            foreach (Control c in this.Controls)
            {
                pick = r.Next(pictures.Count);
                PictureBox p = (PictureBox)c;
                p.BackgroundImage = pictures[pick];
                pictures.RemoveAt(pick);
            }
        }
    }
}

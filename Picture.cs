using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurseWork_2D3D
{
    public partial class Picture : Form
    {
        private Bitmap photo;
        public Picture(Bitmap ph1, string settings)
        {
            this.Text = settings;
            photo = ph1;
            InitializeComponent();
            try
            {
                this.Size = new System.Drawing.Size(photo.Width + 20, photo.Height + 45);
                pictureBox1.Image = photo;
                pictureBox1.Size = new Size(photo.Width, photo.Height);
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            catch (Exception)
            {
                
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

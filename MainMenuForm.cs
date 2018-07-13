using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CurseWork_2D3D
{
    public partial class MainMenuForm : Form
    {
        public Bitmap newWorkForMe;
        public Bitmap endWork;
        private bool loadedIt = false;
        private bool madeIt = false;
        private string fileName;
        public static double _trueSegmLimit;
        public static int _trueRangeLimit;
        public static int _trueSegmSize;
        public MainMenuForm()
        {
            _trueSegmLimit = 14;
            _trueSegmSize = 10;
            _trueRangeLimit = 16384;
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (loadedIt == true)
                return;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "C:\\Users\\SONY\\Documents\\Visual Studio 2013\\Projects\\2D3D\\2D3D\\bin\\Debug";
            openFileDialog1.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadedIt = false;
                try
                {
                    newWorkForMe = new Bitmap(openFileDialog1.OpenFile());
                    fileName = openFileDialog1.SafeFileName;
                    ////////////////////////////////////////////////////////////////////////////////////////////
                    //Form3 filtresForm = new Form3(newWorkForMe);
                    //filtresForm.Show();
                    ////////////////////////////////////////////////////////////////////////////////////////////
                    
                    loadedIt = true; //флаг о том, что новая картинка загружена
                    madeIt = false; //флаг о том, что он еще не сделал 3д модель
                }
                catch (Exception ex)
                {
                    loadedIt = false;
                    MessageBox.Show("Ошибка при чтении файла. " + ex.Message);
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (loadedIt == true && madeIt == false)
            {
                Form filtres = new SettingsForm(newWorkForMe);
                filtres.Show();
            }
            else
            {
             
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
//            // глупая защита
//            if (Analizator.iStarted)
//                return;
//            Analizator.iStarted = true;
            if (loadedIt == true)
            {
                if (madeIt == false)
                {
                    ///////////////////////////////////////////////////////////////////
                    // здесь надо начать высчитывание по алгоритмам информации 2д фото
                    ///////////////////////////////////////////////////////////////////
                    // сегментация
                    //Segmentation seg = new Segmentation(newWorkForMe);
                    //seg.SortRebr();
                    //endWork = seg.Segment();
                    // после этого v2d точно существует

                    // анализатор сегментов и поиск их примерного расположения
                    

                    ///////////////////////////////////////////////////////////////////
                    // и преобразование полученных данных в 3д модель
                    ///////////////////////////////////////////////////////////////////
                    
                
                }
                Form1 frm = new Form1(newWorkForMe, fileName);
                frm.Show();
            }
        }

    }
}

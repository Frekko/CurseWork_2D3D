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
    public partial class SettingsForm : Form
    {
        private Bitmap _photo;
        private Bitmap _photoEdge;
        private Bitmap _photoEnd;
        private string settings = "";
        private double segmLimit;
        private int rangeLimit;
        int segmSize;
        
        public SettingsForm(Bitmap newWork)
        {
            _photo = newWork;
            InitializeComponent();
            label1.Text = "Значение лимита свёртки";
            label3.Text = "Порог минимума сегментов";
            label5.Text = "Лимит перепада яркости границ"; 
            label2.Text = "";
            label4.Text = "";
            label6.Text = "";
            segmLimit = MainMenuForm._trueSegmLimit;
            segmSize = MainMenuForm._trueSegmSize;
            rangeLimit = MainMenuForm._trueRangeLimit;

            textBox1.Text = segmLimit.ToString();
            textBox2.Text = segmSize.ToString();
            textBox3.Text = rangeLimit.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                segmLimit = double.Parse(textBox1.Text);
                label2.Text = "";
            }
            catch (FormatException)
            {
                label2.Text = "Неверный формат, введите double";
                return;
            }
            try 
            {
                segmSize = int.Parse(textBox2.Text);
                label4.Text = "";
            }
            catch (FormatException)
            {
                label4.Text = "Неверный формат, введите int";
                return;
            }
            if (segmSize == 0)
            {
                label4.Text = "Не может быть равным 0!";
                return;   
            }

            try
            {
                rangeLimit = int.Parse(textBox3.Text);
                label6.Text = "";
            }
            catch (FormatException)
            {
                label6.Text = "Неверный формат, введите int";
                return;
            }

            // Если надо отображать и сегментацию, и границы
            if (checkBox2.Checked && checkBox1.Checked)
            {
                Segmentation seg = new Segmentation(_photo, segmLimit, segmSize);
                seg.SortRebr();
                _photoEnd = seg.Segment();

                int height = _photo.Height;
                int width = _photo.Width;
                // к сегментации добавим обведённые границы
                Filters filt = new Filters(_photo, rangeLimit);
                _photoEdge = filt.SobelCanny(_photo);
                //_photoEdge = filt.Sobel(_photo);

                byte[] segmentByte = Filters.GetBytes(_photoEnd);
                byte[] edgeByte = Filters.GetBytes(_photoEdge);
                for (int i = 0; i < segmentByte.Length; i++)
                {
                    if (edgeByte[i] == 0)
                        segmentByte[i] = 0;

                }
                _photoEnd = Filters.GetBitmap(segmentByte, width, height);
            }
            else
            {
                // если надо отобразить сегментацию
                if (checkBox2.Checked)
                {
                    Segmentation seg = new Segmentation(_photo, segmLimit, segmSize);
                    seg.SortRebr();
                    _photoEnd = seg.Segment();
                }
                // Если надо отобразить границы
                if (checkBox1.Checked)
                {
                    int height = _photo.Height;
                    int width = _photo.Width;
                   
                    Filters filt = new Filters(_photo, rangeLimit);
                    _photoEnd = filt.SobelCanny(_photo);
                    //_photoEnd = Filters.GrayImage(_photo);
                }
            }

            settings = "limit=" + segmLimit + ";segmSize=" + segmSize + ";rangeLimit=" + rangeLimit;
            new Picture(_photoEnd, settings).Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            settings = "Original";
            new Picture(_photo, settings).Show();
        }
        
        // Сохранение настроек для модели
        private void button3_Click(object sender, EventArgs e)
        {
            MainMenuForm._trueSegmLimit = segmLimit;
            MainMenuForm._trueSegmSize = segmSize;
        }

        // Сбросить настройки к дефолтным 
        private void button4_Click(object sender, EventArgs e)
        {
            MainMenuForm._trueSegmLimit = 14;
            MainMenuForm._trueSegmSize = 10;
            textBox1.Text = MainMenuForm._trueSegmLimit.ToString();
            textBox2.Text = MainMenuForm._trueSegmSize.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }


    }
}

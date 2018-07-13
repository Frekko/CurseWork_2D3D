using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseWork_2D3D
{
    public class Image2D
    {
        private Bitmap Foto2D;
        public Bitmap newFoto2D;
       // Size s = Foto2D.Size;
        //var height = Foto2D.Height;
      // var width = Foto2D.Width;
       // Color[, ,] _colorMatrix = new Color[, 111, 111];

        public Image2D(Bitmap foto)
        {
            Foto2D = foto;
            //FotoLines newFoto = new FotoLines(Foto2D);
            //newFoto.Svertka();
            //pictureBox1.Image = Foto2D;
        }

        // Получение границ изображения
        public void Lines()
        {
            // Создаём новый объект для расчёта границ
            Filters newFoto = new Filters(Foto2D);

            // Применяем к нему необходимые методы
            // Метод Кэнни (Canny)
          //  newFoto.Ca


        }



        // Получение глубины вершин 
        public void Deep()
        {
            
        }

        /*
        public Bitmap StartFilters()
        {
            //Начинает вычислять границы
            this.Lines();


            // Начинает вычислять глубины
            this.Deep();




            /*
            Bitmap[,] fotoo = new Bitmap[Foto2D.Height, Foto2D.Width];
            FotoLines newFoto = new FotoLines(Foto2D);
            Color coloor = new Color();
            coloor = newFoto.ColorMap();
            fotoo[Foto2D.Height, Foto2D.Width] = newFoto.inBitMap(coloor);
            return fotoo[Foto2D.Height, Foto2D.Width];
        
            */
        }

       /* public void firstFilter()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    _colorMap[i, j] = Foto2D.GetPixel(j, i);
                }
            }
        } */
        
    //}
}

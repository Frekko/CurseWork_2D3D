using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CurseWork_2D3D
{
    public class Segmentation
    {
        private int _width;
        private int _height;
        private byte[] _photo;
        public double limit;
        public int segmSize;
        List<Versh> versh;
        public static List<Rib> ribs;
        public static List<Rib> smallRibs;
        public static Versh[,] v2d;

        private byte[,] r;
        private byte[,] g;
        private byte[,] b;

        public Segmentation(Bitmap photo2D)
        {

            limit = MainMenuForm._trueSegmLimit;
            segmSize = MainMenuForm._trueSegmSize;
            _width = photo2D.Width;
            _height = photo2D.Height;
            versh = new List<Versh>(_width * _height);
            ribs = new List<Rib>(_width * _height * 6);
            smallRibs = new List<Rib>(_width * _height);
            _photo = Filters.GetBytes(photo2D);
            r = new byte[_height, _width];
            g = new byte[_height, _width];
            b = new byte[_height, _width];
        }
        public Segmentation(Bitmap photo2D, double limit, int segmSize)
        {
            
            this.limit = limit;
            this.segmSize = segmSize;
            _width = photo2D.Width;
            _height = photo2D.Height;
            versh = new List<Versh>(_width * _height);
            ribs = new List<Rib>(_width * _height * 6);
            smallRibs = new List<Rib>(_width * _height);
            _photo = Filters.GetBytes(photo2D);
            r = new byte[_height, _width];
            g = new byte[_height, _width];
            b = new byte[_height, _width];
        }
         
        public void SortRebr()
        {
            int index = 0;
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    r[i, j] = _photo[index++];
                    g[i, j] = _photo[index++];
                    b[i, j] = _photo[index++];
                }
            }
            v2d = new Versh[_height, _width];

            for (int x = 0; x < _height; x++)
            {
                for (int y = 0; y < _width; y++)
                {
                    v2d[x, y] = new Versh(x, y, r[x, y], g[x, y], b[x, y]);
                }
            }
            int[,] rebr = { { 0, 1 }, { 0, 2 }, { 0, 3 }, { 1, 0 }, { 2, 0 }, { 3, 0 } };

            for (int x = 0, limitX = _height - 3; x < limitX; x++)
            {
                for (int y = 0, limitY = _width - 3; y < limitY; y++)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        //тут мы знаем, что следующий пиксель существует, потому что границы в цикле ширина-3, высота-3

                        CreateRib(x, y, positionX, positionY, r, g, b, v2d, ribs);
                    }
                }
                for (int y = _width - 3; y < _width; y++)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        // если следующий пиксель не существует
                        if ((positionX >= _height) || (positionY >= _width))
                            continue;

                        CreateRib(x, y, positionX, positionY, r, g, b, v2d, ribs);
                    }
                }
            }
            for (int x = _height - 3; x < _height; x++)
            {
                for (int y = 0; y < _width; y++)
                {

                    for (int i = 0; i < 6; i++)
                    {
                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        // если следующий пиксель не существует
                        if ((positionX >= _height) || (positionY >= _width))
                            continue;

                        CreateRib(x, y, positionX, positionY, r, g, b, v2d, ribs);
                    }
                }
            }
            // сортировка по дистанции (цене ребра)
            ribs.Sort((first, second) => first._dist.CompareTo(second._dist));
            foreach (Versh versh1 in v2d)
            {
                versh.Add(versh1);
            }
        }

        private void CreateRib(int x1, int y1, int x2, int y2, byte[,] r, byte[,] g, byte[,] b, Versh[,] v2d, List<Rib> list)
        {
            double dist = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2) +
                                         Math.Pow((r[x1, y1] - r[x2, y2]), 2) +
                                         Math.Pow((g[x1, y1] - g[x2, y2]), 2) +
                                         Math.Pow((b[x1, y1] - b[x2, y2]), 2));

            Rib rib = new Rib(v2d[x1, y1], v2d[x2, y2], dist);
            ribs.Add(rib);
            list.Add(rib);
        }

        public Bitmap Segment()
        {
            int[,] maxSize = new int[_height * _width, 2];
            // соединяем по пикселям
            for (int k = 0, ribCount = ribs.Count/*так он один раз вычислит и не будет делать это на каждом шаге*/; k < ribCount; k++)
            {
                Rib r = ribs[k];
                if (r._dist > limit)
                    break;
                //если у них один корень, то они уже в одном сегменте - тогда их не надо соединять снова
                if (r._firstV.Root == r._secondV.Root)
                    continue;
                //если же они в разных сегментах, то мы должны оценить, должны ли мы соединять сегменты.
                //Одинокий пиксель - сегмент из одной вершины

                //если вершины в разных сегментах, и вес ребра меньше лимита - соединяем сегменты
                if (r._dist < limit)
                    r._firstV.MergeSegment(r._secondV);
            }
            //для каждого сегмента ищем самое большое ребро внутри него (идём от больших к меньшим, т.к. ищем больший)
            for (int k = ribs.Count - 1; k >= 0; k--)
            {
                if (ribs[k]._firstV.Root == ribs[k]._secondV.Root && ribs[k]._firstV.MaxDist < ribs[k]._dist)
                    ribs[k]._firstV.MaxDist = ribs[k]._dist;
            }

            // Вторая проверка, уже с образовавшимися сегментами
            RemoveSmallSegments();
            // MergeSegments();

            ///////////////////////////////////////////////////////////////////////////////
            // Красим сегменты в зависимости от цвета корней
            byte[] mimimi = new byte[_photo.Length];
            int index = 0;
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    mimimi[index++] = v2d[i, j].Root._r;
                    mimimi[index++] = v2d[i, j].Root._g;
                    mimimi[index++] = v2d[i, j].Root._b;
                }
            }
            Bitmap end = Filters.GetBitmap(mimimi, _width, _height);
            //////////////////////////////////////////////////////////////////////
            //end = Filters.GrayImage(end);
            //Bitmap zyuzyu = Filters.GetBitmap(_foto, _width, _height);
            //Application.Run(new Form1(zyuzyu, zyu));
            return end;
            //new Form1(zyuzyu, end).Show();
        }
        // очень умная шикарная штука, но она осложнила программу и добавила неточностей(
        private void MergeSegments()
        {
            //теперь объединяем уже существующие сегменты по какой-то хитрой формуле - МАГИЯ
            double ksegm = 2;
            for (int k = 0; k < ribs.Count; k++)
            {
                Rib r = ribs[k];
                //если у них один корень, то они уже в одном сегменте - тогда их не надо соединять снова
                if (r._firstV.Root == r._secondV.Root)
                    continue;
                //если же они в разных сегментах, то мы должны оценить, должны ли мы соединять сегменты
                //одинокий пиксель - сегмент из одной вершины

                //поправка от количества пикселей
                double t1 = ksegm / r._firstV.VershCount;
                double t2 = ksegm / r._secondV.VershCount;
                //перепады внутри сегментов с поправкой от количества пикселей внутри сегмента
                double m1 = r._firstV.MaxDist + t1;
                double m2 = r._secondV.MaxDist + t2;

                //меньшая из больших дистанций внутри сегментов, с поправкой (та которая с количеством пикселей)
                double minMax = Math.Min(m1, m2);

                if (r._dist < minMax)
                {
                    r._firstV.MergeSegment(r._secondV);
                    Versh root = r._firstV.Root;
                    //после объединения ищем новый наибольший перепад
                    //т.к. рёбра упорядочены по возрастанию веса, а мы идём с конца - первое попавшееся ребро из нашего сегмента будет самым большим
                    GetMaxDist(root);
                }
            }

        }
        // принадлежит методу сверху, ищет самый больший перепад в сегменте
        private static void GetMaxDist(Versh root)
        {
            for (int i = ribs.Count - 1; i >= 0; i--)
            {
                //если оба принадлежат тому сегменту, который мы только что слепили - т.е. если у них тот же корень
                //то это ребро нам подходит
                if (ribs[i]._firstV.Root == root && ribs[i]._secondV.Root == root)
                {
                    root.MaxDist = ribs[i]._dist;
                    break;
                }
            }
        }
       
        // проходим второй раз либо по всем рёбрам, лбо по маленьким (реализовано два варианта)
        private void RemoveSmallSegments()
        {
            int[,] rebr = { { 0, 1 }, { 1, 0 } };

            for (int x = 0, limitX = _height - 1; x < limitX; x++)
            {
                for (int y = 0, limitY = _width - 1; y < limitY; y++)
                {

                    for (int i = 0; i < 2; i++)
                    {
                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        //тут мы знаем, что следующий пиксель существует, потому что границы в цикле ширина-3, высота-3

                        CreateRib(x, y, positionX, positionY, r, g, b, v2d, smallRibs);
                    }
                }
                for (int y = _width - 1; y < _width; y++)
                {

                    for (int i = 0; i < 2; i++)
                    {
                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        // если следующий пиксел не существует
                        if ((positionX >= _height) || (positionY >= _width))
                            continue;

                        CreateRib(x, y, positionX, positionY, r, g, b, v2d, smallRibs);
                    }
                }
            }
            for (int x = _height - 1; x < _height; x++)
            {
                for (int y = 0; y < _width; y++)
                {

                    for (int i = 0; i < 2; i++)
                    {
                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        // если следующий пиксел не существует
                        if ((positionX >= _height) || (positionY >= _width))
                            continue;

                        CreateRib(x, y, positionX, positionY, r, g, b, v2d, smallRibs);
                    }
                }
            }
            int minSize = (_height + _width) / segmSize;
             for (int k = 0, ribCount = ribs.Count; k < ribCount; k++) // для всех рёбер
           // for (int k = 0, ribCount = smallRibs.Count; k < ribCount; k++) // для маленьких рёбер
            {
                  Rib r = ribs[k]; // для всех рёбер
               // Rib r = smallRibs[k]; // для маленьких рёбер

                //если у них один корень, то они уже в одном сегменте - тогда их не надо соединять снова
                if (r._firstV.Root == r._secondV.Root)
                    continue;
                //если вершины в разных сегментах, и вес ребра меньше лимита - соединяем сегменты
                if (r._secondV.Root.VershCount < minSize)
                    r._firstV.MergeSegment(r._secondV);
                else if (r._firstV.Root.VershCount < minSize)
                    r._secondV.MergeSegment(r._firstV);
            }
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SharpGL.OpenGLAttributes;

namespace CurseWork_2D3D
{
    public class Analizator
    {
        public Bitmap _photo;
        public int _height;
        public int _width;
        public Versh[,] _v2d;
        public int[] zCoordinates;
        private bool[,] visited;
        public static bool iStarted = false;

        public Analizator(Bitmap photo)
        {
            _photo = photo;
            _height = photo.Height;
            _width = photo.Width;
            visited = new bool[_height, _width];
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    visited[i, j] = false;
            _v2d = Segmentation.v2d;
        }

        public List<AreaContainer> FormAreas()
        {
            List<AreaContainer> result = new List<AreaContainer>();
            if (_v2d == null)
            {
                Segmentation segm = new Segmentation(_photo);
                segm.SortRebr();
                segm.Segment();
                _v2d = Segmentation.v2d; //Полученный битмап нам неинтересен, только массив
            }

            AreaContainer currentArea = new AreaContainer();
            // Сначала ищем землю
            // Находим самый большой сегмент на нижней строке (его корень)
            Versh ground = FindLargeSegment(_height - 1);
            int pixelRow = 0;
            int pixelColumn = 0;
            for (int i = 0; i < _width; i++)
            {
                if (_v2d[_height - 1, i].Root == ground.Root)
                {
                    pixelRow = _height - 1;
                    pixelColumn = i;
                    break;
                }
            }
            // Формируем границу земли
            currentArea.Borders = FindBorder(pixelRow, pixelColumn);
            // Формируем всю область земли
            FormSingleArea(pixelRow, pixelColumn);
            // Добавляем землю в список областей - у неё будет индекс 0
            result.Add(currentArea);
            
            // Потом найдём небо
            currentArea = new AreaContainer();
            Versh sky = FindLargeSegment(0);
            pixelRow = 0;
            pixelColumn = 0;
            for (int i = 0; i < _width; i++)
            {
                if (_v2d[0, i].Root == sky.Root)
                {
                    pixelRow = 0;
                    pixelColumn = i;
                    break;
                }
            }
            // Формируем границу неба
            currentArea.Borders = FindBorder(pixelRow, pixelColumn);
            // Формируем всю область неба
            FormSingleArea(pixelRow, pixelColumn);
            // Добавляем небо в список областей - у него будет индекс 1
            result.Add(currentArea);
            
            // А теперь, дамы и господа, добавляем всё остальное
            for (int row = 0; row < _height; row++)
            {
                for (int column = 0; column < _width; column++)
                {
                    bool alreadyDone = false;
                    if (visited[row, column] == true)
                    {
                        continue;
                    }
                    // Чтобы избавиться от багов и мелких отростков пикселей
                    int svoi = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (row + i < 0 || row + i >= _height || column + j < 0 || column + j >= _width)
                                continue;
                            if (_v2d[row, column].Root == _v2d[row + i, column + j].Root)
                                svoi++;
                        }
                    }
                    if (svoi < 5)
                        continue;
                    currentArea = new AreaContainer();
                    // Ищет граничные пиксели
                    currentArea.Borders = FindBorder(row, column);
                    // Ищет все пиксели области
                    FormSingleArea(row, column);
                    // Добавляет в лист листов, где границы всех областей
                    result.Add(currentArea);
                }
            }
            return result;
        }

        private void FormSingleArea(int pixelRow, int pixelColumn)
        {
//            List<Versh> result = new List<Versh>();
            int[,] steps = {{0, 1}, {1, 1}, {1, 0}, {1, -1}, {0, -1}, {-1, -1}, {-1, 0}, {-1, 1}};
            Stack<Versh> untestedVersh = new Stack<Versh>(1000);

            untestedVersh.Push(_v2d[pixelRow, pixelColumn]);
            while (true)
            {
                Versh currentVersh;
                try
                {
                    currentVersh = untestedVersh.Pop();
                }
                catch (InvalidOperationException e)
                    //чтобы нормально выйти, если стек вершей пуст (т.е. когда все обработали все верши внутри области)
                {
                    return;
                }
                if (currentVersh == null)
                    break;
                pixelRow = currentVersh._row;
                pixelColumn = currentVersh._column;
                visited[pixelRow, pixelColumn] = true;

                for (int k = 0; k < 8; k++)
                {
                    int testRow = pixelRow + steps[k, 1];
                    int testColumn = pixelColumn + steps[k, 0];
                    if (testRow < 0 || testRow >= _height || testColumn < 0 || testColumn >= _width)
                        continue;

                    if (_v2d[pixelRow, pixelColumn].Root == _v2d[testRow, testColumn].Root &&
                        visited[testRow, testColumn] == false)
                    {
                        untestedVersh.Push(_v2d[testRow, testColumn]);
                    }
                }
            }

//            return result;
        }

        // Находим поверхность ("землю") 3д модели, а также максимальное количество слоёв
        public List<Versh> FindBorder(int firstRow, int firstColumn)
        {
            while (_v2d[firstRow, firstColumn].isBorderVersh(_height, _width) == false)
                firstRow--;

            int[,] steps = {{0, 1}, {1, 1}, {1, 0}, {1, -1}, {0, -1}, {-1, -1}, {-1, 0}, {-1, 1}};

            List<Versh> borders = new List<Versh>();
            List<Versh> trash = new List<Versh>();
            borders.Add(_v2d[firstRow, firstColumn]);
            Versh first = _v2d[firstRow, firstColumn];
            Versh current = null;
            for (int k = 0; k < 8; k++)
            {
                int secondColumn = firstColumn + steps[k, 1];
                int secondRow = firstRow + steps[k, 0];
                if (secondColumn < 0 || secondColumn >= _width || secondRow < 0 ||
                    secondRow >= _height)
                    continue;

                if (_v2d[secondRow, secondColumn].Root == first.Root &&
                    _v2d[secondRow, secondColumn].isBorderVersh(_height, _width) &&
                    _v2d[secondRow, secondColumn] != first)
                {
                    current = _v2d[secondRow, secondColumn];
                    borders.Add(current);
                    break;
                }
            }
            while (current != first)
            {
                bool foundNext = false;
                int currentRow = current._row;
                int currentColumn = current._column;
                for (int k = 0; k < 8; k++)
                {
                    int newRow = currentRow + steps[k, 0];
                    int newColumn = currentColumn + steps[k, 1];
                    if (newRow == first._row && newColumn == first._column)
                    {
                        return borders;
                    }
                    if (newColumn < 0 || newColumn >= _width || newRow < 0 ||
                        newRow >= _height)
                        continue;

                    if (_v2d[newRow, newColumn].Root == first.Root)
                        if (_v2d[newRow, newColumn].isBorderVersh(_height, _width))
                            if (borders.Contains(_v2d[newRow, newColumn]) == false)
                                if (trash.Contains(_v2d[newRow, newColumn]) == false) //для выхода из тупиков
                                {
                                    current = _v2d[newRow, newColumn];
                                    borders.Add(current);
                                    foundNext = true;
                                    break;
                                }
                }
                if (foundNext == false)
                {
                    trash.Add(borders[borders.Count - 1]);
                    borders.RemoveAt(borders.Count - 1);
                    current = borders[borders.Count - 1];
                }
            }
//            // Создадим землю 3д модели
//            for (int k = lastY, range = 0; k >= firstY; k--, range++)
//            {
//                for (int x = 0; x < _width; x++)
//                {
//                    // Оптимизировать?? 
//                    if (_v2d[k, x].Root == Ground.Root)
//                        _v2d[k, x]._z = range;
//                }
//            }
            return borders;
        }

        // Нахождение заднего фона изображения
        public void FindBackground()
        {
            // Найдём самый большой сегмент, который касается верхнего края
            Versh Background = FindLargeSegment(0);

            // Т.к. это фон, координата z максимально возможно отдалена от пользователя
            int z = zCoordinates.Length;

            // Зададим z координату всем вершинам сегмента
            setZ(Background, z);
        }


        // Нахождение самого большого сегмента в определённом ряду
        // Пока работает только для строк
        public Versh FindLargeSegment(int range)
        {
            int max = 1;
            Versh maxSegm = _v2d[range, 0];
            for (int x = 0; x < _width; x++)
            {
                if (_v2d[range, x].VershCount > max)
                {
                    max = _v2d[range, x].VershCount;
                    maxSegm = _v2d[range, x];
                }
            }
            return maxSegm.Root;
        }


        // метод, задающий координату Z всему сегменту
        public void setZ(Versh ourSegm, int z)
        {
            foreach (Versh v in _v2d)
            {
                if (v.Root == ourSegm.Root)
                    v._z = z;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurseWork_2D3D
{
    // методом Кэнни находим лайнарт
    public class Filters
    {
        private int height;
        private int width;
        public Bitmap _Photo;
        private int limit;

        // для Кэнни
        private byte[] DlBytes;
        private byte[] AtanBytes;


        public Filters(Bitmap Photo2D)
        {
            _Photo = Photo2D;
            height = Photo2D.Height; // высота фото
            width = Photo2D.Width; // ширина фото
            limit = MainMenuForm._trueRangeLimit;
            //AtanMass = new int[height*width];
            /*
            // Всё домножается на 3, т.к. мы проверяем по 3-ём палитрам
            double[,] kernel = {{-3, 0, 3}, {-6, 0, 6}, {-3, 0, 3}};

            foto2 = Filters.Svertka(foto1, height, width, kernel);*/

        }
        public Filters(Bitmap Photo2D, int limitR)
        {
            _Photo = Photo2D;
            height = Photo2D.Height; // высота фото
            width = Photo2D.Width; // ширина фото
            limit = limitR;
            /*
            // Всё домножается на 3, т.к. мы проверяем по 3-ём палитрам
            double[,] kernel = {{-3, 0, 3}, {-6, 0, 6}, {-3, 0, 3}};

            foto2 = Filters.Svertka(foto1, height, width, kernel);*/

        }
     
        ////////////////////////////////////////////////////////////////////////////
        // Два важных метода для конвертации
        // получение байтов из Битмапа
        public static byte[] GetBytes(Bitmap input)
        {
            int count = input.Height*input.Width*3; // размер нашего изображения 
            BitmapData inputD = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb); // выделяем память
            var output = new byte[count];
            Marshal.Copy(inputD.Scan0, output, 0, count); // копируем себе в массив
            input.UnlockBits(inputD); // разблокировка памяти
            return output;
        }

        // получение Битмапа из байтов
        public static Bitmap GetBitmap(byte[] input, int width, int height)
        {
            if (input.Length%3 != 0)
                return null;
                    // проверяем сможем ли мы сконвертировать обратно (должно делиться на 3, так хранятся цветные пиксели)
            var output = new Bitmap(width, height);
            BitmapData outputD = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb); // выделяем память

            Marshal.Copy(input, 0, outputD.Scan0, input.Length);
            output.UnlockBits(outputD); // разблокировка памяти
            return output;
        }

        ////////////////////////////////////////////////////////////////////////////

        // Перевод цветного изображения в градацию серого
        public static Bitmap GrayImage(Bitmap ImageBitmap)
        {
            byte[] ImageBytes = GetBytes(ImageBitmap);
            for (int i = 0; i < ImageBytes.Length; i += 3)
            {
                byte GrayColor = (byte) (ImageBytes[i]*0.299 + ImageBytes[i + 1]*0.587 + ImageBytes[i + 2]*0.114);
                ImageBytes[i] = GrayColor;
                ImageBytes[i+1] = GrayColor;
                ImageBytes[i+2] = GrayColor;
            }
            ImageBitmap = GetBitmap(ImageBytes, ImageBitmap.Width, ImageBitmap.Height);
            return ImageBitmap;
        }

        // алгоритм свёртки для фильтров
        public Bitmap Svertka(Bitmap foto, int height, int width, double[,] kernel) // принимает параметры ядра
        {
            // переводим наше изображение в байты
            byte[] inputBytes = GetBytes(foto);
            // создаём массив для итога с нужным размером
            byte[] outputBytes = new byte[inputBytes.Length];

            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);

            double sumR;
            double sumG;
            double sumB;
            double sumKernel;

            // проходим по изображению, не обрабатывая края
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    sumR = 0;
                    sumG = 0;
                    sumB = 0;
                    sumKernel = 0;

                    // проходим по ядру
                    for (int i = 0; i < kernelWidth; i++)
                    {
                        for (int j = 0; j < kernelHeight; j++)
                        {
                            int positionX = x + (i - (kernelWidth/2));
                            int positionY = y + (j - (kernelHeight/2));

                            // не обрабатываются края (при их категоричности доработаю)
                            if ((positionX < 0) || (positionX >= width) || (positionY < 0) || (positionY >= height))
                                continue;

                            // т.к. всё лежит подряд в массиве, то и умножаем позицию на 3, получаем 3 палитры подряд
                            byte r = inputBytes[3*(width*positionY + positionX) + 0];
                            byte g = inputBytes[3*(width*positionY + positionX) + 1];
                            byte b = inputBytes[3*(width*positionY + positionX) + 2];

                            double kernelValue = kernel[i, j];

                            sumR += r*kernelValue;
                            sumG += g*kernelValue;
                            sumB += b*kernelValue;

                            sumKernel += kernelValue;
                        }
                    }

                    // Нельзя делить на ноль
                    if (sumKernel <= 0)
                        sumKernel = 1;

                    // Нельзя выйти за цветовые пределы
                    sumR = sumR/sumKernel;
                    if (sumR < 0)
                        sumR = 0;
                    if (sumR > 255)
                        sumR = 255;

                    sumG = sumG/sumKernel;
                    if (sumG < 0)
                        sumG = 0;
                    if (sumG > 255)
                        sumG = 255;

                    sumB = sumB/sumKernel;
                    if (sumB < 0)
                        sumB = 0;
                    if (sumB > 255)
                        sumB = 255;

                    // Записываем результат в цвет пикселя
                    outputBytes[3*(width*y + x) + 0] = (byte) sumR;
                    outputBytes[3*(width*y + x) + 1] = (byte) sumG;
                    outputBytes[3*(width*y + x) + 2] = (byte) sumB;
                }
            }
            // Конвертируем полученные байты обратно в Битмап
            return GetBitmap(outputBytes, width, height);
        }

        // Фильтр Гаусса
        private Bitmap Gauss(Bitmap ourPhoto)
        {

            // Использует определенные значения свёртки 
            double[,] kernel = { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
            ourPhoto = Svertka(ourPhoto, ourPhoto.Height, ourPhoto.Width, kernel);
            
            // Ещё возможный вариант
            // double[,] kernel = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };

            // Всё домножено на 3, т.к. смотрим по 3-ём палитрам сразу'
            //double[,] kernelX = { { -3, 0, 3 }, { -6, 0, 6 }, { -3, 0, 3 } };
            //double[,] kernelY = { { -3, -6, -3 }, { 0, 0, 0}, { 3, 6, 3 } };
            //Bitmap fotoAfterSvertka = Svertka(_foto, height, width, kernelY);      
            return ourPhoto;
        }

        // Собель
        public Bitmap SobelCanny(Bitmap photo)
        {

            int width = photo.Width;
            int height = photo.Height;

            // избавимся от шумов Гауссом
            //double[,] kernel = { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
            photo = Gauss(photo);

            // Оператор Собеля
            double[,] kernelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            double[,] kernelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            // лимит для вычисления границ
            //int limit = 128*128;// (photo.Height * photo.Width);

            // переводим наше изображение в байты
            byte[] inputBytes = GetBytes(photo);
            // создаём итоговый массив с нужным размером
            byte[] outputBytes = new byte[inputBytes.Length];

            // создаём массив для итоговых длин с нужным размером
            //byte[] outputBytesD = new byte[inputBytes.Length/3];
            // создаём массив для итоговых углов с нужным размером
            //byte[] outputBytesT = new byte[inputBytes.Length/3];

            // вдруг потом понадобится другой размер 
            int kernelWidth = kernelX.GetLength(0);
            int kernelHeight = kernelX.GetLength(1);

            double sumRx;
            double sumGx;
            double sumBx;
            double sumRy;
            double sumGy;
            double sumBy;
            double sumKernelx;
            double sumKernely;

            // проходим по изображению, не обрабатывая края
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    sumRx = 0;
                    sumGx = 0;
                    sumBx = 0;
                    sumRy = 0;
                    sumGy = 0;
                    sumBy = 0;
                    sumKernelx = 0;
                    sumKernely = 0;

                    // проходим по ядру (по горизонтали)
                    for (int i = 0; i < kernelWidth; i++)
                    {
                        for (int j = 0; j < kernelHeight; j++)
                        {
                            int positionX = x + (i - (kernelWidth/2));
                            int positionY = y + (j - (kernelHeight/2));

                            // не обрабатываются края (при их категоричности доработаю)
                            if ((positionX < 0) || (positionX >= width) || (positionY < 0) || (positionY >= height))
                                continue;

                            // т.к. всё лежит подряд в массиве, то и умножаем позицию на 3, получаем 3 палитры подряд
                            byte rX = inputBytes[3*(width*positionY + positionX) + 0];
                            byte gX = inputBytes[3*(width*positionY + positionX) + 1];
                            byte bX = inputBytes[3*(width*positionY + positionX) + 2];


                            double kernelValueX = kernelX[i, j];

                            sumRx += rX*kernelValueX;
                            sumGx += gX*kernelValueX;
                            sumBx += bX*kernelValueX;

                            sumKernelx += kernelValueX;

                        }
                    }

                    // проходим по ядру (по вертикали)
                    for (int j = 0; j < kernelHeight; j++)
                    {
                        for (int i = 0; i < kernelWidth; i++)
                        {
                            int positionX = x + (i - (kernelWidth / 2));
                            int positionY = y + (j - (kernelHeight / 2));

                            // не обрабатываются края (при их категоричности доработаю)
                            if ((positionX < 0) || (positionX >= width) || (positionY < 0) || (positionY >= height))
                                continue;

                            // т.к. всё лежит подряд в массиве, то и умножаем позицию на 3, получаем 3 палитры подряд

                            byte rY = inputBytes[3 * (width * positionY + positionX) + 0];
                            byte gY = inputBytes[3 * (width * positionY + positionX) + 1];
                            byte bY = inputBytes[3 * (width * positionY + positionX) + 2];

                            double kernelValueY = kernelY[i, j];

                            sumRy += rY * kernelValueY;
                            sumGy += gY * kernelValueY;
                            sumBy += bY * kernelValueY;

                            sumKernely += kernelValueY;
                        }
                    }

                    
                    // Лайтовая версия КэнниСобеля - вводим настраиваемый лимит

                    if (sumRx*sumRx + sumRy*sumRy > limit || sumGx*sumGx + sumGy*sumGy > limit ||
                        sumBx*sumBx + sumBy*sumBy > limit)
                    {
                        outputBytes[3*(width*y + x) + 0] = 0;
                        outputBytes[3*(width*y + x) + 1] = 0;
                        outputBytes[3*(width*y + x) + 2] = 0;
                    }
                    else
                    {
                        outputBytes[3*(width*y + x) + 0] = 255;
                        outputBytes[3*(width*y + x) + 1] = 255;
                        outputBytes[3*(width*y + x) + 2] = 255;
                    }

                    /*
                    // чуть более расширенные данные для Кэнни, пока не надо
                    outputBytes[3 * (width * y + x)] = (byte)Math.Sqrt(sumGx * sumGx + sumGy * sumGy);
                    outputBytes[3 * (width * y + x) + 1] = (byte)Math.Sqrt(sumGx * sumGx + sumGy * sumGy);
                    outputBytes[3 * (width * y + x) + 2] = (byte)Math.Sqrt(sumBx * sumBx + sumBy * sumBy);

                    //outputBytesD[(width * y + x)] = (byte)Math.Sqrt(sumRx * sumRx + sumRy * sumRy);
                    //outputBytesT[(width * y + x)] = (byte)Math.Atan(sumRx / sumRy);
                    */
                }
            }

            //DlBytes = outputBytesD;
            //AtanBytes = outputBytesT;

            // Конвертируем полученные байты обратно в Битмап
            return GetBitmap(outputBytes, width, height);
        }
        
        
        //Алгоритм Кэнни  (Canny)
        public Bitmap CannyFilter(Bitmap photo)
        {
            Bitmap photoForCanny = photo;
            byte[] photoBytes = GetBytes(photo);

            byte[] atans = {0, 45, 90, 135};
            // избавимся от шумов фильтром Гаусса
            //double[,] kernel = { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
            //photo = Svertka(photo, height, width, kernel);
            // Обрабатывает 
            photoForCanny = Filters.GrayImage(photoForCanny);
            SobelCanny(photoForCanny);

            
            // Считаем максимальные перепады яркости и берём соответствующие углы
            for (int i = 0; i < DlBytes.Length; i++)
            {
                /*
                int max = 0;
                // тут смотрим, по какой палитре максимальный перепад
                for (int j = 0; j < 3; j ++)
                {
                    if (DlBytes[i + j] > DlBytes[i + max])
                        max = j;
                }
                */
                
                // дальше надо найти к какому углу из 4-ёх соответствующий угол ближе
                int minDif = Math.Abs(AtanBytes[i] - atans[0]);
                int numberAtan = 0;
                for (int numb = 1; numb < 4; numb++)
                {
                    // Упростить?? Лишнее вычисление?
                    if (minDif > Math.Abs(AtanBytes[i] - atans[numb]))
                    {
                        minDif = Math.Abs(AtanBytes[i] - atans[numb]);
                        numberAtan = numb;
                    }
                }
                //AtanMass[i] = atans[numberAtan];
                AtanBytes[i] = atans[numberAtan];
                //DlBytes[i] = DlBytes[i];
                // ???????? 
                //AtanBytes[i+1] = (byte)atans[numberAtan];
                //AtanBytes[i+2] = (byte)atans[numberAtan];
            }

            int sizeStr = height*3;
            byte[] endBytes = new byte[width*height*3];
            int pixelNumb = height+1;
            // Строим границы
            for (int j = 1; j < width-1; j++)
            {
                for (int y = 3; y < sizeStr-3; y+=3)
                {
                    int x = j*sizeStr;
                    // если угол направления градиента равен нулю, точка будет считаться границей, если её интенсивность больше чем у точки выше и ниже рассматриваемой точки
                    if (AtanBytes[pixelNumb] == 0)
                    {
                        if (DlBytes[pixelNumb] >= DlBytes[pixelNumb - height] && DlBytes[pixelNumb] >= DlBytes[pixelNumb + height])
                        {
                            endBytes[x + y] = 255;
                            endBytes[x + y + 1] = 255;
                            endBytes[x + y + 2] = 255;
                        }

                    }
                    // если угол направления градиента равен 90 градусам, точка будет считаться границей, если её интенсивность больше чем у точки слева и справа рассматриваемой точки
                    if (AtanBytes[pixelNumb] == 90)
                    {
                        if (DlBytes[pixelNumb] >= DlBytes[pixelNumb - 1] && DlBytes[pixelNumb] >= DlBytes[pixelNumb + 1])
                        {
                            endBytes[x + y] = 255;
                            endBytes[x + y + 1] = 255;
                            endBytes[x + y + 2] = 255;
                        }

                    }
                    // если угол направления градиента равен 45 градусам, точка будет считаться границей, если её интенсивность больше чем у точек находящихся в верхнем правом и нижнем левом углу от рассматриваемой точки 
                    if (AtanBytes[pixelNumb] == 45)
                    {
                        if (DlBytes[pixelNumb] >= DlBytes[pixelNumb - height + 1] && DlBytes[pixelNumb] >= DlBytes[pixelNumb + height - 1])
                        {
                            endBytes[x + y] = 255;
                            endBytes[x + y + 1] = 255;
                            endBytes[x + y + 2] = 255;
                        }

                    }
                    // если угол направления градиента равен 135 градусам, точка будет считаться границей, если её интенсивность больше чем у точек находящихся в верхнем левом и нижнем правом углу от рассматриваемой точки
                    if (AtanBytes[pixelNumb] == 135)
                    {
                        if (DlBytes[pixelNumb] >= DlBytes[pixelNumb - height - 1] && DlBytes[pixelNumb] >= DlBytes[pixelNumb + height + 1])
                        {
                            endBytes[x + y] = 255;
                            endBytes[x + y + 1] = 255;
                            endBytes[x + y + 2] = 255;
                        }
                    }
                    pixelNumb++;
                }
            }
            /*
            for (int b = 0; b < endBytes.Length; b++)
            {
                if (endBytes[b] != 255)
                    endBytes[b] = 0;
            }*/
            return GetBitmap(endBytes, photo.Width, photo.Height);
        }
        // Дополнительные реализованные функции, которые в последствии были заменены на более оптимальные        
        /*
        // получаем цветовую карту нашей фотографии и обратно
        private Color ColorMap()
        {
            Color[,] colorMap = new Color[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    colorMap[i, j] = _Foto.GetPixel(j, i);
                }
            }
            return colorMap[height, width];
        }
        // Перевод обратно в BitMap
        public Bitmap inBitMap(Color _exitColorMap)
        {
            Bitmap[,] exitfoto = new Bitmap[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    exitfoto[i, j].SetPixel(j, i, _exitColorMap);
                }
            }

            return exitfoto[height,width];
        }
        */
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph;
using SharpGL.Enumerations;


namespace CurseWork_2D3D
{
    public partial class Form1 : Form
    {
        //private double limit = 14;
        //private double segmSize = 10;
        private Bitmap _photo;
        private string _fileName;
        private int _height;
        private int _width;
        // середина модели для отрисовки
        private float srHeight;
        private float srWidth;
        private bool haveData;
        private bool polygonsFormed;
        private DrawingVertex[,] _drawingVertices;

        private List<AreaContainer> areaContainers;
        public Form1(Bitmap photo, string name)
        {
            _photo = photo;
            _height = _photo.Height;
            _width = _photo.Width;
            _fileName = name;
            haveData = false;
            polygonsFormed = false;
            InitializeComponent();
            
            // середина модели для отрисовки
            srHeight = _height / 2;
            srWidth = _width / 2;
        }


        private float rtri = 0;
        Texture texture = new Texture();
        

        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {

            OpenGL glWind = this.openGLControl1.OpenGL; // для удобства работы с окном вывода

            //надо накладывать 3д текстуру!
            //glWind.Enable(OpenGL.GL_TEXTURE_2D);
            //texture.Create(glWind, "testFoto2.jpg");

            glWind.Enable(OpenGL.GL_TEXTURE_2D);
            texture.Create(glWind, _fileName/*"testFoto10.jpg"*/); // задаём текстуру
            
            glWind.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT); // чистим цвета и глубины
            glWind.LoadIdentity(); // сброс системы координат к начальной позиции

            glWind.Translate(0.0f, -0.5f, -3.0f); // по сути двигаем перо, которым рисуем (f - float)
            glWind.Rotate(rtri, 0, 1, 0); // вращение системы координат (угол поворота, координаты вектора вращения)

//            texture.Bind(glWind);

            //int[] textures = new int[1000];

            // glTexCoord2f(x1 / 383.0, y1 / 383.0);
            // glTexCoord2f(x2 / 383.0, y1 / 383.0);
            // glTexCoord2f(x2 / 383.0, y2 / 383.0);
            // glTexCoord2f(x1 / 383.0, y2 / 383.0);

            // либо GUAD_STRIP попробовать
//            glWind.Begin(OpenGL.GL_POLYGON); // начинаем отрисовывать
//
//           // glWind.TexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, 300, 197, 100, 100, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, textures);
//
//            
//            glWind.Color(1.0f, 1.0f, 1.0f); // задаём цвет в RGB
//           // glWind.TexCoord(0.0f, 1.0f);
//            glWind.TexCoord(1.0 / _photo.Height * 10.0, 1.0 / _photo.Width * 50.0); 
//            glWind.DrawingVertex(-2.0f, 2.0f, -0.5f); // задаём вершину
//            //glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет в RGB
//
//            //glWind.TexCoord(1.0f, 1.0f);
//            glWind.TexCoord(1.0 / 1920 * 500.0, 1.0 / 1200 * 50.0); 
//            glWind.DrawingVertex(2.0f, 2.0f, -0.5f); // задаём вершину
//
//            //glWind.TexCoord(1.0f, 0.0f);
//            glWind.TexCoord(1.0 / 1920 * 745.0, 1.0 / 1200 * 125.0); 
//            glWind.DrawingVertex(4.0f, 1.0f, -0.5f); // задаём вершину
//
//            //glWind.TexCoord(0.0f, 0.0f);
//            glWind.TexCoord(1.0 / 1920 * 500.0, 1.0 / 1200 * 350.0); 
//            glWind.DrawingVertex(2.0f, -2.0f, -0.5f); // задаём вершину
//
//            glWind.TexCoord(1.0 / 1920 * 10.0, 1.0 / 1200 * 350.0);
//            glWind.DrawingVertex(-2.0f, -2.0f, -0.5f); // задаём вершину

            //glWind.DrawingVertex(0.0f, 0.0f, -1.0f); // задаём вершину

            //glWind.Color(1.0f, 0.0f, 1.0f); // задаём цвет в RGB
            //glWind.DrawingVertex(-0.3f, 0.2f, -1.0f); // задаём вершину
            //glWind.DrawingVertex(-0.4f, 0.3f, -1.0f); // задаём вершину
            //glWind.DrawingVertex(-0.2f, 0.2f, -1.0f); // задаём вершину
            //glWind.DrawingVertex(0.2f, 0.1f, -1.0f); // задаём вершину
            
            Set3DModel(glWind);

            glWind.Flush();

            rtri += 5;
        }

        public void Set3DModel(OpenGL glWind)
        {
            bool alreadyDone = false;
                Analizator analizator = new Analizator(_photo);
//            List<Versh> Ground = analizator.FindGround();

                if (haveData == false)
                {
                    // Преобразованные пиксели в вершины для рисования
                    _drawingVertices = new DrawingVertex[_height, _width];
                    // Получим лист листов с границами всех областей
                    areaContainers = analizator.FormAreas();
                    haveData = true;
                }
                MakePolygons(areaContainers, glWind);
//            Set3DPolygon(areaContainers[0].Borders, glWind);

//            for (int i = 0; i < _width; i++)
//            {
//                for (int j = 0; j < _height; j++)
//                {
//                    // лишние операции при нахождении
//                    foreach (Versh v in EndedRoots)
//                    {
//                        if (Segmentation.v2d[i, j].Root == v.Root)
//                        {
//                            alreadyDone = true;
//                            break;
//                        }
//                    }
//                    if (alreadyDone == false)
//                        Set3DPolygon(Segmentation.v2d[i, j].Root.VershCount, glWind, Segmentation.v2d[i, j].Root); // количество вершин, окно куда рисуем, Root?
//                    alreadyDone = false;
//                }
//            }
        }

        public void MakePolygons(List<AreaContainer> areas, OpenGL glWind)
        {
         // float earthYcoef = 1f; 
         // float earthZcoef = 1f;
         // float groundShift = 0;
            float farthestZ = 1;
            float sdvig = -0.5f;
            // areas[0] - земля, areas[1] - небо
            // Сначала преобразуем землю
            glWind.Begin(OpenGL.GL_POLYGON);
            texture.Bind(glWind);
            glWind.Color(1.0f, 1.0f, 1.0f); // Задаём цвет в RGB
            // Отрисовываем землю
            foreach (Versh versh in areas[0].Borders)
            {
                if (_drawingVertices[versh._row, versh._column] == null)
                {
                    DrawingVertex drawingVertex = new DrawingVertex();
                    drawingVertex.x = (float)versh._column / _width + sdvig;
                    drawingVertex.y = ((float) _height - versh._row)/_height; //* earthYcoef;
                    drawingVertex.z = ((float) versh._row - _height)/_height; //* earthZcoef;
                    drawingVertex.textx = (float) versh._column/_width;
                    drawingVertex.texty = ((float) _height - versh._row)/_height;
                    _drawingVertices[versh._row, versh._column] = drawingVertex;
                }
                float x = _drawingVertices[versh._row, versh._column].x;
                float y = _drawingVertices[versh._row, versh._column].y;
                float z = _drawingVertices[versh._row, versh._column].z;
                if (z < farthestZ)
                {
                    farthestZ = z;
                }
                float textx = _drawingVertices[versh._row, versh._column].textx;
                float texty = _drawingVertices[versh._row, versh._column].texty;
                glWind.TexCoord(textx, -texty);
                glWind.Vertex(x, y, z);
            }
            glWind.End();
            //потом небо
            glWind.Begin(OpenGL.GL_POLYGON);
            texture.Bind(glWind);
            glWind.Color(1.0f, 1.0f, 1.0f); // задаём цвет в RGB
            int minRow = _height;
            int minColumn = _width;
            foreach (Versh versh in areas[1].Borders)
            {
                if (_drawingVertices[versh._row, versh._column] == null)
                {
                    if (minRow == _height) //ищем координаты Верша, соответствующего самой высокой границе ОБЛАСТИ (не сегмента) земли
                    { //сравнение стоит,чтобы не считать заново для каждой вершины небы
                        foreach (Versh groundVersh in areas[0].Borders)
                        {
                            if (groundVersh._row < minRow)
                            {
                                minRow = groundVersh._row;
                                minColumn = groundVersh._column;
                            }
                        }
                    }
                    //из этих двух высот получаем величину смещения, чтобы приклеить небо к сплющенной земле
                    DrawingVertex drawingVertex = new DrawingVertex();
                    drawingVertex.x = (float) versh._column/_width+sdvig;
                    drawingVertex.y = ((float) _height - versh._row)/_height;
                    drawingVertex.z = _drawingVertices[minRow, minColumn].z;
                    //drawingVertex.y -= Math.Abs(groundShift*(drawingVertex.z/farthestZ));

                    drawingVertex.textx = (float)versh._column / _width;
                    drawingVertex.texty = ((float)_height - versh._row) / _height;
                    _drawingVertices[versh._row, versh._column] = drawingVertex;
                }
                float x = _drawingVertices[versh._row, versh._column].x;
                float y = _drawingVertices[versh._row, versh._column].y;
                float z = _drawingVertices[versh._row, versh._column].z;
                float textx = _drawingVertices[versh._row, versh._column].textx;
                float texty = _drawingVertices[versh._row, versh._column].texty;
                glWind.TexCoord(textx, -texty);
                glWind.Vertex(x, y, z);
            }
            glWind.End();
            //потом нацепим на них всё остальное -- но сначала просто сделаем вертексы
            for (int areaIndex = 2; areaIndex < areas.Count; areaIndex++)
            {
                List<Versh> borders = areas[areaIndex].Borders;
                foreach (Versh versh in borders)
                {
                    if (_drawingVertices[versh._row, versh._column] != null)
                        break;
                    _drawingVertices[versh._row, versh._column] = new DrawingVertex();
                        _drawingVertices[versh._row, versh._column].x = float.PositiveInfinity; //этим мы показываем, что вертекс ещё не обработали
                }
            }
            //а теперь мы идём по картинке снизу вверх и клеим все области к земле или областям под ними
            //сначала пройдём по самой нижней строке, потому что она крепится не к земле, а ставится с z=0
            for (int column = 0; column < _width; column++)
            {
                int row = _height - 1;
                DrawingVertex lowerDrawingVertex = _drawingVertices[row, column];
                if (lowerDrawingVertex == null || double.IsPositiveInfinity(lowerDrawingVertex.x) == false)
                    continue;

                lowerDrawingVertex.x = (float) column/_width + sdvig;
                lowerDrawingVertex.y = 0;
                lowerDrawingVertex.z = 0;

                lowerDrawingVertex.textx = (float) column/_width;
                lowerDrawingVertex.texty = ((float) _height - row)/_height;
                foreach (AreaContainer area in areas)
                {
                    List<Versh> borders = area.Borders;
                    if (borders.Contains(Segmentation.v2d[row, column]))
                        //в borders лежат пиксели-границы (в виде Versh)
                    {
                        foreach (Versh versh in borders)
                        {
                            DrawingVertex drawingVertex = _drawingVertices[versh._row, versh._column];
                            drawingVertex.x = (float) versh._column/_width +sdvig;
                            drawingVertex.y = ((float) _height - versh._row)/_height; //- groundShift*((float) (_height - minRow)/_height);
                            drawingVertex.z = 0;
                            //drawingVertex.y -= Math.Abs(groundShift * (drawingVertex.z / farthestZ));

                            drawingVertex.textx = (float) versh._column/_width;
                            drawingVertex.texty = ((float) _height - versh._row)/_height;
                        }
                    }
                }
            }
            for (int row = _height - 2; row >= 0; row--)
            {
                for (int column = 0; column < _width; column++)
                {
                    DrawingVertex drawingVertex = _drawingVertices[row, column];
                    if (_drawingVertices[row, column] == null || double.IsPositiveInfinity(drawingVertex.x) == false)
                        continue;
                    //теперь мы знаем, что вертекс существует, н не был обработан
                    //находим границу вершей, в котором лежит текущий
                    List<Versh> border = null;
                    foreach (AreaContainer area in areas)
                    {
                        List<Versh> areaBorder = area.Borders;
                        if (areaBorder.Contains(Segmentation.v2d[row, column]))
                            border = areaBorder;
                    }
                    int[] steps = new[] {0, -1, 1};
                        foreach (int step in steps)
                        {
                            if (column + step < 0 || column + step >= _width)
                                continue;
                            if (_drawingVertices[row, column + step] == null ||
                                double.IsPositiveInfinity(_drawingVertices[row, column + step].x) == true)
                                continue;
                            //теперь мы знаем, что данный нижний вертекс существует и уже был обработан
                            //поэтому заберём у него координату - приклеим нашу область к нему

                            foreach (Versh versh in border)
                            {
                                drawingVertex = _drawingVertices[versh._row, versh._column];
                                drawingVertex.x = (float) versh._column/_width + sdvig;
                                drawingVertex.y = ((float) _height - versh._row)/_height; 
                                                //  groundShift*((float) (_height - minRow)/_height);
                                drawingVertex.z = _drawingVertices[row, column + step].z;
                                //drawingVertex.y -= Math.Abs(groundShift * (drawingVertex.z / farthestZ));

                                drawingVertex.textx = (float) versh._column/_width;
                                drawingVertex.texty = ((float) _height - versh._row)/_height;
                            }
                        }
                }
            }
            for (int i = 2; i < areas.Count; i++)
            {
                glWind.Begin(OpenGL.GL_POLYGON);
                texture.Bind(glWind);
                glWind.Color(1.0f, 1.0f, 1.0f); // задаём цвет в RGB
                foreach (Versh versh in areas[i].Borders)
                {
                    float x = _drawingVertices[versh._row, versh._column].x;
                    float y = _drawingVertices[versh._row, versh._column].y;
                    float z = _drawingVertices[versh._row, versh._column].z;
                    float textx = _drawingVertices[versh._row, versh._column].textx;
                    float texty = _drawingVertices[versh._row, versh._column].texty;
                    glWind.TexCoord(textx, -texty);
                    glWind.Vertex(x, y, z);

                }
                glWind.End();
            }
        }

        // Старая версия
        public void Set3DPolygon(List<Versh> borderList, OpenGL glWind)
        {
            glWind.Begin(OpenGL.GL_POLYGON); // начинаем отрисовывать
            glWind.Color(1.0f, 1.0f, 1.0f); // задаём цвет в RGB
            float z = -1000;
            texture.Bind(glWind);
            int shtuchka = 0;
            bool shtuchkaBool = false;
            foreach (Versh versh in borderList)
            {
                glWind.TexCoord(1.0 / _width * (versh._column - (1f / _width * srHeight)), 1.0 /_height*(versh._row - (1f/_height*srWidth)));
                glWind.Vertex((float) 1/_width*(versh._column - (1f/_width*srHeight)),
                    (float)-1 / _height * (versh._row - (1f / _height * srWidth)), (float)1 / _width * (versh._z - (1f / _width * srHeight)));
                    //1f/20*(float)versh._z); // задаём вершину
            }
            glWind.End();
//            for (int i = 0; i < kol; i++) // количество углов, для автоматизации отрисовки 
//            {
//                glWind.TexCoord(1.0 / _photo.Height * 10.0, 1.0 / _photo.Width * 50.0);
//                glWind.DrawingVertex(-2.0f, 2.0f, -0.5f); // задаём вершину
//            }
        }

        public void getTexturePart()
        {
            
        }


        private void openGLControl1_Load(object sender, EventArgs e)
        {


        }

        // Моя учебная пирамида
        private void Pyramid(object sender, EventArgs e)
        {
            OpenGL glWind = this.openGLControl1.OpenGL; // для удобства работы с окном вывода

            glWind.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT); // чистим цвета и глубины
            glWind.LoadIdentity(); // сброс системы координат к начальной позиции

            glWind.Translate(-1.5f, 0.0f, -10.0f); // по сути двигаем перо, которым рисуем (f - float)
            glWind.Rotate(rtri, 0, 1, 0); // вращение системы координат (угол поворота, координаты вектора вращения)
            glWind.Begin(OpenGL.GL_TRIANGLES); // начинаем отрисовывать

            glWind.Color(1.0f, 0.0f, 0.0f); // задаём цвет в RGB
            glWind.Vertex(0.0f, 1.0f, 0.0f); // задаём вершину
            glWind.Color(0.0f, 1.0f, 0.0f); // задаём цвет
            glWind.Vertex(-1.0f, -1.0f, 1.0f); // задаём вершину
            glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет
            glWind.Vertex(1.0f, -1.0f, 1.0f); // задаём вершину

            glWind.Color(1.0f, 0.0f, 0.0f); // задаём цвет в RGB
            glWind.Vertex(0.0f, 1.0f, 0.0f); // задаём вершину
            glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет
            glWind.Vertex(1.0f, -1.0f, 1.0f); // задаём вершину
            glWind.Color(0.0f, 1.0f, 0.0f); // задаём цвет
            glWind.Vertex(1.0f, -1.0f, -1.0f); // задаём вершину

            glWind.Color(1.0f, 0.0f, 0.0f); // задаём цвет в RGB
            glWind.Vertex(0.0f, 1.0f, 0.0f); // задаём вершину
            glWind.Color(0.0f, 1.0f, 0.0f); // задаём цвет
            glWind.Vertex(1.0f, -1.0f, -1.0f); // задаём вершину
            glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет
            glWind.Vertex(-1.0f, -1.0f, -1.0f); // задаём вершину

            glWind.Color(1.0f, 0.0f, 0.0f); // задаём цвет в RGB
            glWind.Vertex(0.0f, 1.0f, 0.0f); // задаём вершину
            glWind.Color(0.0f, 0.0f, 1.0f); // задаём цвет
            glWind.Vertex(-1.0f, -1.0f, -1.0f); // задаём вершину
            glWind.Color(0.0f, 1.0f, 0.0f); // задаём цвет
            glWind.Vertex(-1.0f, -1.0f, 1.0f); // задаём вершину

            glWind.End();
            glWind.Flush();

            rtri += 10;
        }
    }
}

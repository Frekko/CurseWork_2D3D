using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseWork_2D3D
{
    public class Versh
    {
        private static int[,] rebr = { { 0, 1 }, { 0, 2 }, { 0, 3 }, { 0, -1 }, { 0, -2 }, { 0, -3 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { -1, 0 }, { -2, 0 }, { -3, 0 } };
        public readonly int _row;
        public readonly int _column;
        public readonly byte _r;
        public readonly byte _g;
        public readonly byte _b;
        private Versh _parent;
        private double _maxDist;
        private int _count;
        public int _z = -1;

        // достаём корень какой-либо вершины, кладём его в _parent и возвращаем
        public Versh Root
        {
            get
            {
                //если _parent = null, то мы уже в корне
                if (_parent == null)
                    return this;
                //пока _parent - не корень, двигаем его вверх по дереву
                while (_parent._parent != null)
                    _parent = _parent._parent;
                return _parent;
            }
            set
            {
                Versh root = this.Root;
                root._parent = value.Root;
            }
        }

        public int VershCount
        {
            get { return Root._count; }
            set { Root._count = value; }
        }

        public double MaxDist
        {
            get { return Root._maxDist; }
            set { Root._maxDist = value; }
        }

        public Versh(int x1, int y1, byte r, byte g, byte b)
        {
            _count = 1;
            _row = x1;
            _column = y1;
            _r = r;
            _g = g;
            _b = b;
            _parent = null;
            _maxDist = 0;
        }

        public void MergeSegment(Versh v)
        {
            //проверяем, если уже один сегмент 
            if (this.Root == v.Root)
            {
                return;
            }
            VershCount += v.VershCount;
            v.Root = this.Root;
        }

//        public override bool Equals(object obj)
//        {
//            Versh v = obj as Versh;
//            if (v == null)
//                return false;
//            return _row == v._row && _column == v._column;
//        }

        public bool isBorderVersh(int height, int width)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (_row + i < 0 || _row + i >= height || _column + j < 0 || _column + j >= width)
                        return true;
                    //если мы сюда пришли, то мы знаем, что соседний к нашему пиксель внутри картинки
                    if (Segmentation.v2d[_row + i, _column + j].Root != Root)
                        return true;
                }
            }
            return false;
        }
    }
}
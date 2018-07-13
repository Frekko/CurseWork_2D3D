using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseWork_2D3D
{
    public class Rib
    {
        public Versh _firstV;
        public Versh _secondV;
        public double _dist;
        public int index;
        public Rib(Versh first, Versh second, double dist)
        {
            _firstV = first;
            _secondV = second;
            _dist = dist;
            index = 0;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    class Coord
    {
        protected int x, y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public virtual int X { get { return x; } set { x = value; } }
        public virtual int Y { get { return y; } set { y = value; } }

        public bool Equals(Coord c)
        {
            if ((this.x == c.x) &&
                (this.y == c.y))
            {
                return true;
            }
            else
                return false;
        }

        public virtual object Clone()
        {
            Coord c = new Coord(this.x, this.y);
            return c;
        }
    }
}

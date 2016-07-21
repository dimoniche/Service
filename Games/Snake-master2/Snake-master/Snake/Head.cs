using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    class Head : Coord
    {
        public Head(int x, int y)
            : base(x, y)
        {
        }

        public override int X
        {
            get { return x; }
            set
            {
                if ((value < 0) || (value > MaxX - 1))
                {
                    throw new ArgumentException();
                }
                else
                {
                    x = value;
                }

            }
        }

        public override int Y
        {
            get { return y; }
            set
            {
                if ((value < 0) || (value > MaxY - 1))
                {
                    throw new ArgumentException();
                }
                else
                {
                    y = value;
                }
            }
        }

        public int MaxX { get; set; }
        public int MaxY { get; set; }

        public override object Clone()
        {
            Head c = new Head(this.x, this.y);
            c.MaxX = this.MaxX;
            c.MaxY = this.MaxY;
            return c;
        }
    }
}

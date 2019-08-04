using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TRY.Kampfmodus.Structure
{
    class QField<T>
    {
        private readonly HashSet<T>[,] mQField;
        private readonly int mSquareSize;
        private int mXmin = 0;
        private int mYmin = 0;
        private int mXmax;
        private int mYmax;
        public QField(int height, int width, int squareSize)
        {
            mQField = new HashSet<T>[width,height];
            mSquareSize = squareSize;
            mXmax = width - 1;
            mYmax = height - 1;
            for (var i = 0; i < mQField.GetLength(0); i++)
            {
                for (var j = 0; j < mQField.GetLength(1); j++)
                {
                    mQField[i, j] = new HashSet<T>();
                }
            }
        }

        public void AddElementAt(T element, Rectangle elementArea)
        {

            foreach (var square in GetSquares(elementArea))
            {
                if (IsInField(square))
                {
                    mQField[square.X, square.Y].Add(element);
                }
            }
        }

        public HashSet<T> GetAllElementsNear(Rectangle area)
        {
            HashSet<T> resList = new HashSet<T>();
            foreach (var square in GetSquares(area))
            {
                if (IsInField(square))
                {
                    foreach (var cInQ in mQField[square.X,square.Y])
                    {
                        resList.Add(cInQ);
                    }
                }
            }
            return resList;
        }

        private bool IsInField(Point square)
        {
            if (square.X >= mXmin
                && square.X <= mXmax
                && square.Y >= mYmin
                && square.Y <= mYmax)
            {
                return true;
            }
            return false; 
        }

        //Borrowed from http://eugen.dedu.free.fr/projects/bresenham/
        //Thanks Eugen Dedu
        private List<Point> fatBresenham(int y1, int x1, int y2, int x2)
        {
            List<Point> res = new List<Point>();
            int i;               // loop counter
            int ystep, xstep;    // the step on y and x axis
            int error;           // the error accumulated during the increment
            int errorprev;       // *vision the previous value of the error variable
            int y = y1, x = x1;  // the line points
            int ddy, ddx;        // compulsory variables: the double values of dy and dx
            int dx = x2 - x1;
            int dy = y2 - y1;
            res.Add(new Point(x1, y1));  // first point
                            // NB the last point can't be here, because of its previous point (which has to be verified)
            if (dy < 0)
            {
                ystep = -1;
                dy = -dy;
            }
            else
                ystep = 1;
            if (dx < 0)
            {
                xstep = -1;
                dx = -dx;
            }
            else
                xstep = 1;
            ddy = 2 * dy;  // work with double values for full precision
            ddx = 2 * dx;
            if (ddx >= ddy)
            {  // first octant (0 <= slope <= 1)
               // compulsory initialization (even for errorprev, needed when dx==dy)
                errorprev = error = dx;  // start in the middle of the square
                for (i = 0; i < dx; i++)
                {  // do not use the first point (already done)
                    x += xstep;
                    error += ddy;
                    if (error > ddx)
                    {  // increment y if AFTER the middle ( > )
                        y += ystep;
                        error -= ddx;
                        // three cases (octant == right->right-top for directions below):
                        if (error + errorprev < ddx)  // bottom square also
                            res.Add( new Point(x,y - ystep));
                        else if (error + errorprev > ddx)  // left square also
                            res.Add(new Point(x - xstep, y));
                        else
                        {  // corner: bottom and left squares also
                            res.Add(new Point(x, y - ystep));
                            res.Add(new Point(x - xstep, y));
                        }
                    }
                    res.Add(new Point(x, y));
                    errorprev = error;
                }
            }
            else
            {  // the same as above
                errorprev = error = dy;
                for (i = 0; i < dy; i++)
                {
                    y += ystep;
                    error += ddx;
                    if (error > ddy)
                    {
                        x += xstep;
                        error -= ddy;
                        if (error + errorprev < ddy)
                            res.Add(new Point(x - xstep, y));
                        else if (error + errorprev > ddy)
                            res.Add(new Point(x, y - ystep));
                        else
                        {
                            res.Add(new Point(x - xstep, y));
                            res.Add(new Point(x, y - ystep));
                        }
                    }
                    res.Add(new Point(x, y));
                    errorprev = error;
                }
            }

            return res;
        }

        public List<T> GetAllElementsNear(Vector2 lineStart, Vector2 lineEnd)
        {

            List<T> resList = new List<T>();
            foreach (var square in fatBresenham(
                (int)lineStart.Y/mSquareSize,
                (int)lineStart.X / mSquareSize, 
                (int)lineEnd.Y / mSquareSize, 
                (int)lineEnd.X / mSquareSize))
            {
                if (IsInField(square))
                {
                    foreach (var cInQ in mQField[square.X, square.Y])
                    {
                        if (!resList.Contains(cInQ))
                            resList.Add(cInQ);
                    }
                }
            }
            return resList;
        }

        private Point[] GetSquares(Rectangle rect)
        {
            int startx = rect.X / mSquareSize;
            int starty = rect.Y / mSquareSize;
            int stopx = (rect.X + rect.Width) / mSquareSize;
            int stopy = (rect.Y + rect.Height) / mSquareSize;
            int xwidth = (stopx - startx + 1);
            int ywidth = (stopy - starty + 1);
            var squares = new Point[xwidth*ywidth];
            //Along the x axis, find all squares.
            for (var x = 0; x < xwidth; x++)
            {
                for (var y = 0; y < ywidth; y++)
                {
                    squares[y * xwidth + x].X = x+startx;
                    squares[y * xwidth + x].Y = y+starty;
                }
            }
            return squares;
        } 
    }
}

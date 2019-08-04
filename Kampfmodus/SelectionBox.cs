using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus
{
    sealed class SelectionBox
    {
        private readonly Texture2D mTexture;

        public SelectionBox(Texture2D texture)
        {
            mTexture = texture;
        }

        public Rectangle Rectangle { get; set; }

        public void Draw(SpriteBatch sb)
        {
            // Draw the horizontal line while tracking the mouse movement.
            // Since a rectangle has four sides, 4 draw functions will be needed.
            DrawHorizontalLine(Rectangle.Y, sb);
            DrawHorizontalLine(Rectangle.Y + Rectangle.Height, sb);
            DrawVerticalLine(Rectangle.X, sb);
            DrawVerticalLine(Rectangle.X + Rectangle.Width, sb);
        }
        private void DrawHorizontalLine(int thePositionY, SpriteBatch sb)
        {
            // This variable will help us to transform the mouse coordinates using the WorldToScreen method from the Camera class
            Point yHolder;
            yHolder.X = 0;
            yHolder.Y = thePositionY;

            // From the starting point, we can draw to the from left to right, which will increase the width of the box.
            if (Rectangle.Width > 0)
            {
                // Draw as many dotted lines as the current X position of the mouse is reached.
                for (int aCounter = 0; aCounter <= Rectangle.Width - 10; aCounter += 10)
                {
                    if (Rectangle.Width - aCounter >= 0)
                    {
                        Point incr = Rectangle.Location;
                        incr.X += aCounter;
                        sb.Draw(mTexture,
                            new Rectangle(incr.X,
                                yHolder.Y,
                                10,
                                5),
                            Color.White);
                    }
                }
            }

            // We can draw from right to left, which will decrease the width of the box
            else if (Rectangle.Width < 0)
            {
                for (int aCounter = -10; aCounter >= Rectangle.Width; aCounter -= 10)
                {
                    if (Rectangle.Width - aCounter <= 0)
                    {
                        Point decr = Rectangle.Location;
                        decr.X += aCounter;
                        sb.Draw(mTexture,
                            new Rectangle(decr.X,
                                yHolder.Y,
                                10,
                                5),
                            Color.White);
                    }
                }
            }
        }

        /// <summary>
        /// Using the same logic of the DrawHorizontal method. This time the height will be adjusted.
        /// </summary>
        /// <param name="thePositionX"></param>
        /// <param name="sb"></param>
        private void DrawVerticalLine(int thePositionX, SpriteBatch sb)
        {
            Point xHolder;
            xHolder.X = thePositionX;
            xHolder.Y = 0;
            if (Rectangle.Height > 0)
            {
                for (int aCounter = 0; aCounter <= Rectangle.Height; aCounter += 10)
                {
                    if (Rectangle.Height - aCounter >= 0)
                    {
                        Point incr = Rectangle.Location;
                        incr.Y += aCounter;
                        sb.Draw(mTexture,
                            new Rectangle(xHolder.X,
                                incr.Y,
                                10,
                                5), Color.White);
                    }
                }
            }

            else if (Rectangle.Height < 0)
            {
                for (int aCounter = 0; aCounter >= Rectangle.Height; aCounter -= 10)
                {
                    if (Rectangle.Height - aCounter <= 0)
                    {
                        Point decr = Rectangle.Location;
                        decr.Y += aCounter;
                        sb.Draw(mTexture,
                            new Rectangle(xHolder.X,
                                decr.Y,
                                10,
                                5),
                            Color.White);
                    }
                }
            }
        }
    }
}

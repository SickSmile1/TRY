using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Graphics
{
    class Line
    {
        public Point StartPoint { get; }
        public Point EndPoint { get; set; }
        private readonly string mTextureIdentifier;

        public Line(string textureIdentifier, Point a, Point b)
        {
            StartPoint = a;
            EndPoint = b;
            mTextureIdentifier = textureIdentifier;
        }

        public void Draw(SpriteBatch sb, TextureManager tm)
        {
            if (StartPoint.X != EndPoint.X && StartPoint.Y != EndPoint.Y)
            {
                Vector2 distance = StartPoint.ToVector2() - EndPoint.ToVector2();
                float angle = (float) Math.Atan2(distance.Y, distance.X);
                sb.Draw(tm.GetTexture(mTextureIdentifier),
                    new Rectangle(
                        EndPoint.X,
                        EndPoint.Y,
                        (int) distance.Length(),
                        2),
                    null,
                    Color.White,
                    angle,
                    new Vector2(0, 0),
                    SpriteEffects.None,
                    0);
            }
        }
}
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Graphics
{
    class Explosion
    {
        private readonly String mTextureDescriptor;
        public float Duration { get; }
        public float TimePassed { get; private set; }
        private int NumFrames { get; }
        private Point Position { get; }
        private Point Size { get; }

        public Explosion(String textureDescriptor, float duration, int numFrames, Point position, Point size)
        {
            Duration = duration;
            mTextureDescriptor = textureDescriptor;
            NumFrames = numFrames;
            Position = position;
            Size = size;
        }

        public void Update(GameTime gameTime)
        {
            TimePassed += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
        }

        public void Draw(SpriteBatch sb, TextureManager tm)
        {
            var texture = tm.GetTexture(mTextureDescriptor);
            int numOfAbilityFrames = NumFrames;

            int frameWidth = texture.Width / numOfAbilityFrames;
            int frameHeight = texture.Height;
            float percentageOfCooldown = TimePassed / Duration;
            percentageOfCooldown = percentageOfCooldown > 1 ? 1 : percentageOfCooldown;
            int whichImage = (int)Math.Floor(percentageOfCooldown * (numOfAbilityFrames - 1));
            Rectangle sourceRectangle = new Rectangle(whichImage * frameWidth, 0, frameWidth, frameHeight);
            sb.Draw(
                texture,
                new Rectangle(Position, Size),
                sourceRectangle,
                Color.White);
        }
    }
}

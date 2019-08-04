using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Abilities
{
    internal class DistractionObject
    {
        private readonly string mTextureDescriptor;
        public Rectangle ObjectArea { get; }
        public float Duration { get; private set; }
        public DistractionObject(string textureDescriptor, Vector2 position, Point size, float duration)
        {
            ObjectArea = new Rectangle((int)position.X, (int)position.Y, size.X, size.Y);
            mTextureDescriptor = textureDescriptor;
            Duration = duration;
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager)
        {
            sb.Draw(textureManager.GetTexture(mTextureDescriptor), ObjectArea, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            Duration -= gameTime.ElapsedGameTime.Milliseconds/1000.0f;
        }
    }

}

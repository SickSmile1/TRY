using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus
{
    class CollectableItem
    {
        private readonly string mTextureIdentifier;

        public Rectangle ItemHitBox { get; }

        public bool Collected { get; }

        // true:Energy, false:Oxygen
        public bool Item { get; }
        public Vector2 Position { get; }

        public CollectableItem(string textureIdentifier, bool item, Vector2 position)
        {
            mTextureIdentifier = textureIdentifier;
            Item = item;
 
            Collected = false;
            ItemHitBox = new Rectangle((int) position.X, (int) position.Y,32,32);
            Position = position;
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager)
        {
            if (!Collected)
            {
                sb.Draw(textureManager.GetTexture(mTextureIdentifier), ItemHitBox, Color.AliceBlue);
            }
        }
    }
}

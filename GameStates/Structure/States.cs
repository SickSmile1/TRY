using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.GameStates.Structure
{
    public abstract class States
    {
        protected ContentManager mContent;
        protected GraphicsDevice mGraphicsDevice;
        protected Game1 mGame;
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);


        protected States(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            mGame = game;
            mGraphicsDevice = graphicsDevice;
            mContent = content;
        }
        public string StateName { get; set; }
    }
}
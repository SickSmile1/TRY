using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using TRY.GameStates.Structure;

namespace TRY.GameStates
{
    class GameOver : States
    {
        private Textures mTextures;
        private KeyboardState mKeyboardState;

        public GameOver(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            mGame = game;
            mGraphicsDevice = graphicsDevice;
            mContent = content;
            mTextures = new Textures(content);
            var s = Directory.GetCurrentDirectory();
            if (File.Exists(s+"\\level.txt")) File.Delete(s+"\\level.txt");
            if (File.Exists(s+"\\game.txt")) File.Delete(s+"\\game.txt");
            if (File.Exists(s+"\\progress.txt")) File.Delete(s+"\\progress.txt");
            game.PlayerProgress = new Progress();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTextures.GetTexture("GameOver"),
                destinationRectangle: new Rectangle(0, 0,
                    mGraphicsDevice.Viewport.Width,
                    mGraphicsDevice.Viewport.Height), Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            var newKeyboardState = Keyboard.GetState();
            if (newKeyboardState.IsKeyDown(Keys.Escape) && mKeyboardState.IsKeyUp(Keys.Escape))
            {
                var i = new MenuState(mGame, mGraphicsDevice, mContent);
                mGame.mScreenManager.AddScreen(i);
            }
            mKeyboardState = newKeyboardState;
        }
    }
}

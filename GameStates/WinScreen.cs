using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TRY.GameStates.Structure;

namespace TRY.GameStates
{
    class WinScreen: States
    {
        private Textures mTextures;
        private KeyboardState mKeyboardState;

        public WinScreen(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            mGame = game;
            game.Achieved.KilledBoss = true;
            mGraphicsDevice = graphicsDevice;
            mContent = content;
            mTextures = new Textures(content);
            var s = Directory.GetCurrentDirectory();
            if (File.Exists(s+"\\level.txt")) File.Delete(s+"\\level.txt");
            if (File.Exists(s+"\\game.txt")) File.Delete(s+"\\game.txt");
            if (File.Exists(s+"\\progress.txt")) File.Delete(s+"\\progress.txt");
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTextures.GetTexture("Win"),
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

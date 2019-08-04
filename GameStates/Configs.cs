using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TRY.GameStates.Structure;

namespace TRY.GameStates
{
    class Configs : States
    {
        // All buttons for the MainMenu
        private readonly List<Button> mButtons;

        // State and bool to paint button grey if mouse is over the button
        private new readonly Game1 mGame;
        private Textures mTextures;

        public Configs(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            mTextures = new Textures(content);
            mGame = game;
            Button back;
            Button fullscreen;
            Button music;
            mButtons = new List<Button>
            {
                {back = new Button(50, 60, "Zurück")},
                {fullscreen = new Button(50, 120, "Vollbild")},
                {music = new Button(50, 180, "Musik")}
            };
            back.Click += Back_Click;
            fullscreen.Click += Fullscreen_Click;
            music.Click += Music_Click;
        }

        private void Music_Click(object sender, EventArgs e)
        {
            MediaPlayer.Volume = MediaPlayer.Volume.Equals(0.4f) ? 0f : 0.4f;
        }

        private void Back_Click(object sender, EventArgs e)
        {
             mGame.mScreenManager.RemoveScreen();
        }

        private void Fullscreen_Click(object sender, EventArgs e)
        {
            if (!mGame.mGraphics.IsFullScreen)
            {
                mGame.mGraphics.PreferredBackBufferHeight =
                    mGame.mGraphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                mGame.mGraphics.PreferredBackBufferWidth =
                    mGame.mGraphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                mGame.mGraphics.IsFullScreen = true;
            }
            else
            {
                mGame.mGraphics.PreferredBackBufferHeight = 900;
                mGame.mGraphics.PreferredBackBufferWidth = 1200;
                mGame.mGraphics.IsFullScreen = false;
            }
            mGame.mGraphics.ApplyChanges();
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTextures.GetTexture("Background"), destinationRectangle:
                             new Rectangle(0, 0, mGraphicsDevice.Viewport.Width,
                             mGraphicsDevice.Viewport.Height), Color.AliceBlue);
            foreach (var button in mButtons)
            {
                button.Draw(spriteBatch, mTextures);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var button in mButtons)
            {
                button.Update();
            }
        }
    }
}

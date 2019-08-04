using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TRY.GameStates.Structure;

namespace TRY.GameStates
{
    class Credits : States
    {
        private readonly Button mBack;
        private MouseState mPreviousMouseState, mCurrentMouseState;
        private Textures mTextures;
        private readonly Button mAcrediet;
        public Credits(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            mTextures = new Textures(content);

            mBack = new Button(50, 60, "Zurück");
            mAcrediet = new Button(250, 150);
            mAcrediet.mButtonRectangle.Width = 700;
            mAcrediet.mButtonRectangle.Height = 500;
            mBack.Click += mBack_Click;
        }

        private void mBack_Click(object sender, EventArgs e)
        {
            mGame.mScreenManager.RemoveScreen();
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTextures.GetTexture("Background"), destinationRectangle:
                             new Rectangle(0, 0, mGraphicsDevice.Viewport.Width,
                             mGraphicsDevice.Viewport.Height), Color.AliceBlue);
            spriteBatch.Draw(mTextures.GetTexture("Credits"), destinationRectangle: mAcrediet.mButtonRectangle, Color.White);
            mBack.Draw(spriteBatch, mTextures);
        }

        public override void Update(GameTime gameTime)
        {
            mPreviousMouseState = mCurrentMouseState;
            mCurrentMouseState = Mouse.GetState();

            mBack.Update();
            if (mBack.ReturnRectangle().Contains(mCurrentMouseState.X, mCurrentMouseState.Y)
                && !mBack.ReturnRectangle().Contains(mPreviousMouseState.X, mPreviousMouseState.Y))
                mBack.SetColor(mBack);

            else if (!mBack.ReturnRectangle().Contains(mCurrentMouseState.X, mCurrentMouseState.Y) &&
                     mBack.ReturnRectangle().Contains(mPreviousMouseState.X, mPreviousMouseState.Y))
                mBack.SetColor(mBack);
        }
    }
}

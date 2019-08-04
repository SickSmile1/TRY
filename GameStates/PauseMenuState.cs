using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TRY.GameStates.Structure;
using TRY.SaveGame;

namespace TRY.GameStates
{
    sealed class PauseMenuState : States
    {
        // All buttons for the MainMenu
        private readonly List<Button> mButtons;
        private BattleMode mBattleMode;

        // State and bool to paint button grey if mouse is over the button
        private MouseState mCurrentMouseState, mPreviousMouseState;
        private KeyboardState mKeyboardStateOld;

        private Textures mTextures;

        public PauseMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, BattleMode bms = null) : base(game, graphicsDevice, content)
        {
            mTextures = new Textures(content);
            mBattleMode = bms;
            mButtons = new List<Button>();
            mButtons.Add(new Button(50, 60, "Hauptmenü"));
            mButtons.Add(new Button(50, 120, "Weiter"));
            mButtons.Add(new Button(50, 180, "Einstellungen"));
            mButtons.Add(new Button(50, 240, "Beenden"));
        
            mButtons[0].Click += mMainMenu_Click;
            mButtons[1].Click += mContinue_Click;
            mButtons[2].Click += mConfigs_Click;
            mButtons[3].Click += mExit_Click;
            
        }

        private void mMainMenu_Click(object sender, EventArgs e)
        {
            mGame.mState = new MenuState(mGame, mGraphicsDevice, mContent);
            mGame.mScreenManager.AddScreen(mGame.mState as MenuState);
        }

        private void mExit_Click(object sender, EventArgs e)
        {
            mGame.Exit();
        }

        private void mConfigs_Click(object sender, EventArgs e)
        {
            mGame.mState = new Configs(mGame, mGraphicsDevice, mContent);
            mGame.mScreenManager.AddScreen(mGame.mState);
        }

        private void mContinue_Click(object sender, EventArgs e)
        {
            mGame.mScreenManager.RemoveScreen();
            mBattleMode = mGame.mScreenManager.CurrentScreen(1) as BattleMode;
            if (mBattleMode != null)
            {
                mBattleMode.StateName = "BattleModeState";
                SaveStatetoFile.SaveStateFile(mGame, mBattleMode.BattleModeState);
            }
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
            mPreviousMouseState = mCurrentMouseState;
            mCurrentMouseState = Mouse.GetState();

            KeyboardState newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.Escape) && mKeyboardStateOld.IsKeyUp(Keys.Escape))
            {
                mBattleMode = mGame.mScreenManager.CurrentScreen(i: 2) as BattleMode;
                if (mBattleMode != null)
                {
                    mBattleMode.StateName = "BattleModeState";
                    mGame.mState = mBattleMode;
                }
            }

            foreach (var button in mButtons)
            {
                button.Update();
                if (button.ReturnRectangle().Contains(mCurrentMouseState.X, mCurrentMouseState.Y)
                    && !button.ReturnRectangle().Contains(mPreviousMouseState.X, mPreviousMouseState.Y))
                    button.SetColor(button);

                else if (!button.ReturnRectangle().Contains(mCurrentMouseState.X, mCurrentMouseState.Y) &&
                         button.ReturnRectangle().Contains(mPreviousMouseState.X, mPreviousMouseState.Y))
                    button.SetColor(button);
            }

            mKeyboardStateOld = newKeyboardState;
        }
    }
}

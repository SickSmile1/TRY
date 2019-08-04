using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TRY.GameStates.Structure;

namespace TRY.GameStates
{
    sealed class MenuState : States
    {
        // All buttons for the MainMenu
        private readonly List<Button> mButtons;

        // State and bool to paint button grey if mouse is over the button
        private readonly Textures mTextures;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            Button newGame;
            Button bContinue;
            Button configs;
            Button credits;
            Button techDemo;
            Button achievements;
            Button exit;
            mTextures = new Textures(content);
            mButtons = new List<Button>
            {
                {newGame = new Button(50, 60, "Neues Spiel")},
                {bContinue = new Button(50, 120, "Spiel Fortsetzen")},
                {configs = new Button(50, 180, "Einstellungen")},
                {credits = new Button(50, 240, "Mitwirkende")},
                {achievements = new Button(50, 300, "Errungenschaften")},
                {techDemo = new Button(50, 360, "TechDemo")},
                {exit = new Button(50, 420, "Beenden")}
            };

            newGame.Click += mNewGame_Click;
            bContinue.Click += mContinue_Click;
            configs.Click += mConfigs_Click;
            credits.Click += mCredits_Click;
            achievements.Click += mAchievements_Click;
            techDemo.Click += mTechDemo_Click;
            exit.Click += mExit_Click;
            Song song = content.Load<Song>("Music/Crawl");
            MediaPlayer.Play(song);
        }

        private void mCredits_Click(object sender, EventArgs e)
        {
            mGame.mState = new Credits(mGame, mGraphicsDevice, mContent);
            mGame.mScreenManager.AddScreen(mGame.mState);
        }

        private void mNewGame_Click(object sender, EventArgs e)
        {
            var s = Directory.GetCurrentDirectory();
            if (File.Exists(s+"\\achieved.txt")) File.Delete(s+"\\achieved.txt");
            if (File.Exists(s+"\\level.txt")) File.Delete(s+"\\level.txt");
            if (File.Exists(s+"\\game.txt")) File.Delete(s+"\\game.txt");
            if (File.Exists(s+"\\progress.txt")) File.Delete(s+"\\progress.txt");
            mGame.PlayerProgress = new Progress();
            mGame.Achieved = new Achieved();
            var i = new LevelScreen(mGame, mGraphicsDevice, mContent, firstStart: true);
            mGame.mState = i;
            mGame.mScreenManager.AddScreen(mGame.mState);
        }

        private void mConfigs_Click(object sender, EventArgs e)
        {
            mGame.mState = new Configs(mGame, mGraphicsDevice, mContent);
            mGame.mScreenManager.AddScreen(mGame.mState);
        }

        private void mContinue_Click(object sender, EventArgs e)
        {
            var s = Directory.GetCurrentDirectory();
            if (File.Exists(@s+"\\game.txt"))
            {
                mGame.mState = new BattleMode(mGame, mGraphicsDevice, mContent, load: true, level: mGame.PlayerProgress.CurrentLevel);
                mGame.mScreenManager.AddScreen(mGame.mState);
            }
            else if (File.Exists(@s+"\\level.txt"))
            {
                var i = new LevelScreen(mGame, mGraphicsDevice, mContent, true);
                mGame.mState = i;
                mGame.mScreenManager.AddScreen(mGame.mState);
            }
            else
            {
                mNewGame_Click(sender, e);
            }
        }
        private void mAchievements_Click(object sender, EventArgs e)
        {
            mGame.mState = new Achievements(mGame, mGraphicsDevice, mContent);
            mGame.mScreenManager.AddScreen(mGame.mState);
        }

        private void mTechDemo_Click(object sender, EventArgs e)
        {
            mGame.mState = new TechDemo(mGame, mGraphicsDevice, mContent);
            mGame.mScreenManager.AddScreen(mGame.mState);
        }

        private void mExit_Click(object sender, EventArgs e)
        {
            mGame.Exit();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTextures.GetTexture("BackgroundStart"), destinationRectangle:
                             new Rectangle(0, 0, mGraphicsDevice.Viewport.Width,
                             mGraphicsDevice.Viewport.Height), Color.AliceBlue);

            foreach (var t in mButtons)
            {
                t.Draw(spriteBatch, mTextures);
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

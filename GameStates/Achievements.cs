using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using TRY.GameStates.Structure;

namespace TRY.GameStates
{
    class Achievements : States
    {
        private Button mBack;
        private List<Button> mAchievements;
        private MouseState mPreviousMouseState, mCurrentMouseState;
        private Textures mTextures;

        [JsonIgnore]
        private new readonly Game1 mGame;

        public Achievements(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            mTextures = new Textures(content);
            mGame = game;
            InitializeButtons();
            mAchievements[10].mText = "Highscore : " + mGame.Achieved.CalculateHighscore(mGame);
            mAchievements[11].mText = "Getötete Gegner : "+mGame.Achieved.KilledEnemys;
            mAchievements[12].mText = "Gespielte Zeit : " + (int)mGame.Achieved.Timer/60;
            mAchievements[13].mText = "Charaktere gefunden : " + mGame.Achieved.PlayerCount;
            mAchievements[14].mText = "Sauerstoff-Flaschen gefunden : " + mGame.Achieved.Oxygen;
            mAchievements[15].mText = "Abgeschlossene Level : " + mGame.Achieved.LevelsPlayed() +"%";
            UpdateAchieved();
            mBack.Click += mBack_Click;
        }

        private void UpdateAchieved()
        {
            var d = mGame.Achieved.ReturnAchieved();
            for (var i = 0; i < 10; i++)
            {
                if (d[i] == 1) mAchievements[i].SetColor(mAchievements[i]);
            }
        }

        private void InitializeButtons()
        {
            mBack = new Button(50, 50, "Zurück");
            mAchievements = new List<Button>
            {
                { new Button(250, 50, "First Blood") },
                { new Button(250, 100, "You win!") },
                { new Button(250, 150, "What is wrong with you?") },
                { new Button(250, 200, "I'm Here To Kick Ass And Chew Bubblegum") },
                { new Button(250, 250, "Say 'hello' to my little friend!") },
                { new Button(250, 300, "Houston, we have a problem.") },
                { new Button(250, 350, "It's alive! It's alive!") },
                { new Button(250, 400, "I don't feel so good...") },
                { new Button(250, 450, "I'm going to make him an offer he can't refuse.") },
                { new Button(250, 500, "Mission Impossible") },
                { new Button(250, 550, "Highscore ") },
                { new Button(250, 600, "Getötete Gegner :") },
                { new Button(250, 650, "Gespielte Zeit :") },
                { new Button(250, 700, "Charaktere gefunden :") },
                { new Button(250, 750, "Sauerstoff-Flaschen gefunden :") },
                { new Button(250, 800, "Abgeschlossene Level :") }
            };
            foreach (var button in mAchievements)
            {
                button.mButtonRectangle.Width = 500;
            }

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
            mBack.Draw(spriteBatch, mTextures);
            foreach (var button in mAchievements)
            {
                button.Draw(spriteBatch, mTextures);
            }
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

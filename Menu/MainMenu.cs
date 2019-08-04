using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TRY.Menu
{
    class MainMenu
    {
        // All buttons for the MainMenu
        private readonly Button mNewGame;
        private readonly Button mContinue;
        private readonly Button mConfigs;
        private readonly Button mCredits;
        private readonly Button mAchievements;
        private readonly Button mTechDemo;
        private readonly Button mExit;
        protected Dictionary<string, Button> mDButtons;
        private SpriteBatch mSpriteBatch;
        private ContentManager mContent;

        // State and bool to paint button grey if mouse is over the button
        private bool mMouseHovering;
        private MouseState mCurreMouseState, mPreviousMouseState;

        public MainMenu(ContentManager content, SpriteBatch spriteBatch)
        {
            mDButtons = new Dictionary<string, Button>
            { 
                {"New Game", mNewGame = new Button(content, 50, 50, "Neues Spiel")},
                {"Continue", mContinue= new Button(content, 50, 100, "Spiel Fortsetzen")},
                {"Configs", mConfigs = new Button(content, 50, 150, "Einstellungen")},
                {"Credits", mCredits = new Button(content, 50, 200, "Mitwirkende")},
                {"Achievements", mAchievements = new Button(content, 50, 250, "Errungenschaften")},
                {"TechDemo", mTechDemo = new Button(content, 50, 300, "TechDemo")},
                {"Exit", mExit = new Button(content, 50, 350, "Beenden")}
            };
        }

        public virtual void LoadContent(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mSpriteBatch.Draw(mContent.Load<Texture2D>("Menu/MenuBackground"), destinationRectangle: new Rectangle(0, 0, 1920, 1080));
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mNewGame.Draw(gameTime, spriteBatch);
            mContinue.Draw(gameTime, spriteBatch);
            mConfigs.Draw(gameTime, spriteBatch);
            mCredits.Draw(gameTime, spriteBatch);
            mAchievements.Draw(gameTime, spriteBatch);
            mTechDemo.Draw(gameTime, spriteBatch);
            mExit.Draw(gameTime, spriteBatch);
            //foreach (var button in Buttons.Values)
            //{
            //    button.Draw(gameTime, mSpriteBatch);
            //}

        }
        public virtual void Update(GameTime gameTime)
        {
            mPreviousMouseState = mCurreMouseState;
            mCurreMouseState = Mouse.GetState();

            mMouseHovering = false;
            Console.WriteLine("not hovering");
            foreach (var button in mDButtons.Values)
            {
                if (button.returnRectangle().Contains(mCurreMouseState.X, mCurreMouseState.Y))
                {
                    mMouseHovering = true;
                    button.SetColor(button);
                    Console.WriteLine("isovering");
                }
            }
        }
    }
}

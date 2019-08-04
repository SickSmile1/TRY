using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TRY.GameStates.Structure;
using TRY.SaveGame;

namespace TRY.GameStates
{
    sealed class LevelScreen : States
    {
        private readonly List<Button> mLevelButtons;
        public readonly List<Attributes> mLevelAttributes;

        private readonly List<Button> mLvlUpCharacter;
        private readonly List<Button> mLvlUp;
        private static int OxygenCounter { get; set; }
        private static int EnergyCounter { get; set; }
        private static int CharacterCounter { get; set; }

        private MouseState mPreviousMouseState, mCurrentMouseState;
        private readonly Button mBack;
        private readonly Button mButOxygen;
        private readonly Button mButEnergy;
        private readonly Textures mTextures;
        private new readonly Game1 mGame;
        private readonly Button mFileStart;
        private readonly bool mFirstStart;
        private int mChosen;
        private readonly List<Button> mHudChoose;

        /// <summary>
        /// Loads all LevelStartButtons,
        /// has List of Buttons, UpgradeButtons 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="content"></param>
        /// <param name="load"></param>
        /// <param name="firstStart"></param>
        public LevelScreen(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, bool load = false, bool firstStart = false) : base(game, graphicsDevice, content)
        {
            OxygenCounter = 0;
            CharacterCounter = 0;
            EnergyCounter = 0;

            mFirstStart = firstStart;

            var rand = new Random();
            mTextures = new Textures(content);
            mGame = game;

            var width = (graphicsDevice.Viewport.Width / 14);

            mBack = new Button(50, 30, "Zurück");
            mFileStart = new Button(400, 850, "Wähle zwei Charaktere um das Spiel zu Starten");
            mFileStart.mButtonRectangle.Width = 400;

            mBack.Click += mBack_Click;
            mFileStart.Click += mFileStart_Click;

            mLevelAttributes = new List<Attributes>();
            mLevelButtons = new List<Button>();

            var widthList = new List<int> {510, 595, 425, 680, 510, 340, 765, 595, 425, 255, 850, 680, 510, 340, 170, 765, 595, 425, 255, 680, 510, 340, 595, 425, 510};
            var heightList = new List<int> {460, 410, 410, 360, 360, 360, 310, 310, 310, 310, 260, 260, 260, 260, 260, 210, 210, 210, 210, 160, 160, 160, 110, 110, 60};
            for (var i = 0; i < 25; i++)
            {
                mLevelButtons.Add(new Button(widthList[i] + 100, heightList[i]));

                mLevelAttributes.Add(new Attributes());
                mLevelButtons[i].mButtonRectangle.Width = 80;
                mLevelButtons[i].mButtonRectangle.Height = 80;
            }

            // set attributes for levels 50 en 10 ox 4 ch
            if (!load)
            {
                mGame.PlayerProgress.CurrentLevel = 0; 
                mLevelAttributes[0].Visited = true;

                while (OxygenCounter < 10)
                {
                    var l = rand.Next(1, 24);
                    var o = rand.Next(2);
                    mLevelAttributes[l].Oxygen = o;
                    OxygenCounter += o;
                }

                while (EnergyCounter < 50)
                {
                    var l = rand.Next(1, 24);
                    var o = rand.Next(5);
                    mLevelAttributes[l].Energy = o;
                    EnergyCounter += o;
                }

                while (CharacterCounter < 4)
                {
                    var l = rand.Next(1, 24);
                    var o = rand.Next(2);
                    CharacterCounter += o;
                    if (mLevelAttributes[l].Chamber > 0) CharacterCounter -= 1;
                    mLevelAttributes[l].Chamber = o;
                }
            }
            

            if (File.Exists(Directory.GetCurrentDirectory()+"\\level") || load)
            {
                var i = new LoadFromFile("level");
                mLevelAttributes = i.ReturnProps();
                foreach (var level in mLevelAttributes) level.Player = false;
                mLevelAttributes[mGame.PlayerProgress.CurrentLevel].Player = true;
            }

            mLevelAttributes[mGame.PlayerProgress.CurrentLevel].Player = true;

            int[] height = {100, 150, 200, 250, 300, 350};

            mLvlUp = new List<Button>();
            mLvlUpCharacter = new List<Button>();
            var charnum = 0;
            foreach (var chars in mGame.PlayerProgress.PlayerLevel.Keys)
            {
                if(game.PlayerProgress.PlayerLevel[chars][1] == 1)
                {
                    mLvlUpCharacter.Add(new Button(width + 80, height[charnum], game.PlayerProgress.PlayerLevel[chars][1].ToString() ) );
                    mLvlUpCharacter[charnum].Texture = chars;
                    mLvlUp.Add(new Button(width + 30, height[charnum]) );
                    mLvlUpCharacter[charnum].mButtonRectangle.Width = 40;
                    mLvlUpCharacter[charnum].mButtonRectangle.Height = 40;
                    mLvlUp[charnum].mButtonRectangle.Width = 40;
                    mLvlUp[charnum].mButtonRectangle.Height = 40;
                    charnum++;
                }
            }
            mButOxygen = new Button(width +30, 400, "Sauerstoff: "+mGame.PlayerProgress.PlayerOxygen);
            mButEnergy = new Button(width +30, 450, "Energie: "+mGame.PlayerProgress.PlayerEnergy);
            if (firstStart)
            {
                mHudChoose = new List<Button>()
                {
                    { new Button(200, 550, texture: "MaximusHud") },
                    { new Button(500, 550, texture: "WienseHud") },
                    { new Button(800, 550, texture: "BurkhaHud") }
                };
                foreach (var hud in mHudChoose)
                {
                    hud.mButtonRectangle.Width = 200;
                    hud.mButtonRectangle.Height = 250;
                }
            }
        }

        private void mFileStart_Click(object sender, EventArgs e)
        {
            if (mChosen <= 1) return;
            StateName = "LevelState";
            SaveStatetoFile.SaveStateFile(mGame, levelScreen: this);
            var counter = 0;
            foreach (var characters in mHudChoose)
            {
                if (counter == 2) break;
                if (Regex.IsMatch(characters.Texture, "Sel$"))
                {
                    mGame.PlayerProgress.PlayerLevel[characters.Texture.Substring(0, characters.Texture.Length - 6)][1]
                        = 1;
                    counter++;
                }
            }
            mGame.mState = new BattleMode(mGame, mGraphicsDevice, mContent, attribute: mLevelAttributes[0]);
            mGame.mScreenManager.AddScreen(mGame.mState);
        }

        private void mBack_Click(object sender, EventArgs e)
        {
            StateName = "LevelState";
            SaveStatetoFile.SaveStateFile(mGame, levelScreen: this);
            mGame.mScreenManager.RemoveScreen();
            mGame.mScreenManager.AddScreen(new MenuState(mGame, mGraphicsDevice, mContent));
            OxygenCounter = 0;
            CharacterCounter = 0;
            EnergyCounter = 0;
        }

        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTextures.GetTexture("Background"),
                             destinationRectangle: new Rectangle(0,0,
                             mGraphicsDevice.Viewport.Width,
                             mGraphicsDevice.Viewport.Height), Color.AliceBlue);

            foreach (var button in mLvlUpCharacter)
            {
                button.Draw(spriteBatch, mTextures, false, true);
            }

            foreach (var button in mLvlUp)
            {
                button.Draw(spriteBatch, mTextures, true);
            }

            for(var i = 0; i < mLevelButtons.Count; i++)
            {
                mLevelButtons[i].DrawLevels(spriteBatch, mTextures, mLevelAttributes[i]);
            }
            mLevelButtons[24].DrawLevels(spriteBatch, mTextures, mLevelAttributes[24], level: true);

            mBack.Draw(spriteBatch, mTextures);
            mButEnergy.Draw(spriteBatch, mTextures);
            mButOxygen.Draw(spriteBatch, mTextures);
            if (mFirstStart)
            {
                mFileStart.Draw(spriteBatch, mTextures);
                foreach (var button in mHudChoose)
                {
                    button.Draw(spriteBatch, mTextures, false, false, true);
                }
            }
        }
        
        private void StartNewBattle(int s)
        {
            StateName = "LevelState";
            SaveStatetoFile.SaveStateFile(mGame, levelScreen: this);
            mGame.mState = new BattleMode(mGame, mGraphicsDevice, mContent, attribute: mLevelAttributes[s], level: s);
            mGame.mScreenManager.AddScreen(mGame.mState);
            mGame.PlayerProgress.PlayerOxygen -= 1;
        }

        private void LooseCondition()
        {
            mGame.Achieved.FirstLoose = true;
            var i = new GameOver(mGame, mGraphicsDevice, mContent);
            mGame.mScreenManager.AddScreen(i);
        }

        private bool ButtonReachable(Point point)
        {
            var prevLevel = mLevelButtons[mGame.PlayerProgress.CurrentLevel].mButtonRectangle.Location;
            var vec = new Vector2(point.X, point.Y);
            var vec2 = new Vector2((float)prevLevel.X+40, (float)prevLevel.Y+40);
            var dist = vec - vec2;
            if (dist.Length() < 150)
            {
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            mCurrentMouseState = Mouse.GetState();
            var currentMousePoint = new Point(mCurrentMouseState.X, mCurrentMouseState.Y);
            var prevMousePoint = new Point(mPreviousMouseState.X, mPreviousMouseState.Y);
            mBack.Update();

            if (!mFirstStart)
            {
                if (mGame.PlayerProgress.PlayerOxygen == 0) LooseCondition();

                var index = 0;
                var names = new List<string>();
                foreach (var chats in mGame.PlayerProgress.PlayerLevel.Keys)
                {
                    if (mGame.PlayerProgress.PlayerLevel[chats][1] == 1) names.Add(chats);
                }
                foreach (var chars in names)
                {
                    if (mPreviousMouseState.LeftButton == ButtonState.Released &&
                        mCurrentMouseState.LeftButton == ButtonState.Pressed 
                        && mLvlUp[index].ReturnRectangle().Contains(currentMousePoint))
                    {
                        if (mGame.PlayerProgress.PlayerEnergy > 0)
                        {
                            Game1.sSoundEffectInstance[12].Stop();
                            Game1.sSoundEffectInstance[12].Play();
                            mGame.Achieved.UpgradedWeapon = true;
                            mGame.Achieved.EnergyUsed += 1;
                            mGame.PlayerProgress.PlayerLevel[chars][0] += 1;
                            mGame.PlayerProgress.PlayerEnergy -= 1;
                            mLvlUp[index].mText = mGame.PlayerProgress.PlayerLevel[chars].ToString();
                            mButEnergy.mText = "Energie: " + mGame.PlayerProgress.PlayerEnergy;
                        }
                    }
                    index++;
                }

            
                for (var i = 0; i<mLevelButtons.Count;i++)
                {
                    mLevelButtons[i].Update();
                    // hover update
                    if (mLevelButtons[i].ReturnRectangle().Contains(mCurrentMouseState.X, mCurrentMouseState.Y)
                        && !mLevelButtons[i].ReturnRectangle().Contains(mPreviousMouseState.X, mPreviousMouseState.Y))
                    {
                        mLevelButtons[i].SetColor(mLevelButtons[i]);
                    }
                    else if (!mLevelButtons[i].ReturnRectangle().Contains(currentMousePoint)
                             && mLevelButtons[i].ReturnRectangle().Contains(prevMousePoint))
                    {
                        mLevelButtons[i].SetColor(mLevelButtons[i]);
                    }
                    // next level/button choose
                    if (mPreviousMouseState.LeftButton == ButtonState.Released &&
                        mCurrentMouseState.LeftButton == ButtonState.Pressed && mLevelButtons[i].ReturnRectangle().Contains(currentMousePoint) &&
                        !mLevelAttributes[i].Visited && ButtonReachable(currentMousePoint))
                    {
                        mLevelAttributes[mGame.PlayerProgress.CurrentLevel].Player = false;
                        mLevelAttributes[i].Visited = true;
                        mLevelAttributes[i].Player = true;
                        mGame.PlayerProgress.CurrentLevel = i;
                        StartNewBattle(i);
                    }
                }
            }
            else
            {
                mFileStart.Update();
                for (int i = 0; i < 3; i++)
                {
                    if (mPreviousMouseState.LeftButton == ButtonState.Released &&
                        mCurrentMouseState.LeftButton == ButtonState.Pressed &&
                        mHudChoose[i].ReturnRectangle().Contains(currentMousePoint))
                    {
                        if (Regex.IsMatch(mHudChoose[i].Texture, "Sel$") || mChosen > 1) return;
                        mHudChoose[i].Texture = mHudChoose[i].Texture + "Sel";
                        mChosen++;
                    }
                    if (mPreviousMouseState.RightButton == ButtonState.Released &&
                        mCurrentMouseState.RightButton == ButtonState.Pressed &&
                        mHudChoose[i].ReturnRectangle().Contains(currentMousePoint))
                    {
                        if (Regex.IsMatch(mHudChoose[i].Texture, "Sel$"))
                        {
                            mHudChoose[i].Texture = mHudChoose[i].Texture.Substring(0, mHudChoose[i].Texture.Length-3);
                            if (mChosen >=1 ) mChosen--;
                        }
                    }
                }
            }
            
            mPreviousMouseState = mCurrentMouseState;
        }
    }
}

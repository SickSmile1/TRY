using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using QuakeConsole;
using TRY.GameStates;
using TRY.GameStates.Structure;
using TRY.SaveGame;

namespace TRY
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public sealed class Game1 : Game
    {
        public readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        public ConsoleComponent Console { get; }
        internal States mState;
        internal ManualInterpreter mInterpreter;


        public static readonly SoundEffectInstance[] sSoundEffectInstance = new SoundEffectInstance[14];

        public Progress PlayerProgress { get; set; }
        public Achieved Achieved { get; set; }


        internal readonly ScreenManager mScreenManager;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            mScreenManager = new ScreenManager();
            MediaPlayer.IsRepeating = true;

            Console = new ConsoleComponent(this);
            mInterpreter = new ManualInterpreter();
            Console.Interpreter = mInterpreter;
            Components.Add(Console);

            mGraphics.HardwareModeSwitch = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var s = Directory.GetCurrentDirectory();
            if (File.Exists(s + "\\progress.txt")) PlayerProgress = JsonConvert.DeserializeObject<Progress>(File.ReadAllText(s + "\\progress.txt"));
            else PlayerProgress = new Progress();
            if (File.Exists(s + "\\achieved.txt"))
            {
                LoadFromFile.LoadAchieved(this);
                //Achieved = JsonConvert.DeserializeObject<Achieved>(File.ReadAllText(s + "\\achieved.txt"));
            }
            else Achieved = new Achieved();
            mState = new MenuState(this, mGraphics.GraphicsDevice, Content);
            mScreenManager.AddScreen(mState);
            base.Initialize();

            mGraphics.PreferredBackBufferWidth = mGraphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            mGraphics.PreferredBackBufferHeight = mGraphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            mGraphics.IsFullScreen = true;

            mGraphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);
            // Load SoundEffect resource
            sSoundEffectInstance[0] = Content.Load<SoundEffect>("Music/mativve__walking-5").CreateInstance();
            sSoundEffectInstance[1] = Content.Load<SoundEffect>("Music/shout").CreateInstance();
            sSoundEffectInstance[2] = Content.Load<SoundEffect>("Music/Explosion").CreateInstance();
            sSoundEffectInstance[3] = Content.Load<SoundEffect>("Music/Piu").CreateInstance();
            sSoundEffectInstance[4] = Content.Load<SoundEffect>("Music/Gun").CreateInstance();
            sSoundEffectInstance[5] = Content.Load<SoundEffect>("Music/Sword").CreateInstance();
            sSoundEffectInstance[6] = Content.Load<SoundEffect>("Music/EndbossDeath").CreateInstance();
            sSoundEffectInstance[7] = Content.Load<SoundEffect>("Music/CharacterDeath").CreateInstance();
            sSoundEffectInstance[8] = Content.Load<SoundEffect>("Music/achievement").CreateInstance();
            sSoundEffectInstance[9] = Content.Load<SoundEffect>("Music/badumm").CreateInstance();
            sSoundEffectInstance[10] = Content.Load<SoundEffect>("Music/pickup").CreateInstance();
            sSoundEffectInstance[11] = Content.Load<SoundEffect>("Music/slamming").CreateInstance();
            sSoundEffectInstance[12] = Content.Load<SoundEffect>("Music/levelup").CreateInstance();
            sSoundEffectInstance[13] = Content.Load<SoundEffect>("Music/glassbreaking").CreateInstance();
            sSoundEffectInstance[0].IsLooped = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Do nothing
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            mScreenManager.ScreenUpdate(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        
        protected override void Draw(GameTime gameTime)
        {
            mSpriteBatch.Begin();
            mScreenManager.ScreenDraw(gameTime, mSpriteBatch);

            mSpriteBatch.End();
            base.Draw(gameTime: gameTime);
        }

    }
}
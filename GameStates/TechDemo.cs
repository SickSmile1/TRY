using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TRY.GameStates.Structure;
using TRY.Kampfmodus;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;
using TRY.Kampfmodus.Collision;
using TRY.Kampfmodus.Commands;
using TRY.Kampfmodus.Pathfinding;
using TRY.Kampfmodus.Structure;
using TRY.Kampfmodus.Weapons;

namespace TRY.GameStates
{
    internal class TechDemo : States
    {
        private SpriteBatch mSpriteBatch;
        private SpriteBatch mInterfaceBatch;

        //Textures and Resources
        private TextureManager mTextureManager;
        private UserInterface mUserInterface;
        private static readonly SoundEffectInstance[] sSoundEffectInstance = new SoundEffectInstance[14];
        private int mPlayerCount;
        private double mTime;

        private Pathfinding mPathfinding;

        // Class instances
        private BattleModeState BattleModeState { get; set; }
        private InputManager mInputManager;
        private Selection mSelection;
        private Camera mMainCamera;
        private CollisionManager mCollisionManager;
        private AddEnemy mAddEnemy;

        //Map neccessaire
        private Map mNewMap;
        private int mEnemys;

        private bool mSwitchedSides;
        private bool mShowPathfinding;

        // Number of button in level screen
        private int mLevel;

        public TechDemo(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            mGame = game;
            mGraphicsDevice = graphicsDevice;
            mContent = content;
            Initialize();
            var song = content.Load<Song>("Music/Fight");
            MediaPlayer.Play(song);
            ConsoleCommands.RegisterConsoleCommands(mGame.mInterpreter, BattleModeState, game, mAddEnemy);
            game.Achieved.StartedBattleMode = true;
        }

        void ToggleDrawingPathfinding()
        {
            mShowPathfinding = !mShowPathfinding;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        private void Initialize()
        {
            // create new camera object
            mInputManager = InputManager.Instance;
            LoadContent();
            mLevel = 26;
            mNewMap = new Map(mContent, mGraphicsDevice);
            mNewMap.GenerateMap(mLevel);
            mSwitchedSides = false;
            mShowPathfinding = false;

            QField<IStaticCollider> staticColliderField = new QField<IStaticCollider>(mNewMap.Height/2,mNewMap.Width/2,mNewMap.TileWidth*2);
            mCollisionManager = new CollisionManager(mNewMap,staticColliderField);
            BattleModeState = new BattleModeState(mCollisionManager,mNewMap, 26);
            mPathfinding = new Pathfinding(mNewMap.TiledMapObjects, new Point(20, 20), new Point(mNewMap.WidthInPixels, mNewMap.HeightInPixels), staticColliderField);
            mGame.mInterpreter.RegisterCommand("win", args => Win());
            mGame.mInterpreter.RegisterCommand("loose", args => RemoveEnemies());
            mGame.mInterpreter.RegisterCommand("letsbefriends", (v => SwitchAllCharacters()));
            mGame.mInterpreter.RegisterCommand("showPaths", (v => ToggleDrawingPathfinding()));
            mSelection = new Selection(BattleModeState, new SelectionBox(mTextureManager.GetTexture("DottedLine")), true);
            mUserInterface = new UserInterface(
                BattleModeState,
                mSelection,
                mTextureManager.GetTexture("PortraitInactive"),
                mTextureManager.GetFont("Font"));
            mInterfaceBatch = new SpriteBatch(mGraphicsDevice);
            mSpriteBatch = new SpriteBatch(mGraphicsDevice);
            mMainCamera = new Camera(mGraphicsDevice.Viewport, mNewMap.WidthInPixels, mNewMap.HeightInPixels);
            mAddEnemy = new AddEnemy(BattleModeState, mPathfinding, mCollisionManager);
            
            InitializePlayers();
            AddEnemys();
            foreach (var enemyCharacter in BattleModeState.GetEnemyCharacters())
            {
                enemyCharacter.UpdateCommand(new GoNearCommand(enemyCharacter, new Point(118,2052), 100));
            }
        }

        /// <summary>
        /// Initializes Players at spawn
        /// </summary>
        private void InitializePlayers()
        {
            var s = 0;
            var ids = new List<string>();
            foreach (var name in mGame.PlayerProgress.PlayerLevel.Keys)
            {
                ids.Add(name);
            }

            var t = new List<Point> { new Point(50, 50), new Point(50, 80), new Point(50, 100), new Point(50, 130), new Point(50, 160), new Point(50, 190)};
            foreach (var k in t)
            {
                var c = new Character("Astronaut", k, new Point(20, 43), mPathfinding);
                mPlayerCount += 1;
                BattleModeState.AddPlayerCharacter(c);
                c.Id = ids[s];
                mGame.PlayerProgress.PlayerLevel[ids[s]][1]  = 1;
                s++;
            }

            var characters = BattleModeState.GetPlayerCharacters();
            foreach (var chars in characters)
            {
                if (chars.Id == "Burkha")
                    ShieldCharacter(chars);
                else if (chars.Id == "Vut")
                    RifleCharacter(chars);
                else if (chars.Id == "Maximus")
                    LaserCharacter(chars);
                else if (chars.Id == "Wiense")
                    MeleeChar(chars);
                else if (chars.Id == "Ngol")
                    SwordCharacter(chars);
                else if (chars.Id == "Domogas") IncreaseChar(chars);
            }

        }

        private void RemoveEnemies()
        {
            var i = BattleModeState.GetPlayerCharacters();
            foreach (var c in i)
            {
                c.Health = 0;
            }
        }


        private void SwitchAllCharacters()
        {
            if (!mSwitchedSides)
            {
                mSwitchedSides = true;
                var players = BattleModeState.GetPlayerCharacters();
                foreach (var enemy in BattleModeState.GetEnemyCharacters())
                {
                    BattleModeState.ToggleTeams(enemy);
                    enemy.AbortCommand();
                    enemy.Destination = null;
                }
            }
            
        }

        private void IncreaseChar(ICharacter c)
        {
            var level = mGame.PlayerProgress.PlayerLevel["Domogas"][0];
            if (mGame.PlayerProgress.PlayerLevel["Domogas"][1] != 1) return;
            c.Weapon = new IncreaseStrength("Circle", BattleModeState.FindCharactersInRadius, true);
            c.Id = "Domogas";
            c.Texture = "Domogas";
            c.CharacterAnimation.TextureIdentifier = "Domogas";
            c.Weapon.Damage += (c.Weapon.Damage / 10)* level;
            c.SupportAbility = new ReviveCharacter(BattleModeState.FindNearestDeadCharacter);
            c.Ability = new MindControl(BattleModeState.FindNearestCharacter,
                                        BattleModeState.ToggleTeams);
            c.Ability.Duration += (c.Ability.Duration / 10) * level;
        }


        private void MeleeChar(ICharacter c)
        {
            var level = mGame.PlayerProgress.PlayerLevel["Wiense"][0];
            if (mGame.PlayerProgress.PlayerLevel["Wiense"][1] != 1) return;
            c.Weapon = new Melee(BattleModeState.FindNearestCharacter, true, 32, 2, 200, true);
            c.Id = "Wiense";
            c.CharacterAnimation.TextureIdentifier = "Wiense";
            c.Texture = "Wiense";
            c.Weapon.Damage += (c.Weapon.Damage / 10) * level;
            c.Ability = new Emp(BattleModeState.FindCharactersInRadius, BattleModeState.ToggleActive);
            c.Ability.Radius += (c.Ability.Radius / 20) * level;
        }

        private void LaserCharacter(ICharacter c)
        {
            var level = mGame.PlayerProgress.PlayerLevel["Maximus"][0];
            if (mGame.PlayerProgress.PlayerLevel["Maximus"][1] != 1) return;
            c.Weapon = new Rifle("Laser",
                true,
                BattleModeState.FindNearestCharacterInRadius,
                BattleModeState.AddProjectile, projectileSize: new Point(20,4));
            c.Weapon.Damage += 50;
            c.Ability = new Distraction("Rabbit", BattleModeState.AddDistractionObject);
            c.CharacterAnimation.TextureIdentifier = "Maximus";
            c.Texture = "Maximus";
            c.Ability.Radius += (c.Ability.Radius / 20) * level;
        }

        private void ShieldCharacter(ICharacter c)
        {
            var level = mGame.PlayerProgress.PlayerLevel["Burkha"][0];
            if (mGame.PlayerProgress.PlayerLevel["Burkha"][1] != 1) return;
            c.Weapon = new Shield("Shield", "HealthBarShield",
                BattleModeState.FindNearestCharacter, true, BattleModeState.Projectiles);
            c.Id = "Burkha";
            c.Texture = "Burkha";
            c.CharacterAnimation.TextureIdentifier = "Burkha";
            c.SupportAbility = new ReviveCharacter(BattleModeState.FindNearestDeadCharacter);
            c.Ability = new ShieldExplosion(BattleModeState.FindCharactersInRadius);
            c.Weapon.MaxShieldDamage += (c.Weapon.MaxShieldDamage / 10) * level;
            c.Ability.Damage += (c.Ability.Damage / 10)* level;
        }

        private void SwordCharacter(ICharacter c)
        {
            var level = mGame.PlayerProgress.PlayerLevel["Ngol"][0];
            if (mGame.PlayerProgress.PlayerLevel["Ngol"][1] != 1) return;
            c.Weapon = new Melee(BattleModeState.FindNearestCharacter, true, 32, 2, 200);
            c.Id = "Ngol";
            c.CharacterAnimation.TextureIdentifier = "Ngol";
            c.Texture = "Ngol";
            c.Weapon.Damage += (c.Weapon.Damage / 10) * level;
            c.Ability = new RoundKick(BattleModeState.FindCharactersInRadius);
            c.Ability.Radius += (c.Ability.Radius / 20) * level;
        }

        private void RifleCharacter(ICharacter c)
        {
            int level = mGame.PlayerProgress.PlayerLevel["Vut"][0];
            if (mGame.PlayerProgress.PlayerLevel["Vut"][1] == 1)
            {
                c.Weapon = new Rifle(
                    "Projectile",
                    true,
                    BattleModeState.FindNearestCharacterInRadius,
                    BattleModeState.AddProjectile);
                c.Id = "Vut";
                c.Texture = "Vut";
                c.CharacterAnimation.TextureIdentifier = "Vut";
                c.Ability = new Mines("Mine", BattleModeState.AddMineObject, BattleModeState.FindCharactersInRadius,true);
                c.Weapon.Damage += 50;
                c.Ability.Damage += (c.Ability.Damage / 10) * level;
                c.Ability.Radius += (c.Ability.Radius / 10) * level;
            }
        }

        private bool CheckLooseCondition()
        {
            var i = 0;
            foreach (var ch in BattleModeState.GetPlayerCharacters())
            {
                if (ch.Health <= 0) i++;
            }

            if (i == BattleModeState.GetPlayerCharacters().Count()) return true;
            return false;
        }

        private void AddEnemys()
        {
            Formation.CreateTech(mAddEnemy);
            // Total number of spawned enemies.
            mEnemys += 990;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        private void LoadContent()
        {
            
            mTextureManager = new TextureManager(mContent, mGraphicsDevice);

            // Load SoundEffect resource
            sSoundEffectInstance[0] = mContent.Load<SoundEffect>("Music/mativve__walking-5").CreateInstance();
            sSoundEffectInstance[1] = mContent.Load<SoundEffect>("Music/shout").CreateInstance();
            sSoundEffectInstance[2] = mContent.Load<SoundEffect>("Music/Explosion").CreateInstance();
            sSoundEffectInstance[3] = mContent.Load<SoundEffect>("Music/Piu").CreateInstance();
            sSoundEffectInstance[4] = mContent.Load<SoundEffect>("Music/Gun").CreateInstance();
            sSoundEffectInstance[5] = mContent.Load<SoundEffect>("Music/Sword").CreateInstance();
            sSoundEffectInstance[6] = mContent.Load<SoundEffect>("Music/EndbossDeath").CreateInstance();
            sSoundEffectInstance[7] = mContent.Load<SoundEffect>("Music/CharacterDeath").CreateInstance();
            sSoundEffectInstance[8] = mContent.Load<SoundEffect>("Music/achievement").CreateInstance();
            sSoundEffectInstance[9] = mContent.Load<SoundEffect>("Music/badumm").CreateInstance();
            sSoundEffectInstance[10] = mContent.Load<SoundEffect>("Music/pickup").CreateInstance();
            sSoundEffectInstance[11] = mContent.Load<SoundEffect>("Music/slamming").CreateInstance();
            sSoundEffectInstance[12] = mContent.Load<SoundEffect>("Music/levelup").CreateInstance();
            sSoundEffectInstance[13] = mContent.Load<SoundEffect>("Music/glassbreaking").CreateInstance();
            sSoundEffectInstance[0].IsLooped = true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mGraphicsDevice.Clear(Color.Black);
            mInterfaceBatch.Begin();
            mSpriteBatch.Begin(transformMatrix: mMainCamera.Transform);
            mNewMap.Draw(mMainCamera, mLevel);

            BattleModeState.Draw(mSpriteBatch, mTextureManager);

            mSelection.SelectionBox.Draw(mSpriteBatch);
            mSelection.mLine.Draw(mSpriteBatch,mTextureManager);
            if(mShowPathfinding) mPathfinding.Draw(mSpriteBatch, mTextureManager.GetTexture("schieße"), mTextureManager.GetTexture("schieße"));
            if(!mSwitchedSides) mUserInterface.Draw(mGraphicsDevice, mInterfaceBatch, mTextureManager);

            mSpriteBatch.End();

            mInterfaceBatch.End();
        }

        private void Win()
        {
            mGame.Achieved.Timer = mTime;
            CurrentEnemyAlive();
            if (mGame.Console.IsVisible) mGame.Console.ToggleOpenClose();
        }

        private void CurrentPlayerAlive()
        {
            var players = BattleModeState.GetPlayerCharacters();
            var ids = players.Select(ident => ident.Id).ToList();
            if (ids.Count >= mPlayerCount) return;
            foreach (var chars in mGame.PlayerProgress.PlayerLevel.Keys)
            {
                if (!ids.Contains(chars))
                    mGame.PlayerProgress.PlayerLevel[chars][1] = 0;
            }
            mPlayerCount = ids.Count;
        }

        private void CurrentEnemyAlive()
        {
            var count = BattleModeState.GetEnemyCharacters();
            if (count.Count < mEnemys)
            {
                mGame.Achieved.KilledEnemys += (mEnemys-count.Count);
                mEnemys = mGame.Achieved.KilledEnemys;
            }
        }

        /// <summary>
        /// Updates the main action of the Game
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            CurrentPlayerAlive();
            mNewMap.Update(gameTime, mLevel);
            mInputManager.Update(mMainCamera, mUserInterface, mGame,
                mGraphicsDevice, mContent, BattleModeState, mSelection);
            if (mGame.IsActive) mInputManager.Update(mMainCamera, mUserInterface, mGame, mGraphicsDevice, 
                mContent, BattleModeState, mSelection);
            BattleModeState.Update(gameTime);
            mCollisionManager.Update();
            
            mTime += gameTime.ElapsedGameTime.Milliseconds / 1000d;

            if (!CheckLooseCondition()) return;

            if (mGame.Console.IsVisible) mGame.Console.ToggleOpenClose();
            mGame.mState = new GameOver(mGame, mGraphicsDevice, mContent);
            mGame.mScreenManager.AddScreen(mGame.mState);
        }
    }
}

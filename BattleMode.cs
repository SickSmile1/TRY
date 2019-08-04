using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;
using TRY.GameStates;
using TRY.GameStates.Structure;
using TRY.Kampfmodus;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;
using TRY.SaveGame;
using TRY.Kampfmodus.Collision;
using TRY.Kampfmodus.Pathfinding;
using TRY.Kampfmodus.Structure;
using TRY.Kampfmodus.Weapons;

namespace TRY
{
    internal class BattleMode : States
    {
        private SpriteBatch mSpriteBatch;
        private SpriteBatch mInterfaceBatch;

        //Textures and Resources
        private TextureManager mTextureManager;
        private UserInterface mUserInterface;
        private int mPlayerCount;
        internal double mTime;

        private Pathfinding mPathfinding;

        // Class instances
        public BattleModeState BattleModeState { get; private set; }
        private InputManager mInputManager;
        private Selection mSelection;
        private Camera mMainCamera;
        private CollisionManager mCollisionManager;
        private AddEnemy mAddEnemy;

        // Map neccessaire
        private Map mNewMap;
        private Random mRandom;
        private TiledMapObjectLayer ObjectLayer { get; set; }

        private TiledMapObjectLayer SpawnLayer { get; set; }
        private int mEnemys;

        // Alle Variablen für die linken und rechten Türen. Die linke Tür an Position 0 passt 
        // zur rechten Tür an position 0.
        private readonly bool mLoad;
        private readonly Attributes mAttributes;

        // Number of button in level screen
        private int mLevel;

        //Just for Cheat
        private bool mSwitchedSides;
        private bool mShowPathfinding;

        private EnemySpawns mEnemySpawns;
        private bool mReviving;

        public BattleMode(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, bool load = false,
            Attributes attribute = null, int level = 0) : base(game, graphicsDevice, content)
        {
            mSwitchedSides = false;
            MediaPlayer.Volume = 0.4f;
            mAttributes = attribute;
            mRandom = new Random();
            mGame = game;
            mLevel = level;
            mGraphicsDevice = graphicsDevice;
            mContent = content;
            mLoad = load;
            Initialize();
            var song = content.Load<Song>("Music/Fight");
            MediaPlayer.Play(song);
            ConsoleCommands.RegisterConsoleCommands(mGame.mInterpreter, BattleModeState, game, mAddEnemy);
            game.Achieved.StartedBattleMode = true;
            if (mSwitchedSides)
            {
                SwitchAllCharacters();
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        private void Initialize()
        {
            mInputManager = InputManager.Instance;
            LoadContent();
            
            //Initialise Map
            mNewMap = new Map(mContent, mGraphicsDevice);
            mNewMap.GenerateMap(mLevel, mLoad);

            var objectLayer = mNewMap.Objects(mLevel);
            if (mLevel != 24)
            {
                SpawnLayer = mNewMap.EnemyStart();
            }
            objectLayer.IsVisible = false;
            ObjectLayer = objectLayer;

            QField<IStaticCollider> staticColliderField = new QField<IStaticCollider>(mNewMap.Height/2,mNewMap.Width/2, mNewMap.TileWidth*2);
            mCollisionManager = new CollisionManager(mNewMap, staticColliderField);
            BattleModeState = new BattleModeState(mCollisionManager,mNewMap, mLevel);
            mPathfinding = new Pathfinding(mNewMap.TiledMapObjects, new Point(20, 20), new Point(mNewMap.WidthInPixels, mNewMap.HeightInPixels), staticColliderField);
            mGame.mInterpreter.RegisterCommand("win", args => Win());
            mGame.mInterpreter.RegisterCommand("loose", args => RemoveEnemies());
            mGame.mInterpreter.RegisterCommand("showPaths", (v => ToggleDrawingPathfinding()));
            mSelection = new Selection(BattleModeState, new SelectionBox(mTextureManager.GetTexture("DottedLine")));
            mUserInterface = new UserInterface(
                BattleModeState,
                mSelection,
                mTextureManager.GetTexture("PortraitInactive"),
                mTextureManager.GetFont("Font"));
            mInterfaceBatch = new SpriteBatch(mGraphicsDevice);
            mSpriteBatch = new SpriteBatch(mGraphicsDevice);
            mMainCamera = new Camera(mGraphicsDevice.Viewport, mNewMap.WidthInPixels, mNewMap.HeightInPixels);
            mAddEnemy = new AddEnemy(BattleModeState, mPathfinding, mCollisionManager);
            mEnemySpawns = new EnemySpawns(mNewMap.EnemySpawns, mAddEnemy);
        

            if (!mLoad)
            {
                InitializePlayers();
                InitializeDoors();
                InitializeCollectable();
                CryoChamber();

                if (mLevel < 24)
                {
                    AddEnemys();
                }
                
                mEnemys = BattleModeState.GetEnemyCharacters().Count;
            }
            else
            {
                var load = new LoadFromFile("battle");


                var chars = load.ReturnChar();
                if (chars.Count > 0) LoadPlayer(chars);

                InitializeDoors();

                chars = load.ReturnEnemy();
                if (chars.Count > 0) LoadEnemy(chars);

                chars = load.ReturnDoors();
                if (chars.Count > 0) LoadDoors(chars);

                chars = load.ReturnOxEn();

                if (chars.Count > 0) LoadOxEn(chars);

                chars = load.ReturnCryo();
                if (chars.Count > 0) CryoChamber();
                
                mEnemys = BattleModeState.GetEnemyCharacters().Count;
            }
            if (mLevel == 24)
            {
                foreach (var obj in ObjectLayer.Objects)
                {
                    if (obj.Name == "EndbossSpawn")
                    {
                        var c = new Endboss("Boss", obj.Position.ToPoint(), mPathfinding,
                            BattleModeState.FindNearestCharacter, BattleModeState.AddProjectile, BattleModeState.RemoveProjectile, "Projectile");
                        BattleModeState.AddNpc(c);
                    }
                }
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
        void ToggleDrawingPathfinding()
        {
            mShowPathfinding = !mShowPathfinding;
        }

        private void RemoveEnemies()
        {
            var i = BattleModeState.GetPlayerCharacters();
            foreach (var c in i)
            {
                c.Health = 0;
            }
        }

        private void CryoChamber()
        {

            if (mAttributes == null || mAttributes.Chamber <= 0) return;
            var index = mRandom.Next(mNewMap.ItemPosition.Count);
            var cryo = new Character("Astronaut",
                new Point((int) mNewMap.ItemPosition[index].X,
                    (int) mNewMap.ItemPosition[index].Y),
                new Point(20, 43), mPathfinding);
            BattleModeState.AddCryoChamber(new CryoChamber("CryoChamber", new Vector2(60, 90),
                100, 20, cryo));
            mNewMap.ItemPosition.RemoveAt(index);
        }

        private void LoadOxEn(List<string> oxen)
        {
            BattleModeState.Oxygen = Convert.ToInt32(oxen[0]);
            BattleModeState.Energy = Convert.ToInt32(oxen[1]);
            InitializeCollectable();
        }

        private void LoadDoors(List<string> door)
        {
            var i = 0;
            foreach (var doors in BattleModeState.Doors)
            {
                if (Convert.ToBoolean(door[i])) continue;
                doors.Open();
                i++;
            }
        }

        private void LoadPlayer(List<string> chars)
        {
            mPlayerCount = 0;
            for (var i = 0; i < chars.Count; i += 5)
            {
                var x = Convert.ToInt32(chars[i]);
                var y = Convert.ToInt32(chars[i + 1]);
                var c = new Character("Astronaut", new Point(x, y), new Point(20, 43), mPathfinding);
                c.Health = Convert.ToInt32(chars[i + 2]);
                BattleModeState.AddPlayerCharacter(c);
                mGame.PlayerProgress.PlayerLevel[chars[i + 3]][1] = 1;
                if (chars[i + 3] == "Burkha")
                    ShieldCharacter(c);
                else if (chars[i + 3] == "Vut")
                    RifleCharacter(c);
                else if (chars[i + 3] == "Maximus")
                    LaserCharacter(c);
                else if (chars[i + 3] == "Wiense")
                    MeleeChar(c);
                else if (chars[i + 3] == "Ngol")
                    SwordCharacter(c);
                else if (chars[i + 3] == "Domogas") IncreaseChar(c);

                mPlayerCount += 1;
            }
        }

        private void NewCharacter(ICharacter ch)
        {
            var dead = new List<string>();
            foreach (var chs in mGame.PlayerProgress.PlayerLevel.Keys)
            {
                if (mGame.PlayerProgress.PlayerLevel[chs][1] == 0) dead.Add(chs);
            }
            BattleModeState.AddPlayerCharacter(ch);
            var s = mRandom.Next(dead.Count);

            mGame.PlayerProgress.PlayerLevel[dead[s]][1]  = 1;
            mPlayerCount += 1;

            if (dead[s] == "Burkha")
                ShieldCharacter(ch);
            else if (dead[s] == "Vut")
                RifleCharacter(ch);
            else if (dead[s] == "Maximus")
                LaserCharacter(ch);
            else if (dead[s] == "Wiense")
                MeleeChar(ch);
            else if (dead[s] == "Ngol")
                SwordCharacter(ch);
            else if (dead[s] == "Domogas") IncreaseChar(ch);
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
            c.Health = 75;
            c.MaxHealth = 75;
            c.Health += (c.Health / 20)* level;
            c.MaxHealth += (c.MaxHealth / 20)* level;
            c.SupportAbility = new ReviveCharacter(BattleModeState.FindNearestDeadCharacter);
            c.Ability = new MindControl(BattleModeState.FindNearestCharacter,
                                        BattleModeState.ToggleTeams);
            c.Ability.Duration += (c.Ability.Duration / 10) * level;
        }


        private void MeleeChar(ICharacter c)
        {
            var level = mGame.PlayerProgress.PlayerLevel["Wiense"][0];
            if (mGame.PlayerProgress.PlayerLevel["Wiense"][1] != 1) return;
            c.Weapon = new Melee(BattleModeState.FindNearestCharacter, true, 130, 1, 20, true);
            c.Id = "Wiense";
            c.CharacterAnimation.TextureIdentifier = "Wiense";
            c.Texture = "Wiense";
            c.Weapon.Damage += (c.Weapon.Damage / 10) * level;
            c.Health = 120;
            c.MaxHealth = 120;
            c.Health += (c.Health / 10)* level;
            c.MaxHealth += (c.MaxHealth / 10)* level;
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
            c.Weapon.Damage += (c.Weapon.Damage / 10) * level;
            c.Health += (c.Health / 20)* level;
            c.MaxHealth += (c.MaxHealth / 20)* level;
            c.Ability = new Distraction("Rabbit", BattleModeState.AddDistractionObject);
            c.CharacterAnimation.TextureIdentifier = "Maximus";
            c.Id = "Maximus";
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
            c.Health = 75;
            c.MaxHealth = 75;
            c.Health += (c.Health / 20)* level;
            c.MaxHealth += (c.MaxHealth / 20)* level;
            c.Weapon.MaxShieldDamage += (c.Weapon.MaxShieldDamage / 10) * level;
            c.Ability.Damage += (c.Ability.Damage / 10)* level;
        }

        private void SwordCharacter(ICharacter c)
        {
            var level = mGame.PlayerProgress.PlayerLevel["Ngol"][0];
            if (mGame.PlayerProgress.PlayerLevel["Ngol"][1] != 1) return;
            c.Weapon = new Melee(BattleModeState.FindNearestCharacter, true, 130, 1, 20);
            c.Id = "Ngol";
            c.CharacterAnimation.TextureIdentifier = "Ngol";
            c.Texture = "Ngol";
            c.Weapon.Damage += (c.Weapon.Damage / 10) * level;
            c.Health = 120;
            c.MaxHealth = 120;
            c.Health += (c.Health / 10)* level;
            c.MaxHealth += (c.MaxHealth / 10)* level;
            c.Ability = new RoundKick(BattleModeState.FindCharactersInRadius);
            c.Ability.Damage += (c.Ability.Damage / 10) * level;
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
                c.Weapon.Damage += (c.Ability.Damage / 10) * level;
                c.Health += (c.Health / 20)* level;
                c.MaxHealth += (c.MaxHealth / 20)* level;
                c.Ability.Damage += (c.Ability.Damage / 10) * level;
                c.Ability.Radius += (c.Ability.Radius / 20) * level;
            }
        }

        private void LoadEnemy(List<string> chars)
        {
            for (var i = 0; i < chars.Count; i += 5)
            {
                var x = Convert.ToInt32(chars[i]);
                var y = Convert.ToInt32(chars[i + 1]);
                if (chars[i + 3] is "Explosiv")
                {
                    mAddEnemy.AddExplosive(new Point(x, y));
                }
                else if (chars[i + 3] is "RangedEnemy")
                {
                    mAddEnemy.AddRangedEnemy(new Point(x, y));
                }
                else if (chars[i + 3] is "MeleeEnemy")
                {
                    mAddEnemy.AddMeleeEnemy(new Point(x, y));
                }
                else if (chars[i + 3] is "Supervisor")
                {
                    mAddEnemy.AddMeleeEnemy(new Point(x, y));
                }
                mEnemys += 1;
            }
            BattleModeState.SetKiActions();
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
            
            foreach (var obj in SpawnLayer.Objects)
            {
                Formation.Create(mAddEnemy, (int)obj.Position.X, (int)obj.Position.Y);
            }
            // Total number of spawned enemies.
            mEnemys += 10;
            BattleModeState.SetKiActions();
            // Total number of spawned enemies.
            mEnemys += 10;
            BattleModeState.SetKiActions();
        }

        private void InitializeCollectable()
        {
            if (mAttributes != null && mAttributes.Energy > 0)
            {
                for (var i = 0; i < mAttributes.Energy; i++)
                {
                    var index = mRandom.Next(mNewMap.ItemPosition.Count);
                    BattleModeState.AddItem(new CollectableItem("Energy", true, mNewMap.ItemPosition[index]));
                    mNewMap.ItemPosition.RemoveAt(index);
                    
                }
            }

            if (mAttributes != null && mAttributes.Oxygen > 0)
            {
                var index = mRandom.Next(mNewMap.ItemPosition.Count);
                BattleModeState.AddItem(new CollectableItem("Oxygen", false, mNewMap.ItemPosition[index]));
                mNewMap.ItemPosition.RemoveAt(index);
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
                if (mGame.PlayerProgress.PlayerLevel[name][1] == 1) ids.Add(name);
            }
            foreach (var obj in ObjectLayer.Objects)
            {
                if (obj.Name == "Spawn" && s < ids.Count)
                {
                    var c = new Character("Astronaut", obj.Position.ToPoint(), new Point(20, 43), mPathfinding);
                    mPlayerCount += 1;
                    BattleModeState.AddPlayerCharacter(c);
                    c.Id = ids[s];
                    mGame.PlayerProgress.PlayerLevel[ids[s]][1]  = 1;
                    s++;
                }
                // Attempt to center camera around characters at spawn
                mMainCamera.MoveCamera(new Vector2(obj.Position.ToPoint().X - mMainCamera.mViewport.Width, obj.Position.ToPoint().Y + mMainCamera.mViewport.Height));
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

        /// <summary>
        /// Initializes Doors
        /// </summary>
        private void InitializeDoors()
        {
            mNewMap.GetDoors(mTextureManager, BattleModeState, mCollisionManager);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        private void LoadContent()
        {
            
            mTextureManager = new TextureManager(mContent, mGraphicsDevice);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mGraphicsDevice.Clear(Color.Black);
            mInterfaceBatch.Begin();
            mSpriteBatch.Begin(transformMatrix: mMainCamera.Transform);
            mNewMap.Draw(mMainCamera, mLevel);
            BattleModeState.Draw(mSpriteBatch, mTextureManager);

            // Drawing the selection box, mBattleModeState needed for the WorldtoScreen method, which will set the coordinates correctly. 
            mSelection.SelectionBox.Draw(mSpriteBatch);
            mSelection.mLine.Draw(mSpriteBatch,mTextureManager);
            mUserInterface.Draw(mGraphicsDevice, mInterfaceBatch, mTextureManager);
            if (mShowPathfinding) mPathfinding.Draw(mSpriteBatch, mTextureManager.GetTexture("schieße"), mTextureManager.GetTexture("schieße"));
            mSpriteBatch.End();
            mInterfaceBatch.End();
        }

        private bool CheckWinCondition()
        {
            var winZone = mNewMap.GetWinZone(mLevel);
            var winRectangle = new Rectangle((int)winZone.Position.X, (int)winZone.Position.Y, (int)winZone.Size.Height, (int)winZone.Size.Width);
            var numPlayerInWinRoom = BattleModeState.GetPlayerCharacters().Count(playerCharacter => playerCharacter.CharacterArea.Intersects(winRectangle));

            return numPlayerInWinRoom >= BattleModeState.GetPlayerCharacters().Count();
        }

        private void Win()
        {
            mGame.Achieved.LevelPlayed += 1;
            mGame.Achieved.Timer += mTime;
            CurrentEnemyAlive();
            if (mGame.Console.IsVisible) mGame.Console.ToggleOpenClose();
            string s = Directory.GetCurrentDirectory(); 
            if (mGame.PlayerProgress.CurrentLevel == 24)
            {
                var i = new WinScreen(mGame, mGraphicsDevice, mContent);
                mGame.mState = i;
                mGame.mScreenManager.AddScreen(mGame.mState);
            }
            else if (!File.Exists(s + "\\level.txt"))
            {
                SaveStatetoFile.SaveStateFile(mGame, battleMode: BattleModeState);
                var i = new LevelScreen(mGame, mGraphicsDevice, mContent);
                mGame.mState = i;
                mGame.mScreenManager.AddScreen(mGame.mState);
            }
            else
            {
                mGame.mScreenManager.RemoveScreen();
                SaveStatetoFile.SaveStateFile(mGame, battleMode: BattleModeState);
                var i = new LevelScreen(mGame, mGraphicsDevice, mContent, true);
                mGame.mState = i;
                mGame.mScreenManager.AddScreen(mGame.mState);
            }
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

        public void CurrentEnemyAlive()
        {
            var count = BattleModeState.GetEnemyCharacters();
            if (count.Count < mEnemys)
            {
                mGame.Achieved.KilledEnemys += (mEnemys-count.Count);
                mEnemys = mGame.Achieved.KilledEnemys;
            }
        }

        private void IncrementEnergy(bool energy)
        {
            if (energy) mGame.PlayerProgress.PlayerEnergy += 1;
            else
            {
                mGame.PlayerProgress.PlayerOxygen += 1;
                mGame.Achieved.Oxygen += 1;
            }
        }

        /// <summary>
        /// Updates the main action of the Game
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (mGame.Achieved.PlayerCount > BattleModeState.GetPlayerCharacters().Count) mReviving = true;
            var e = Tuple.Create(BattleModeState.Energy, BattleModeState.Oxygen);
            CurrentPlayerAlive();
            mNewMap.Update(gameTime, mLevel);
            mEnemySpawns.Update(gameTime, (float)mTime, mLevel, BattleModeState, mLevel);
            mEnemys += mEnemySpawns.EnemiesSpawned;
            mInputManager.Update(mMainCamera, mUserInterface, mGame,
                mGraphicsDevice, mContent, BattleModeState, mSelection);
            if (mGame.IsActive) mInputManager.Update(mMainCamera, mUserInterface, mGame, mGraphicsDevice, 
                mContent, BattleModeState, mSelection);
            BattleModeState.Update(gameTime);
            mCollisionManager.Update();
            
            mTime += gameTime.ElapsedGameTime.Milliseconds / 1000d;

            if (CheckWinCondition()) Win();
            if (BattleModeState.Energy > e.Item1) IncrementEnergy(true);
            if (BattleModeState.Oxygen > e.Item2) IncrementEnergy(false);

            if (BattleModeState.CryoChambers.Count > 0 && BattleModeState.CryoChambers[0].Create)
            {
                NewCharacter(BattleModeState.CryoChambers[0].mNewCharacter);
                BattleModeState.CryoChambers[0].Create = false;
                mGame.Achieved.PlayerCount += 1;
            }

            if (mGame.Achieved.PlayerCount == BattleModeState.GetPlayerCharacters().Count && mReviving)
            {
                mGame.Achieved.Revived += 1;
            }
            if (!CheckLooseCondition()) return;

            mGame.Achieved.Timer += mTime;
            if (mGame.PlayerProgress.CurrentLevel == 0) mGame.Achieved.LostRoundOne = true;
            if (mGame.Console.IsVisible) mGame.Console.ToggleOpenClose();
            mGame.mState = new GameOver(mGame, mGraphicsDevice, mContent);
            mGame.mScreenManager.AddScreen(mGame.mState);
        }
    }
}

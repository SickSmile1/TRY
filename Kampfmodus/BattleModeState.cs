using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.AI;
using TRY.Kampfmodus.Characters;
using TRY.Kampfmodus.Collision;
using TRY.Kampfmodus.Graphics;
using TRY.Kampfmodus.Weapons;

namespace TRY.Kampfmodus
{
    /// <summary>
    /// Saves all relevant game state information, tracks all characters
    /// </summary>
    sealed class BattleModeState
    {
        private readonly List<Explode> mExplodedCharacters;
        public readonly HashSet<ICharacter> mPlayerCharacters;
        private readonly CollisionManager mCollisionManager;
        private HealthBar mHealthBar;
        private int mLevel;

        public delegate void ProjectileFunction(Projectile x);

        public delegate ICharacter FindCharacter(Vector2 pos, bool player);

        public delegate List<ICharacter> CharactersInRadius(Vector2 pos, float radius, bool player);
        public delegate ICharacter CharacterInRadius(Vector2 pos, float radius, bool player);

        public delegate void PlaceRabbit(DistractionObject rabbit);
        public delegate void PlaceMine(MineObjects mine);
        public delegate void TogglePlayer(ICharacter c);
        public delegate void FreezeCharacters(List<ICharacter> characters, bool unfreeze);
        public delegate void ExplodedCharacter(Explode explode, ICharacter character);

        private List<ICharacter> NpCharacters { get; }
        public HashSet<Projectile> Projectiles { get; }
        public List<Door> Doors { get; }
        public List<CollectableItem> Items{ get; }
        public List<CryoChamber> CryoChambers { get; }
        private KiPlanner KiPlan { get; set; }

        private List<Explosion> Explosions { get; }

        public List<DistractionObject> DistractionObjects { get; }
        private List<MineObjects> Mines { get; }
        public int Energy { get; set; }
        public int Oxygen { get; set; }

        
        public void AddDistractionObject(DistractionObject rabbit)
        {
            DistractionObjects.Add(rabbit);
        }
        public void AddMineObject(MineObjects mine)
        {
            try
            {
                mCollisionManager.AddCollider(mine);
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
            Mines.Add(mine);
        }

        private void RemoveMineObject(MineObjects mine)
        {
            Mines.Remove(mine);
            mCollisionManager.RemoveCollider(mine);
        }
        public void ToggleActive(List<ICharacter> characters, bool makeActive)
        {
            if (characters == null) return;
            if (!makeActive)
            {
                foreach (var c in characters)
                {
                    c.Active = false;
                }
            }
            else
            {
                foreach (var c in characters)
                {
                    c.Active = true;
                }
            }
        }
        public void ToggleTeams(ICharacter c)
        {
            if (c.Player)
            {
                mPlayerCharacters.Remove(c);
                NpCharacters.Add(c);
                c.Player = false;
                if (c.Weapon != null)
                {
                    c.Weapon.Player = false;
                }
            }
            else
            {
                NpCharacters.Remove(c);
                mPlayerCharacters.Add(c);
                c.Player = true;
                if (c.Weapon != null)
                {
                    c.Weapon.Player = true;
                }

            }
        }
        public List<ICharacter> FindCharactersInRadius(Vector2 pos, float radius, bool player)
        {
            List<ICharacter> cList;
            var ret = new List<ICharacter>();
            cList = player ? new List<ICharacter>(mPlayerCharacters):
                             new List<ICharacter>(NpCharacters);
            foreach (var character in cList)
            {
                if ((character.MidPoint - pos).Length() < radius)
                {
                    ret.Add(character);
                }
            }

            return ret;
        }

        public ICharacter FindNearestCharacterInRadius(Vector2 pos, float radius, bool player)
        {
            List<ICharacter> cList;
            cList = player ? new List<ICharacter>(mPlayerCharacters) :
                new List<ICharacter>(NpCharacters);

            var nList = cList.Where(c => Vector2.Distance(pos, c.MidPoint) < radius 
                                         && c.Pathfinding.IsVisible(c.MidPoint,pos));
            ICharacter nearest = null;
            float distance = 0;
            foreach (var character in nList)
            {
                if (nearest == null || Vector2.Distance(character.MidPoint, pos) < distance)
                {
                    nearest = character;
                    distance = Vector2.Distance(character.MidPoint, pos);
                }
            }

            return nearest;
        }

        /// <summary>
        /// Finds the nearest Character
        /// </summary>
        /// <param name="pos">current position</param>
        /// <param name="player">true: Player Character, false: Enemy Character</param>
        /// <returns>Nearest Character</returns>
        public ICharacter FindNearestCharacter(Vector2 pos, bool player)
        {
            List<ICharacter> cList;
            cList = player ? new List<ICharacter>(mPlayerCharacters):
                             new List<ICharacter>(NpCharacters);
            if (cList.Count == 0) return null;
            var nearest = cList[0];
            foreach (var character in cList)
            {
                if ((character.MidPoint - pos).Length() < (nearest.MidPoint - pos).Length())
                {
                    nearest = character;
                }
            }

            return nearest;
        }

        public ICharacter FindNearestDeadCharacter(Vector2 pos, bool player)
        {
            List<ICharacter> cList;
            cList = player ? new List<ICharacter>(mPlayerCharacters):
                             new List<ICharacter>(NpCharacters);
            if (cList.Count == 0) return null;
            
            var deadList = cList.FindAll(x => x.Health <= 0 && !x.IsBeingRevived);
            if (deadList.Count == 0) return null;
            var nearest = deadList[0];
            foreach (var character in deadList)
            {
                if ((character.MidPoint - pos).Length() < (nearest.MidPoint - pos).Length())
                {
                    nearest = character;
                }
            }

            return nearest;
        }

        public void AddExplodedCharacter(Explode explode, ICharacter character)
        {
            mExplodedCharacters.Add(explode);
            character.Health = 0;
        }

        public void AddProjectile(Projectile p)
        {
            try
            {
                mCollisionManager.AddCollider(p);
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
            Projectiles.Add(p);
        }

        public void RemoveProjectile(Projectile p)
        {
            Projectiles.Remove(p);
            mCollisionManager.RemoveCollider(p);
        }

        private int mMapLength;
        private int mMapHeight;

        public BattleModeState(CollisionManager collisionManager, Map map, int level = 0)
        {
            //Initialise all data structures
            Projectiles = new HashSet<Projectile>();
            mPlayerCharacters = new HashSet<ICharacter>();
            NpCharacters = new List<ICharacter>();
            Items = new List<CollectableItem>();
            Doors = new List<Door>();
            CryoChambers = new List<CryoChamber>();
            Energy = 0;
            Oxygen = 0;
            mCollisionManager = collisionManager;
            mLevel = level;

            mMapHeight = map.HeightInPixels;
            mMapLength = map.WidthInPixels;
            DistractionObjects = new List<DistractionObject>();
            Mines = new List<MineObjects>();
            mExplodedCharacters = new List<Explode>();
            Explosions = new List<Explosion>();
        }


    public void AddCryoChamber(CryoChamber chamber)
        {
            CryoChambers.Add(chamber);
        }

        public void AddDoor(Door door)
        {
            Doors.Add(door);
            mCollisionManager.AddCollider(door);
        }

        public void AddItem(CollectableItem item)
        {
            Items.Add(item);
        }


        private void CollectItem(IList<CollectableItem> items, HashSet<ICharacter> characters)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].Collected) continue;
                foreach (var c in characters)
                {
                    if (!c.CharacterArea.Intersects(items[i].ItemHitBox)) continue;
                    if (items[i].Item)
                    {
                        Energy++;
                        items.Remove(items[i]);
                        Game1.sSoundEffectInstance[10].Play();
                    }
                    else
                    {
                        Oxygen++;
                        items.Remove(items[i]);
                        Game1.sSoundEffectInstance[10].Play();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// this function adds a player character to the game
        /// </summary>
        /// <param name="c"></param>
        public void AddPlayerCharacter(ICharacter c)
        {
            try
            {
                mCollisionManager.AddCollider(c);
            }
            catch
            {
                return;
            }
            mPlayerCharacters.Add(c);
        }

        /// <summary>
        /// This function adds an NPC to the game.
        /// </summary>
        /// <param name="c"></param>
        public void AddNpc(ICharacter c)
        {
            try
            {
                mCollisionManager.AddCollider(c);
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
            NpCharacters.Add(c);
        }

        /// <summary>
        /// this function removes a player character from the game
        /// </summary>
        /// <param name="c"></param>
        private void RemovePlayerCharacter(ICharacter c)
        {
            mPlayerCharacters.Remove(c);
            mCollisionManager.RemoveCollider(c);
        }

        private void RemoveNpcCharacter(ICharacter npc)
        {
            NpCharacters.Remove(npc);
            mCollisionManager.RemoveCollider(npc);
        }

        public List<ICharacter> GetPlayerCharacters()
        {
            var allCharacters = new List<ICharacter>();
            foreach (var ch in mPlayerCharacters)
            {
                allCharacters.Add(ch);
            }
            return allCharacters;
        }

        public List<ICharacter> GetEnemyCharacters()
        {
            var allEnemies = new List<ICharacter>();
            foreach (var ch in NpCharacters)
            {
                allEnemies.Add(ch);
            }
            return allEnemies;
        }

        public void SetKiActions()
        {
            KiPlan = new KiPlanner(this, new HashSet<ICharacter>(NpCharacters));
        }

        public void ReinforcementKi()
        {
            KiPlan?.Reinforcement(new HashSet<ICharacter>(NpCharacters));
        }

        /// <summary>
        /// This function draws the whole game state
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="textureManager"></param>
        public void Draw(SpriteBatch sb, TextureManager textureManager)
        {
            if (CryoChambers.Count > 0)
            {
                CryoChambers[0].Draw(sb, textureManager);
            }

            foreach (var c in mPlayerCharacters)
            {
                if (c.Id == "Burkha")
                {
                    if (c.Ability.Active)
                    {
                        c.Ability.Draw(sb, textureManager);
                    }
                }
                if (c.Id != "Ngol") continue;
                if (c.Ability.Active)
                {
                    c.Ability.Draw(sb, textureManager);
                }
            }

            mHealthBar = new HealthBar(textureManager.GetTexture("PortraitInactive"));

            foreach (var c in mPlayerCharacters)
            {
                c.Draw(sb, textureManager);
            }

            foreach (var c in NpCharacters)
            {
                c.Draw(sb, textureManager);
                mHealthBar.DrawEnemyHp(sb, c);
            }

            foreach (var projectile in Projectiles)
            {
                projectile.Draw(sb, textureManager);
            }

            foreach (var door in Doors)
            {
                door.Draw(sb);
                if(door.Health < 100) mHealthBar.DrawDoorHp(sb, door);
            }

            if (Items.Count != 0)
            {
                foreach (var item in Items)
                {
                    item.Draw(sb, textureManager);
                }
            }
            foreach (var c in DistractionObjects)
            {
                c.Draw(sb, textureManager);
            }

            if (mExplodedCharacters.Count > 0)
            {
                foreach (var c in mExplodedCharacters)
                {
                    c.Draw(sb, textureManager, c.mMidPoint);
                }
            }

            if (Mines != null)
            {
                foreach (var c in Mines)
                {
                    c.Draw(sb, textureManager);
                }
            }

            foreach (var explosion in Explosions)
            {
                explosion.Draw(sb,textureManager);
            }
        }

        /// <summary>
        /// This function updates the game state
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            foreach (var door in Doors)
            {
                door.Update(gameTime);
            }
            if (CryoChambers.Count > 0)
            {
                CryoChambers[0].Update(gameTime, this);
            }

            CollectItem(Items, mPlayerCharacters);

            List<ICharacter> cl = new List<ICharacter>(mPlayerCharacters);
            foreach (var c in cl)
            {
                c.Update(gameTime);
                if (c.Health <= 0 && c.DeathTimer >=15)
                {
                    RemovePlayerCharacter(c);
                }

            }

            List<Projectile> projectiles = new List<Projectile>(Projectiles);

            foreach (var projectile in projectiles)
            {
                if (!projectile.Exploded)
                {
                    projectile.Update(gameTime);
                }
                else
                {
                    RemoveProjectile(projectile);
                    var size = projectile.Damage;
                    if (size < 20)
                    {
                        Explosions.Add(new Explosion("Explosion", 1.0f, 9, 
                            new Point(projectile.ObjectArea.Center.X - projectile.Damage / 2, projectile.ObjectArea.Y - projectile.Damage / 2), 
                            new Point(projectile.Damage, projectile.Damage)));

                    }
                    else
                    {
                        Explosions.Add(new Explosion("Explosion2", 1.0f,12,
                            new Point(projectile.ObjectArea.Center.X-size/4, projectile.ObjectArea.Center.Y-size/4),new Point(size/2,size/2) ));
                    }
                }
            }
            
            if (mLevel != 24 && mLevel != 26)
            {
                KiPlan?.Update(gameTime, this);
            }


            for (var i = 0; i < DistractionObjects.Count; i++)
            {
                DistractionObjects[i].Update(gameTime);
                if (DistractionObjects[i].Duration <= 0)
                {
                    DistractionObjects.RemoveAt(i);
                    i--;
                }
            }

            if (Mines != null)
            {
                var minesList = new List<MineObjects>(Mines);
                foreach (var mine in minesList)
                {
                    mine.Update(gameTime);
                    if (mine.mRemove)
                    {
                        RemoveMineObject(mine);
                    }
                }
            }

            if (mExplodedCharacters.Count > 0)
            {
                foreach (var c in mExplodedCharacters)
                {
                    c.Update(gameTime);
                    if (c.mExit)
                    {
                        mExplodedCharacters.Remove(c);
                    }
                }
            }

            var npcList = new List<ICharacter>(NpCharacters);
            foreach (var npc in npcList)
            {
                if (npc.Position.X < 0
                    || npc.Position.Y < 0
                    || npc.Position.X > mMapLength
                    || npc.Position.Y > mMapHeight
                    ) RemoveNpcCharacter(npc);
                npc.Update(gameTime);
                if (npc.Health <= 0)
                {
                    RemoveNpcCharacter(npc);
                    if(npc.Texture.Equals("Explosive"))
                        Explosions.Add(new Explosion("Explosion", 2.0f,9,npc.MidPoint.ToPoint(),new Point(80,80)));
                }
            }


            for (int i = 0; i < Explosions.Count; i++)
            {
                Explosions[i].Update(gameTime);
                if (Explosions[i].TimePassed > Explosions[i].Duration)
                {
                    Explosions.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
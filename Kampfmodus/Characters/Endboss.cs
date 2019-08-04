using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Collision;
using TRY.Kampfmodus.Commands;
using TRY.Kampfmodus.Weapons;

namespace TRY.Kampfmodus.Characters
{
    internal sealed class Endboss : ICharacter
    {
        private readonly string mProjectileTextureIdentifier;
        private readonly BattleModeState.FindCharacter mNearestCharacter;
        private readonly BattleModeState.ProjectileFunction mAddProjectileFunction;
        private readonly BattleModeState.ProjectileFunction mRemoveProjectileFunction;
        private float mPassedTime;
        private float mRotationTime;
        private int mDirection;

        public Animation CharacterAnimation { get; set; }
        private const int RangeRadius = 1000;

        private const int AttackRate = 3;
        private const int Damage = 15;
        private const float BulletSpeed = 0.4f;

        private bool mScream;


        private readonly List<Vector2> mCircle = new List<Vector2>
        {
            new Vector2(0,0), new Vector2(15,15), new Vector2(30,30), new Vector2(45,45),
            new Vector2(30,60), new Vector2(15,75),new Vector2(0,90), new Vector2(-15,75),
            new Vector2(-30,60), new Vector2(-45,45), new Vector2(-30,30), new Vector2(-15,15)
        };

        private readonly List<Vector2> mSpread1 = new List<Vector2>
        {
            new Vector2(-1,0), new Vector2(-2,2), new Vector2(1,0), new Vector2(2,2), new Vector2(0,1)
        };

        private readonly List<Vector2> mSpread2 = new List<Vector2>
        {
            new Vector2(-1,0), new Vector2(-1,2), new Vector2(1,0), new Vector2(1,2), new Vector2(0,1),
            new Vector2(-2,1), new Vector2(2,1)
        };


        public Endboss(string textureIdentifier, Point position, Pathfinding.Pathfinding pathfinding,
            BattleModeState.FindCharacter nearestCharacter,
            BattleModeState.ProjectileFunction addProjectileFunction,
            BattleModeState.ProjectileFunction removeProjectileFunction,
            string projectileProjectileTextureIdentifier)
        {
            Position = new Vector2(position.X, position.Y);
            Texture = textureIdentifier;
            Pathfinding = pathfinding;
            mNearestCharacter = nearestCharacter;
            mAddProjectileFunction = addProjectileFunction;
            mRemoveProjectileFunction = removeProjectileFunction;
            mProjectileTextureIdentifier = projectileProjectileTextureIdentifier;
            Vision = 400;
            mDirection = 0;
            CharacterArea = new Rectangle((int)Position.X, (int)Position.Y, 200, 250);
            Health = 1500;
            MidPoint = new Vector2(Position.X + (float)CharacterArea.Width / 2,
                Position.Y + (float)CharacterArea.Height / 2);
            mPassedTime = 0;
            Player = false;
            mRotationTime = 0;
            var astronaut = new[] { 6, 0, 6, 6, 4};
            CharacterAnimation = new Animation(textureIdentifier, new Vector2(CharacterArea.Width, CharacterArea.Height), new List<int>(astronaut));
            mScream = false;
        }


        public void Update(GameTime gameTime)
        {
            mRotationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // phase 1 
            if (Health > 500)
            {
                if (mRotationTime < 5)
                {
                    BulletPatternSpread(gameTime, mSpread1);
                }
                else
                {
                    BulletPatternSpread(gameTime, mSpread2);
                    if (mRotationTime > 10) { mRotationTime = 0; }
                }
            }
            // phase 2 
            else
            {
                if (!mScream)
                {
                    mScream = true;
                    Game1.sSoundEffectInstance[6].Play();
                }

                if (mRotationTime < 5)
                {
                    BulletPatternCircle(gameTime);
                }
                else
                {
                    BulletTrackNearestCharacter(gameTime);
                    if (mRotationTime > 10) { mRotationTime = 0; }
                }
            }

            var fps = mDirection == 0 || mDirection == 1 ? 5 : 13;
            CharacterAnimation.UpdateAnimation(gameTime, fps, mDirection);
            if (Health <= 0) { Game1.sSoundEffectInstance[6].Play(); }
        }

        /// <summary>
        /// Shoots bullets in a circle at the nearest character
        /// </summary>
        /// <param name="gameTime"></param>
        private void BulletPatternCircle(GameTime gameTime)
        {
            var closestPlayer = mNearestCharacter.Invoke(Position, !Player);
            if (closestPlayer == null) return;
            if (!(RangeRadius >= Vector2.Distance(closestPlayer.MidPoint, Position))) return;
            mPassedTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (mPassedTime > (1f / AttackRate * 4)) {
                foreach (var vector in mCircle)
                {
                    mAddProjectileFunction(
                        new Projectile(
                            mProjectileTextureIdentifier,
                            Player,
                            MidPoint + vector + new Vector2(0, 100),
                            closestPlayer.MidPoint + vector,
                            new Point(20, 20),
                            Damage, BulletSpeed));
                    mPassedTime = 0;
                }

                Game1.sSoundEffectInstance[4].Play();
            }
        }

        /// <summary>
        /// Attacks the nearest character directly with a high attack rate
        /// </summary>
        /// <param name="gameTime"></param>
        private void BulletTrackNearestCharacter(GameTime gameTime)
        {
            var closestPlayer = mNearestCharacter.Invoke(Position, !Player);
            if (closestPlayer == null) return;
            if (!(RangeRadius >= Vector2.Distance(closestPlayer.MidPoint, Position))) return;
            mPassedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mPassedTime > (1f / AttackRate))
            {
                mAddProjectileFunction(
                    new Projectile(
                        mProjectileTextureIdentifier,
                        Player,
                        MidPoint + new Vector2(0, 100),
                        closestPlayer.MidPoint,
                        new Point(20, 20),
                        Damage, BulletSpeed));
                mPassedTime = 0;
                Game1.sSoundEffectInstance[4].Play();
            }
        }

        /// <summary>
        /// Shoots a lot of bullets in all directions
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="pattern"></param>
        private void BulletPatternSpread(GameTime gameTime, List<Vector2> pattern)
        {
            mPassedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mPassedTime > (1f / AttackRate))
            {
                foreach (var vector in pattern)
                {
                    mAddProjectileFunction(
                        new Projectile(
                            mProjectileTextureIdentifier,
                            Player,
                            MidPoint,
                            MidPoint + vector,
                            new Point(20, 20),
                            Damage, BulletSpeed));
                    mPassedTime = 0;
                }
                Game1.sSoundEffectInstance[4].Play();
            }
        }


        public void Draw(SpriteBatch sb, TextureManager textureManager)
        {
            CharacterAnimation.Draw(sb, Position, textureManager);
        }

        public void UpdateCommand(ICommand x) {}
        public void AbortCommand() {}

        public void CollidesWith(IDynamicCollider collider)
        {
        }

        public void CollidesWith(IStaticCollider collider)
        {
        }

        public CollisionManager.HasMoved ObjectMoved { get; set; }
        public string Texture { set; get; }
        public Vector2 Position { get; set; }
        public Rectangle CharacterArea { get; }
        public Vector2? Destination { get; set; }
        public Vector2 MidPoint { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public float DeathTimer { get; set; }
        public IWeapon Weapon { get; set; }
        public IAbility Ability { get; set; }
        public IAbility SupportAbility { get; set; }
        public bool IsBeingRevived { get; set; }
        public int PlayerLevel { get; set; }
        public bool Player { get; set; }
        public bool Active { get; set; }
        public string Id { get; set; }
        public int Vision { get; }
        public Rectangle ObjectArea => CharacterArea;
        public Pathfinding.Pathfinding Pathfinding { get; set; }
    }
}

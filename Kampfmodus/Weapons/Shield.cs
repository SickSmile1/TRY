using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Weapons
{
    internal class Shield : IWeapon
    {
        public static bool mShieldActive;

        public int MaxShieldDamage { get; set; }
        public float SecondsSinceShot { get; set; }
        public static int MaxShieldDamage2 { get; private set; }
    
        public static float mCurrentShieldDamage;
        private readonly int mShieldRange;
        private Vector2 mSize;
        private readonly int mRange;
        private readonly string mTextureIdentifierShield;
        private readonly string mTextureIdentifierHpBar;
        private readonly HashSet<Projectile> mProjectiles;
        private readonly BattleModeState.FindCharacter mNearestCharacter;

        public Shield(string textureIdentifierShield, string textureIdentifierHpBar, BattleModeState.FindCharacter nearestCharacter, bool player,
            HashSet<Projectile> projectiles, int maxShieldValue = 500, int range = 400)
        {
            mRange = range;
            mTextureIdentifierHpBar = textureIdentifierHpBar;
            Player = player;
            mTextureIdentifierShield = textureIdentifierShield;
            mProjectiles = projectiles;
            mNearestCharacter = nearestCharacter;
            mCurrentShieldDamage = 1;
            mShieldActive = true;
            MaxShieldDamage = maxShieldValue;
            MaxShieldDamage2 = MaxShieldDamage;
            mShieldRange = range;
            SecondsSinceShot = 0;
            AttackRate = 1;
        }

        public void UseWeapon(GameTime gameTime, Vector2 position)
        {
            var enemyNearby = false;
            var nearestEnemy = mNearestCharacter.Invoke(position, !Player);
            if (nearestEnemy != null && (nearestEnemy.MidPoint - position).Length() < mRange)
            {
                enemyNearby = true;
            }
            if (mShieldActive)
            {
                List<Projectile> excludedObjects = new List<Projectile>();

                foreach (var projectile in mProjectiles)
                {
                    if (mShieldRange / 2f >= Vector2.Distance(projectile.CurrentPosition, position))
                    {
                        excludedObjects.Add(projectile);
                    }
                }
                foreach (var projectile in excludedObjects)
                {
                    if (projectile.Player != Player)
                    {
                        mCurrentShieldDamage += projectile.Damage;
                        projectile.Destroy();
                    }
                }
                //if the shield took too much damage deactivate shield
                if (mCurrentShieldDamage > MaxShieldDamage)
                {
                    mShieldActive = false;
                }
            }
            if (!enemyNearby)
            {
                mCurrentShieldDamage = MathHelper.Clamp(mCurrentShieldDamage - 1f, 0, MaxShieldDamage);
            }
            if (mCurrentShieldDamage < 0.1)
            {
                mShieldActive = true;
            }
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager, Vector2 position)
        {
            if (mShieldActive)
            {
                //Load texture
                Texture2D texture = textureManager.GetTexture(mTextureIdentifierShield);
                var x = new Vector2(texture.Width, texture.Height);
                x.Normalize();
                mSize = x * mShieldRange;
                // Position of circle centered around character
                var rectangle = new Rectangle((int) position.X - (int) mSize.X / 2,
                    (int) position.Y - (int) mSize.Y / 2,
                    (int) mSize.X,
                    (int) mSize.Y);
                sb.Draw(texture, rectangle, Color.AliceBlue);
            }
            // Draw shield hp bar
            var textureshield = textureManager.GetTexture(mTextureIdentifierHpBar);
            var healthFactor = (MaxShieldDamage2 - mCurrentShieldDamage) / MaxShieldDamage2;
            var healthBarWidth = textureshield.Width / 2f * healthFactor;
                sb.Draw(textureshield, new Rectangle((int)position.X - 40 , (int)position.Y - 60,
                    (int)healthBarWidth, textureshield.Height / 2), Color.White); 
        }

        public bool Player { get; set; }
        public int Damage { get; set; }
        public float AttackRate { get; set; }
    }
}

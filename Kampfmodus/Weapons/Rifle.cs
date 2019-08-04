using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Weapons
{
    internal sealed class Rifle : IWeapon
    {
        private readonly int mRangeRadius;
        private readonly string mProjectileTextureIdentifier;
        private readonly BattleModeState.CharacterInRadius mNearestCharacter;
        private readonly BattleModeState.ProjectileFunction mAddProjectileFunction;
        private Point mProjectileSize;

        public Rifle(string projectileProjectileTextureIdentifier, 
            bool player, 
            BattleModeState.CharacterInRadius nearestCharacter,
            BattleModeState.ProjectileFunction addProjectileFunction,
            int range = 400, 
            float fireRate = 3f, 
            int damage = 5, Point? projectileSize = null)
        {
            if (projectileSize == null)
            {
                projectileSize = new Point(10,10);
            }

            mProjectileSize = projectileSize.Value;
            mNearestCharacter = nearestCharacter;
            mAddProjectileFunction = addProjectileFunction;
            mProjectileTextureIdentifier = projectileProjectileTextureIdentifier;
            Player = player;
            mRangeRadius = range;
            SecondsSinceShot = 0;
            AttackRate = fireRate;
            Damage = damage;
        }

        public int MaxShieldDamage { get; set; }
        public float SecondsSinceShot { get; set; }
        public bool Player { get; set; }
        public int Damage { get; set; }
        public float AttackRate { get; set; }

        public void UseWeapon(GameTime gameTime, Vector2 position)
        {
            var closestEnemy = mNearestCharacter.Invoke(position, mRangeRadius, !Player);
            if(closestEnemy != null)
            {
                SecondsSinceShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (SecondsSinceShot > (1f / AttackRate))
                {
                    mAddProjectileFunction(
                        new Projectile(
                            mProjectileTextureIdentifier,
                            Player,
                            position,
                            closestEnemy.MidPoint,
                            mProjectileSize,
                            Damage));
                    SecondsSinceShot = 0;
                    if (mProjectileTextureIdentifier == "Laser")
                    {
                        Game1.sSoundEffectInstance[3].Play();
                    }
                    else
                    {
                        Game1.sSoundEffectInstance[4].Play();
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager, Vector2 position)
        {
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Weapons
{
    internal sealed class Melee : IWeapon
    {
        private readonly int mRangeRadius;
        private BattleModeState.FindCharacter mFindTarget;
        private bool mRobot;
        public int MaxShieldDamage { get; set; }
        public float SecondsSinceShot { get; set; }

        public Melee(BattleModeState.FindCharacter findTarget, bool player, int range, float rate = 1f, int damage = 30, bool robot = false)
        {
            Player = player;
            mFindTarget = findTarget;
            mRangeRadius = range;
            SecondsSinceShot = 0;
            AttackRate = rate;
            Damage = damage;
            mRobot = robot;
        }

        public void UseWeapon(GameTime gameTime, Vector2 position)
        {
            //Find closest Character of opposite team
            SecondsSinceShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            var closestEnemy = mFindTarget.Invoke(position, !Player);
            if(closestEnemy == null ||
               !(mRangeRadius >= Vector2.Distance(closestEnemy.MidPoint, position)) ||
               !(SecondsSinceShot > (1 / AttackRate)))
                return;
            closestEnemy.Health -= Damage;
            SecondsSinceShot = 0;
            if (mRobot)
            {
                Game1.sSoundEffectInstance[11].Play();
            }
            else
            {
                Game1.sSoundEffectInstance[5].Play();
            }
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager, Vector2 position)
        {
            
        }

        public bool Player { get; set; }
        public int Damage { get; set; }
        public float AttackRate { get; set; }
    }
}

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Weapons
{
    internal class Explode : IWeapon
    {
        private readonly BattleModeState.CharactersInRadius mFindCharactersInRadius;
        private readonly BattleModeState.ExplodedCharacter mBmsExplodedCharacter;
        private Animation mExplosionAnimation;
        private readonly float mRadius;
        private readonly ICharacter mCharacter;
        private Vector2 mSize;
        public Vector2 mMidPoint;
        private bool mExplode;
        public bool mExit;

        public Explode(BattleModeState.CharactersInRadius findCharactersInRadius, BattleModeState.ExplodedCharacter c,
            ICharacter character, float radius = 100, int damage = 50)
        {
            mFindCharactersInRadius = findCharactersInRadius;
            mBmsExplodedCharacter = c;
            mCharacter = character;
            mRadius = radius;
            mSize = new Vector2(50, 50);
            Damage = damage;
            mExplode = false;
            mExit = false;

            SecondsSinceShot = 0;
            AttackRate = 1;
            var animation = new[] { 9 };
            mExplosionAnimation = new Animation("Explosion", mSize, new List<int>(animation));
        }

        public int Damage { get; set; }
        public float AttackRate { get; set; }
        public int MaxShieldDamage { get; set; }
        public float SecondsSinceShot { get; set; }

        public void UseWeapon(GameTime gameTime, Vector2 position)
        {
            if (mExplode) return;
            var character1 = mFindCharactersInRadius(position, mRadius, !Player);
            if (character1.Count <= 0) return;
            var character = character1.Where(c => c.Health > 0 && c.Pathfinding.IsVisible(c.MidPoint, position));
            var characters = character as ICharacter[] ?? character.ToArray();
            if (characters.Any())
            {
                mExplode = true;
                foreach (var c in characters)
                {
                    c.Health -= Damage;
                }
                mExplosionAnimation.ResetAnimation();
                mBmsExplodedCharacter(this, mCharacter);
                mMidPoint = new Vector2(mCharacter.MidPoint.X - (mSize.X / 2), mCharacter.MidPoint.Y - (mSize.Y / 2));
            }
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager, Vector2 position)
        {
            mExplosionAnimation.Draw(sb, mMidPoint, textureManager);
        }

        public bool Player { get; set; }

        public void Update(GameTime gameTime)
        {
            mExplosionAnimation.UpdateAnimation(gameTime, 22f / 5, 0, false);
            mExit = false;
        }
    }
}

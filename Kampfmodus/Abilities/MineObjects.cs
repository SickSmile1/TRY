using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;
using TRY.Kampfmodus.Collision;

namespace TRY.Kampfmodus.Abilities
{
    internal class MineObjects: IDynamicCollider
    {
        private readonly string mTextureDescriptor;
        public Rectangle ObjectArea { get; }
        public CollisionManager.HasMoved ObjectMoved { get; set; }
        private int Damage { get; }
        private readonly float mRadius;
        private readonly Animation mMineAnimation;
        private bool mExplosion;
        public bool mRemove;
        private float mTimer;
        private Vector2 mPosition;
        private readonly Point mSize;
        private readonly BattleModeState.CharactersInRadius mCharactersInRadius;
        private readonly bool mPlayer;
        public MineObjects(string textureDescriptor, BattleModeState.CharactersInRadius charactersInRadius, bool player, Vector2 position, Point size, int damage, float radius)
        {
            mTextureDescriptor = textureDescriptor;
            mCharactersInRadius = charactersInRadius;
            mPlayer = player;
            mPosition = position;
            Damage = damage;
            mSize = size;
            mRadius = radius;
            mRemove = false;
            mTimer = 0;
            ObjectArea = new Rectangle((int)position.X, (int)position.Y, size.X, size.Y);
            mExplosion = false;
            var animation = new[] { 12 };
            mMineAnimation = new Animation("Explosion2", new Vector2(size.X*2, size.Y*2), new List<int>(animation));
        }
        public void CollidesWith(IDynamicCollider collider)
        {
            if (!(collider is ICharacter character)) return;
            if (mExplosion) return;
            if (character.Player == mPlayer) return;
            var affectedCharacters = mCharactersInRadius(mPosition, mRadius, !mPlayer);
            foreach (var affectedCharacter in affectedCharacters)
            {
                affectedCharacter.Health -= Damage;
            }
            mExplosion = true;
            mMineAnimation.ResetAnimation();
            Game1.sSoundEffectInstance[2].Play();
        }

        public void CollidesWith(IStaticCollider collider)
        {
            if (!collider.ObjectArea.Contains(mPosition)) return;
            while (collider.ObjectArea.Contains(mPosition))
            {
                mPosition = new Vector2(mPosition.X + 1, mPosition.Y);
            }
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager)
        {
            if (mExplosion)
            {
                mMineAnimation.Draw(sb, new Vector2(mPosition.X-((float)mSize.X/2), mPosition.Y-((float)mSize.X/2) ), textureManager);
            }
            else
            {
                sb.Draw(textureManager.GetTexture(mTextureDescriptor), ObjectArea, Color.White);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!mExplosion) return;
            mMineAnimation.UpdateAnimation(gameTime, 7f, 0, false);
            mTimer += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (mTimer > 2.1f) mRemove = true;
        }
    }
}

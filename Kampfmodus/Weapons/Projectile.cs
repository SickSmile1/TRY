using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;
using TRY.Kampfmodus.Collision;

namespace TRY.Kampfmodus.Weapons
{
    internal sealed class Projectile : IDynamicCollider
    {
        private readonly Vector2 mDirection;
        public bool Player { get; }
        public bool Exploded { get; private set; }
        private float mMaxDistance;
        private readonly float mSpeed;
        private readonly Point mSize;
        private readonly string mTextureIdentifier;

        public Projectile(string textureIdentifier, bool player, Vector2 position, Vector2 destination, Point size, int damage, float speed = 1.2f, float maxDistance = 1600)
        {
            Exploded = false;
            mSize = size;
            Damage = damage;
            mTextureIdentifier = textureIdentifier;
            mSpeed = speed;
            Player = player;
            CurrentPosition = new Vector2(position.X, position.Y);
            ObjectArea = new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, mSize.X, mSize.Y);
            mDirection = Vector2.Normalize(destination - CurrentPosition);
            mMaxDistance = maxDistance;
        }

        public Vector2 CurrentPosition { get; private set; }
        public int Damage { get; }
        public Rectangle ObjectArea { get; private set; }
        public CollisionManager.HasMoved ObjectMoved { get; set; }

        public void Update(GameTime elapsedMs)
        {
            var forward = mDirection * (float)elapsedMs.ElapsedGameTime.TotalMilliseconds * mSpeed;
            CurrentPosition += forward;
            ObjectArea = new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, mSize.X, mSize.Y);
            ObjectMoved?.Invoke(this);
            mMaxDistance -= forward.Length();
            if (mMaxDistance <= 0)
            {
                Exploded = true;
            }
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager)
        {
            var texture = textureManager.GetTexture(mTextureIdentifier);
            var rectangle = new Rectangle((int)CurrentPosition.X, (int) CurrentPosition.Y,
                mSize.X, mSize.Y);
            sb.Draw(texture, 
                destinationRectangle: rectangle, 
                null,
                Color.AliceBlue,
                (float)Math.Atan2(mDirection.Y,mDirection.X),
                new Vector2(0,0),
                SpriteEffects.None,
                0);
        }
        public void CollidesWith(IDynamicCollider collider)
        {
            //In case the projectile hits a Character, only explode if the Character is a target
            if (collider is ICharacter collidingCharacter && !Exploded)
            {
                if (collidingCharacter.Player != Player)
                {
                    collidingCharacter.Health -= Damage;
                    Exploded = true;
                }
            }
        }

        public void Destroy()
        {
            Exploded = true;
        }

        public void CollidesWith(IStaticCollider collider)
        {
            if (collider is Door door)
            {
                door.Health -= Damage;
            }
            Exploded = true;
        }
    }
}

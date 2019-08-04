using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus
{
    class CryoChamber
    {
        private readonly int mUnlockTime;
        private float mElapsedTime;
        public bool Create { get; set; }
        private bool mUnlock;
        private bool mUnlocked;
        public readonly ICharacter mNewCharacter;
        public Vector2 mPosition;
        private int mUnlockRadius;
        private  readonly Animation mCryoAnimation;

        public CryoChamber(string textureIdentifier, Vector2 size,
                           int unlockArea, int unlockTime, ICharacter newCharacter)
        {
            Create = false;
            // Checks if a character is nearby.
            mUnlock = false;
            // Checks if frozen character is set free.
            mUnlocked = false;
            mElapsedTime = 0;
            mUnlockTime = unlockTime;
            mNewCharacter = newCharacter;
            mPosition = mNewCharacter.Position;
            mUnlockRadius = unlockArea;
            // 0:Idle, 1:Load Animation, 2:Destroyed.
            var cryo = new[] { 1, 22, 1 };
            mCryoAnimation = new Animation(textureIdentifier, size, new List<int>(cryo));
        }

        public void Update(GameTime gameTime, BattleModeState bms)
        {
            // Type of animation based on variable unlock.
            int unlock;

            List<ICharacter> charactersInRadius = bms.FindCharactersInRadius(mPosition, mUnlockRadius, true);
            foreach (var characterInRadius in charactersInRadius)
            {
                if ((characterInRadius.MidPoint - mPosition).Length() < mUnlockRadius)
                {
                    mUnlock = true;
                }
            }
            // If true, set countdown and unlock the character after a certain time.
            // After unlocking character disable further interactions by setting mUnlocked true.
            // If clauses mainly exist for choosing the animations.
            if (mUnlock && !mUnlocked)
            {
                mElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                unlock = 1;
                if (mElapsedTime >= mUnlockTime)
                {
                    Create = true;
                    mUnlocked = true;
                    unlock = 2;
                    Game1.sSoundEffectInstance[13].Play();
                }
                mCryoAnimation.UpdateAnimation(gameTime, 22f / mUnlockTime, unlock, false);
            }
            else if(!mUnlock && !mUnlocked)
            {
                mElapsedTime = 0;
                unlock = 0;
                mCryoAnimation.ResetAnimation();
                mCryoAnimation.UpdateAnimation(gameTime, 22f / mUnlockTime, unlock, false);
            }
            else
            {
                unlock = 2;
                mCryoAnimation.ResetAnimation();
                mCryoAnimation.UpdateAnimation(gameTime, 22f / mUnlockTime, unlock, false);
            }
            mUnlock = false;
        }


        public void Draw(SpriteBatch sb, TextureManager textureManager)
        {
            mCryoAnimation.Draw(sb, mPosition, textureManager);
        }
    }
}

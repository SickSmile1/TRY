using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Characters
{
    class Animation
    {
        public string TextureIdentifier { private get; set; }
        private  Rectangle mSourceRectangle;
        private readonly List<int> mFramesPerAnimation;

        private float mElapsedTime;
        private int mFrameWidth;
        private int mFrameHeight;
        private float mScaleX;
        private float mScaleY;
        public int mCurrentFrame;

        public Animation(string textureIdentifier, Vector2 scale, List<int> framesPerAnimations)
        {
            TextureIdentifier = textureIdentifier;
            // This list stores the number of frames an animation needs.
            mFramesPerAnimation = framesPerAnimations;
            //Initialised this was so the real values can be obtained in the first draw routine
            mFrameWidth = 0;
            mFrameHeight = 0;
            mScaleX = scale.X;
            mScaleY = scale.Y;
            mElapsedTime = 0;
            mCurrentFrame = 0;
        }

        public void ResetAnimation()
        {
            mCurrentFrame = 0;
        }

        public void UpdateAnimation(GameTime gameTime,float fps, int animation, bool loop=true)
        {
            mElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(mFrameHeight != 0)
            {
                var i = mCurrentFrame == 0 ? 0 : 1;
                var j = animation == 0 ? 0 : 1;
                mSourceRectangle = new Rectangle(mCurrentFrame * mFrameWidth + i, animation * mFrameHeight + j,
                                             mFrameWidth, mFrameHeight);
            }
            else
            {
                mSourceRectangle = new Rectangle(mCurrentFrame,animation, 32,32);
            }
            if (mElapsedTime > 1f / fps)
            {
                if (mCurrentFrame < mFramesPerAnimation[animation] - 1)
                {
                    mCurrentFrame++;
                }
                else
                {
                    mCurrentFrame = !loop ? mCurrentFrame : 0;
                }
                mElapsedTime = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos, TextureManager textureManager)
        {
            var texture = textureManager.GetTexture(TextureIdentifier);
            if (mFrameHeight == 0)
            {
                mFrameWidth = texture.Width / mFramesPerAnimation.Max();
                mFrameHeight = texture.Height / mFramesPerAnimation.Count;
                mScaleX /= mFrameWidth;
                mScaleY /= mFrameHeight;
            }
            spriteBatch.Draw(texture, pos, mSourceRectangle, Color.White, 0,
                             new Vector2(0, 0),
                             new Vector2(mScaleX, mScaleY), SpriteEffects.None, 0);
        }
    }
}

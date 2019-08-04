using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.GameStates.Structure;

namespace TRY
{
    /// <summary>
    /// This class uses a list of States to manage all Screens. The last entry of the list is drawn and updated.
    /// </summary>
    internal sealed class ScreenManager
    {
        private readonly List<States> mScreens;

        internal ScreenManager()
        {
            mScreens = new List<States>();
        }

        public States CurrentScreen(int i)
        {
            return mScreens[mScreens.Count - i];
        }

        /// <summary>
        /// Add Screen to list
        /// </summary>
        /// <param name="screen"></param>
        internal void AddScreen(States screen)
        {
            mScreens.Add(screen);
        }

        /// <summary>
        /// Remove Screen from list
        /// </summary>
        internal void RemoveScreen()
        {
            mScreens?.RemoveAt(mScreens.Count - 1);
        }

        /// <summary>
        /// Calls the Draw method for the last state of mScreens
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        internal void ScreenDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var item = mScreens[mScreens.Count - 1];
            item.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Calls the Update method for the last state of mScreens
        /// </summary>
        /// <param name="gameTime"></param>
        internal void ScreenUpdate(GameTime gameTime)
        {
            var item = mScreens[mScreens.Count - 1];
            item.Update(gameTime);
        }
    }
}

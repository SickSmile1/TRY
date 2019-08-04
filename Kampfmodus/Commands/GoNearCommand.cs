using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    /// <summary>
    /// A simple command that orders a character to move
    /// </summary>
    class GoNearCommand : ICommand
    {
        //The Character that is supposed to move
        private ICharacter mCharacter;
        //The destination of the Character
        private Vector2 mDestination;
        private bool mSent;
        private float mDistance;

        /// <summary>
        /// Create a new move command.
        /// </summary>
        /// <param name="character">The Character that is supposed to move</param>
        /// <param name="dest">The Destination of the Character</param>
        /// <param name="distance"></param>
        public GoNearCommand(ICharacter character, Point dest, float distance)
        {
            mCharacter = character;
            mDestination = new Vector2(dest.X, dest.Y);
            mSent = false;
            mDistance = distance;
        }

        /// <summary>
        /// Execute the Command, move the Character
        /// </summary>
        public void Execute()
        {
            if (!mSent)
            {
                mCharacter.Destination = mDestination;
                mSent = true;
            }
            if (Vector2.Distance(mCharacter.MidPoint, mDestination) < mDistance)
            {
                mCharacter.Destination = null;
                mCharacter.AbortCommand();
            }
        }
    }
}
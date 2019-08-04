using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    /// <summary>
    /// A simple command that orders a character to move
    /// </summary>
    class PatrolCommand : ICommand
    {
        //The Character that is supposed to move
        private ICharacter mCharacter;
        //The destination of the Character
        private Vector2 mDestination1;
        private Vector2 mDestination2;
        private bool mReverse;

        /// <summary>
        /// Create a new move command.
        /// </summary>
        /// <param name="character">The Character that is supposed to move</param>
        /// <param name="dest1"></param>
        /// <param name="dest2"></param>
        public PatrolCommand(ICharacter character, Point dest1, Point dest2)
        {
            mCharacter = character;

            mDestination1 = new Vector2(dest1.X, dest1.Y);
            mDestination2 = new Vector2(dest2.X, dest2.Y);

            mCharacter.Destination = mDestination1;
            mReverse = false;
        }

        /// <summary>
        /// Execute the Command, move the Character
        /// </summary>
        public void Execute()
        {
            //If the Character moves in reverse, Move from Destination to Start.
            if (mReverse)
            {
                if (mCharacter.Destination == null)
                {
                    mCharacter.Destination = mDestination2;
                    mReverse = false;
                }
            }
            //If the Character does not move in reverse, Move from Start to Destination.
            else
            {
                if (mCharacter.Destination == null)
                {
                    mCharacter.Destination = mDestination1;
                    mReverse = true;
                }
            }
        }
    }
}

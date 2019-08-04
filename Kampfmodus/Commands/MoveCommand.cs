using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    /// <summary>
    /// A simple command that orders a character to move
    /// </summary>
    class MoveCommand : ICommand
    {
        //The Character that is supposed to move
        private ICharacter mCharacter;
        //The destination of the Character
        private Vector2 mDestination;

        /// <summary>
        /// Create a new move command.
        /// </summary>
        /// <param name="character">The Character that is supposed to move</param>
        /// <param name="dest">The Destination of the Character</param>
        public MoveCommand(ICharacter character, Point dest)
        {
            mCharacter = character;
            mDestination = new Vector2(dest.X,dest.Y);
        }

        /// <summary>
        /// Execute the Command, move the Character
        /// </summary>
        public void Execute()
        {
            mCharacter.Destination = mDestination;
            mCharacter.AbortCommand();
        }
    }
}

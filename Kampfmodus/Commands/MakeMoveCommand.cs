using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    /// <summary>
    /// A simple class to produce a move Command for a given character.
    /// </summary>
    class MakeMoveCommand : ICommandFactory
    {
        //The move Destination of the Character
        private readonly Point mDestination;

        /// <summary>
        /// Create a new Move-Command producer by specifying 
        /// </summary>
        /// <param name="dest"></param>
        public MakeMoveCommand(Point dest)
        {
            mDestination = dest;
        }

        /// <summary>
        /// Produce a move command to a given point for a given character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public ICommand ProduceCommand(ICharacter c)
        {
            return new MoveCommand(c,mDestination);
        }
    }
}

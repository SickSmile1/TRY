using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    /// <summary>
    /// A simple class to produce a patrol command for a given character.
    /// </summary>
    class MakePatrolCommand : ICommandFactory
    {
        //The move Destination of the Character
        private readonly Point mDestination1;
        private readonly Point mDestination2;

        /// <summary>
        /// Create a new Move-Command producer by specifying 
        /// </summary>
        public MakePatrolCommand(Point dest1, Point dest2)
        {
            mDestination1 = dest1;
            mDestination2 = dest2;
        }

        // Produce a move command to a given point for a given character
        public ICommand ProduceCommand(ICharacter c)
        {
            return new PatrolCommand(c, mDestination1, mDestination2);
        }
    }
}

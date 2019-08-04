using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    /// <summary>
    /// A simple class to produce a move Command for a given character.
    /// </summary>
    class MakeFollowCommand : ICommandFactory
    {
        //The move Destination of the Character
        private readonly ICharacter mDestination;

        /// <summary>
        /// Create a new Follow command producer by specifying Character to follow
        /// </summary>
        /// <param name="dest"></param>
        public MakeFollowCommand(ICharacter dest)
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
            return new FollowCommand(c, mDestination);
        }
    }
}
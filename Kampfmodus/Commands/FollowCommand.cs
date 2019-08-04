using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    /// <summary>
    /// A simple command that orders a character to move
    /// </summary>
    class FollowCommand : ICommand
    {
        //The Character that is supposed to move
        private ICharacter mCharacter;
        //The destination of the Character
        private ICharacter mFollowCharacter;

        /// <summary>
        /// Create a new move command.
        /// </summary>
        /// <param name="character">The Character that is supposed to move</param>
        /// <param name="followCharacter"></param>
        public FollowCommand(ICharacter character, ICharacter followCharacter)
        {
            mCharacter = character;
            mFollowCharacter = followCharacter;
        }

        /// <summary>
        /// Execute the Command, move the Character
        /// </summary>
        public void Execute()
        {
            if (mCharacter == mFollowCharacter) mCharacter.AbortCommand();
            else if (Vector2.Distance(mCharacter.Position, mFollowCharacter.Position) <
                     // ReSharper disable once RedundantJumpStatement
                     mCharacter.CharacterArea.Height) return;
            else mCharacter.Destination = mFollowCharacter.MidPoint;
        }
    }
}
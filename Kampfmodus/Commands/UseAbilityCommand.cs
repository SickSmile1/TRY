using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    class UseAbilityCommand: ICommand
    {
        private ICharacter mCharacter;
        public UseAbilityCommand(ICharacter character)
        {
            mCharacter = character;
        }
        public void Execute()
        {
            mCharacter.Ability?.UseAbility(mCharacter.MidPoint);
            mCharacter.AbortCommand();
        }
    }
}

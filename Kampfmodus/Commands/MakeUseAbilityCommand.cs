using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    class MakeUseAbilityCommand : ICommandFactory
    {
        public ICommand ProduceCommand(ICharacter c)
        {
            return new UseAbilityCommand(c);
        }
    }
}

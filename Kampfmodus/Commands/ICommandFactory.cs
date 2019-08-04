using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    interface ICommandFactory
    {
        ICommand ProduceCommand(ICharacter c);
    }
}

using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    interface ICommandSubject
    {
        void Register(ICharacter observer);
        void Unregister(ICharacter observer);
        void UnregisterAll();
        void SendCommand(ICommandFactory commandFactory);
    }
}

namespace TRY.Kampfmodus.Commands
{
    interface ICommandObserver
    {
        void UpdateCommand(ICommand x);
        void AbortCommand();
    }
}

using System.Collections.Generic;
using TRY.Kampfmodus.Characters;
using TRY.Kampfmodus.Commands;

namespace TRY.Kampfmodus
{
    class SelectedSubject: ICommandSubject
    {
        private HashSet<ICharacter> mSelectedCharacters;

        public SelectedSubject()
        {
            mSelectedCharacters = new HashSet<ICharacter>();
        }
        public void Register(ICharacter observer)
        {
            mSelectedCharacters.Add(observer);
        }

        public void Unregister(ICharacter observer)
        {
            mSelectedCharacters.Remove(observer);
        }

        public void UnregisterAll()
        {
            mSelectedCharacters.Clear();
        }

        public void SendCommand(ICommandFactory commandFactory)
        {
            foreach (var c in mSelectedCharacters)
            {
                c.UpdateCommand(commandFactory.ProduceCommand(c));
            }
        }

        public bool IsObserver(ICharacter ch)
        {
            return mSelectedCharacters.Contains(ch);
        }

        public List<ICharacter> RegisteredObservers => new List<ICharacter>(mSelectedCharacters);
    }
}

using System.Collections.Generic;
using TRY.Kampfmodus.Interfaces;

namespace TRY.Kampfmodus
{
    class SelectedSubject: ICommandSubject
    {
        private HashSet<Character> mSelectedCharacters;

        public SelectedSubject()
        {
            mSelectedCharacters = new HashSet<Character>();
        }
        public void Register(Character observer)
        {
            mSelectedCharacters.Add(observer);
        }

        public void Unregister(Character observer)
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
    }
}

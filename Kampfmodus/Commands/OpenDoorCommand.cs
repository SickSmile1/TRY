using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Commands
{
    class OpenDoorCommand: ICommand
    {
        private Door mDoor;
        private ICharacter mCharacter;

        public OpenDoorCommand(ICharacter character, Door door)
        {
            mDoor = door;
            mCharacter = character;
        }
        public void Execute()
        {
            mCharacter.Destination = mDoor.DoorArea.Center.ToVector2();
            var distance = (mCharacter.MidPoint - mDoor.DoorArea.Center.ToVector2()).Length();
            if (distance < 80)
            {
                mDoor.Toggle();
                mCharacter.Destination = null;
                mCharacter.AbortCommand();
            }
        }
    }
}

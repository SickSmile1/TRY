using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;
using TRY.Kampfmodus.Commands;
using TRY.Kampfmodus.Graphics;

namespace TRY.Kampfmodus
{
    class Selection
    {

        private SelectedSubject mSelectedCharacters;
        private BattleModeState mBattleModeState;

        public SelectionBox SelectionBox { get; }

        private bool mOpenDoor;
        private Door mDoorToOpen;

        private bool mLeftDraggingActive;
        private Point mSelectionStart;

        private bool mRightDraggingActive;
        public Line mLine;
        private readonly Line mEmptyLine;
        private bool mTechdemo;

        public Selection(BattleModeState bms, SelectionBox sb, bool techdemo = false)
        {
            mSelectedCharacters = new SelectedSubject();
            mBattleModeState = bms;
            mTechdemo = techdemo;
            SelectionBox = sb;

            mEmptyLine = new Line("WhiteLine", new Point(0, 0), new Point(0, 0));
            mLine = mEmptyLine;

            InputManager.Instance.LeftClickStart += OnLeftClickStart;
            InputManager.Instance.LeftClickHold += OnLeftClickHold;
            InputManager.Instance.LeftClickRelease += OnLeftClickRelease;
            InputManager.Instance.RightClickStart += OnRightClickStart;
            InputManager.Instance.RightClickHold += OnRightClickHold;
            InputManager.Instance.RightClickRelease += OnRightClickRelease;
            InputManager.Instance.ActivateAbility += OnActivateAbility;
        }

        public List<ICharacter> SelectedCharacters => mSelectedCharacters.RegisteredObservers;

        private void OnActivateAbility(object sender, EventArgs eventArgs)
        {
            SendCommand(new MakeUseAbilityCommand());
        }

        public bool IsSelected(ICharacter ch)
        {
            return mSelectedCharacters.IsObserver(ch);
        }

        private void OnRightClickStart(object sender, EventArgs eventArgs)
        {
            var e = (InputManager.ClickEventArgs)eventArgs;
            mLine = new Line("WhiteLine", e.MousePosition, e.MousePosition);
        }

        private void OnRightClickHold(object sender, EventArgs eventArgs)
        {
            var e = (InputManager.ClickEventArgs)eventArgs;
            if (mRightDraggingActive ||
                Math.Abs(mLine.StartPoint.X - e.MousePosition.X) > 10 ||
                Math.Abs(mLine.StartPoint.Y - e.MousePosition.Y) > 10)
            {
                mRightDraggingActive = true;
                mLine.EndPoint = e.MousePosition;
            }
        }
        private void OnRightClickRelease(object sender, EventArgs eventArgs)
        {
            var e = (InputManager.ClickEventArgs)eventArgs;
            mLine.EndPoint = e.MousePosition;
            if (mRightDraggingActive)
            {
                SendCommand(new MakePatrolCommand(mLine.StartPoint, mLine.EndPoint));
                mRightDraggingActive = false;
            }
            else
            {
                foreach (var playerCharacter in mBattleModeState.mPlayerCharacters)
                {
                    if (playerCharacter.CharacterArea.Contains(e.MousePosition))
                    {
                        SendCommand(new MakeFollowCommand(playerCharacter));
                        return;
                    }
                }
                if(mTechdemo) SendCommand(new MakeGoNearCommand(mLine.EndPoint, 132));
                else SendCommand(new MakeMoveCommand(mLine.EndPoint));
            }

            mLine = mEmptyLine;
        }

        private void OnLeftClickStart(object sender, EventArgs eventArgs)
        {
            var e = (InputManager.ClickEventArgs)eventArgs;

            mOpenDoor = false;
            foreach (var door in mBattleModeState.Doors)
            {
                if (door.DoorArea.Contains(e.MousePosition))
                {
                    mOpenDoor = true;
                    mDoorToOpen = door;
                }
            }

            if (!mOpenDoor)
            {
                mSelectionStart = new Point(e.MousePosition.X, e.MousePosition.Y);
                SelectionBox.Rectangle = new Rectangle(e.MousePosition.X, e.MousePosition.Y, 0, 0);
                foreach (var character in mBattleModeState.GetPlayerCharacters())
                {
                    if (character.CharacterArea.Contains(e.MousePosition))
                    {
                        SelectCharacter(character);
                    }
                }
            }
        }

        private void OnLeftClickHold(object sender, EventArgs eventArgs)
        {
            var e = (InputManager.ClickEventArgs)eventArgs;
            if (!mOpenDoor)
            {
                if (mLeftDraggingActive ||
                    Math.Abs(mSelectionStart.X - e.MousePosition.X) > 3 ||
                    Math.Abs(mSelectionStart.Y - e.MousePosition.Y) > 3)
                {
                    mLeftDraggingActive = true;
                    mOpenDoor = false;
                    SelectionBox.Rectangle = new Rectangle(
                        Math.Min(e.MousePosition.X, mSelectionStart.X),
                        Math.Min(e.MousePosition.Y, mSelectionStart.Y),
                        Math.Abs(e.MousePosition.X - mSelectionStart.X),
                        Math.Abs(e.MousePosition.Y - mSelectionStart.Y)
                    );
                    foreach (var character in mBattleModeState.GetPlayerCharacters())
                    {
                        if (SelectionBox.Rectangle.Intersects(character.CharacterArea))
                        {
                            SelectCharacter(character);
                        }
                        else
                        {
                            UnselectCharacter(character);
                        }
                    }
                }
            }
        }

        private void OnLeftClickRelease(object sender, EventArgs eventArgs)
        {
            if (mOpenDoor)
            {
                if(SelectedCharacters.Count > 0)
                    SelectedCharacters[0].UpdateCommand(new OpenDoorCommand(SelectedCharacters[0], mDoorToOpen));
                mOpenDoor = false;
                mDoorToOpen = null;
            }
            else
            {
                SelectionBox.Rectangle = new Rectangle(-1, -1, 0, 0);
            }
        }


        /// <summary>
        /// This function sends a command to all selected characters
        /// </summary>
        /// <param name="cf"></param>
        private void SendCommand(ICommandFactory cf)
        {
            mSelectedCharacters.SendCommand(cf);
        }

        public void UnselectAll()
        {
            mSelectedCharacters.UnregisterAll();
        }

        /// <summary>
        /// Register a single character as selected
        /// </summary>
        /// <param name="c"></param>
        public void SelectCharacter(ICharacter c)
        {
            mSelectedCharacters.Register(c);
        }

        private void UnselectCharacter(ICharacter c)
        {
            mSelectedCharacters.Unregister(c);
        }
    }
}

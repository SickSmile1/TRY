using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Interfaces;

namespace TRY.Kampfmodus
{
    //Saves all relevant game state information, tracks all characters
    class BattleModeState
    {
        //this is a collection of all playable characters
        public HashSet<Character> mPlayerCharacters;
        //this data structure registers all selected characters in an observer-pattern that communicates command-objects to all registered characters
        private SelectedSubject mSelectedCharacters;

        public BattleModeState()
        {
            //Initialise all data structures
            mPlayerCharacters = new HashSet<Character>();
            mSelectedCharacters = new SelectedSubject();
        }

        //this function adds a player character to the game
        public void AddPlayerCharacter(Character c)
        {
            mPlayerCharacters.Add(c);
        }

        //this function removes a player character from the game
        public void RemovePlayerCharacter(Character c)
        {
            mPlayerCharacters.Remove(c);
        }

        //this function selects all clicked-on characters as selected
        public void SelectPlayerCharacters(Point clickPosition)
        {
            //initialise the list of clicked-on characters as empty
            List<Character> selectedCharacters = new List<Character>();

            //Check if the clicked Position lies in the range of any of the player characters
            foreach (var character in mPlayerCharacters)
            {
                if (character.GetRectangle().Contains(clickPosition))
                {
                    selectedCharacters.Add(character);
                }
            }

            //register all clicked-on characters as selected
            SelectCharacters(selectedCharacters);
        }

        //Register a single character as selected
        public void SelectCharacter(Character c)
        {
            mSelectedCharacters.UnregisterAll();
            mSelectedCharacters.Register(c);
        }

        //Register a list of characters as selected
        public void SelectCharacters(List<Character> characterList)
        {
            mSelectedCharacters.UnregisterAll();
            foreach (Character c in characterList)
            {
                mSelectedCharacters.Register(c);
            }
        }

        //This function draws the whole game state
        public void Draw(SpriteBatch sb)
        {
            foreach (var c in mPlayerCharacters)
            {
                c.Draw(sb);
            }
        }

        //This function updates the game state
        public void UpdateState()
        {
            foreach (var c in mPlayerCharacters)
            {
                c.UpdateState();
            }

        }
        //This function sends a command to all selected characters
        public void SendCommand(ICommandFactory cf)
        {
            this.mSelectedCharacters.SendCommand(cf);
        }
    }
}

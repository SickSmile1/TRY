using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.AI
{
    internal sealed class KiActionManager
    {
        // Setting up an action list; Write down all desired behaviors we want
        // the AI to have. While iterating over the array, it's possible to let
        // actions with higher priority execute first.
        private readonly IKiActions[] mActions;

        public KiActionManager(IKiActions[] actions)
        {
            mActions = actions;
        }

        public void UpdateActionList(GameTime g, HashSet<ICharacter> players, List<DistractionObject> rab)
        {
            foreach (var action in mActions)
            {
                // checks of certain conditions are fulfilled before executing the action item
                // for example: Only start evading player characters of enemy HP is less than 30%
                if (action.CanExecute())
                {
                    action.Execute(g, players, rab);
                }

                // If Blocking is true, further action items will not be executed.
                // for example: If an enemy notices a distraction object they will not move.
                if (action.Blocking)
                {
                    action.Blocking = false;
                    break;
                }
            }
        }
    }
}

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.AI
{
    internal interface IKiActions
    {
        bool CanExecute();
        void Execute(GameTime g, HashSet<ICharacter> players, List<DistractionObject> rab);
        bool Blocking { get; set; }
    }
}

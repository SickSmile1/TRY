using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.AI
{
    internal sealed class MoveToTarget : IKiActions
    {
        /// <summary>
        /// This class takes charge of enemy movements towards a certain destination.
        /// </summary>
        
        private readonly ICharacter mEnemy;
        private Vector2 mSetTarget;

    public MoveToTarget(ICharacter e)
        {
            mEnemy = e;
            Blocking = false;
            mSetTarget = Vector2.Zero;
        }

        public bool Blocking { get; set; }

        public void SetTarget(Vector2 dest)
        {
            mSetTarget = dest;
        }

        public bool CanExecute()
        {
            return true;
        }
        public void Execute(GameTime g, HashSet<ICharacter> players, List<DistractionObject> rab)
        {
            if (mSetTarget == Vector2.Zero) return;
            mEnemy.Destination = mSetTarget;
            mSetTarget = Vector2.Zero;
            Blocking = true;
        }
    }
}

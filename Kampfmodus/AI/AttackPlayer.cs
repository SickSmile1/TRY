using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.AI
{
    /// <summary>
    /// This class represents the attacking behaviour of the enemies.
    /// Aliens with more than 10% will attack the closest target in range.
    /// If a rabbit is placed, aliens rather observe it than attacking.
    /// </summary>
    sealed class AttackPlayer : IKiActions
    {
        private readonly ICharacter mEnemy;

        public AttackPlayer(ICharacter e)
        {
            mEnemy = e;
            Blocking = false;
        }

        public bool Blocking { get; set; }
        public bool CanExecute()
        {
            return mEnemy.Health > 10;
        }

        public void Execute(GameTime g, HashSet<ICharacter> players, List<DistractionObject> rab)
        {
            if (rab.Any())
            {
                // check if distraction rabbit is in range
                foreach (var distract in rab)
                {
                    var position = new Vector2(distract.ObjectArea.X, distract.ObjectArea.Y);
                    if (Vector2.Distance(position, mEnemy.MidPoint) < mEnemy.Vision)
                    {
                        Blocking = true;
                        return;
                    }
                }
            }

            // attack the closest target in range
            if (mEnemy.Active)
            {
                mEnemy.Weapon?.UseWeapon(g, mEnemy.MidPoint);
            }
        }
    }
}

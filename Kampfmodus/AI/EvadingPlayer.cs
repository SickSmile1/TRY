using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.AI
{
    internal sealed class EvadePlayer : IKiActions
    {
        /// <summary>
        /// This class takes charge of the fleeing behaviour of aliens using
        /// vector mirroring.
        /// All Aliens will start evading if their HP is less than 30%.
        /// </summary>
 
        private readonly ICharacter mEnemy;

        public EvadePlayer(ICharacter e)
        {
            mEnemy = e;
            Blocking = false;
        }

        public bool Blocking { get; set; }
        private ICharacter ClosestTarget(ICollection<ICharacter> enemies)
        {
            if (enemies.Count > 0)
            {
                var closestEnemy = enemies.First();
                // Search for the closest player character.
                foreach (var enemy in enemies)
                {
                    if (Vector2.Distance(enemy.MidPoint, mEnemy.MidPoint) <
                        Vector2.Distance(closestEnemy.MidPoint, mEnemy.MidPoint))
                    {
                        closestEnemy = enemy;

                    }
                }
                return closestEnemy;
            }

            return null;
        }

        // Calculates the vector of a character towards the enemy. Then adds the vector
        // on top of the current enemy's position.
        private Vector2 Mirror(ICollection<ICharacter> enemies)
        {
            var target = ClosestTarget(enemies);
            var mirrorVector = new Vector2(mEnemy.Position.X - target.Position.X,
                mEnemy.Position.Y - target.Position.Y);
            var mirror = new Vector2(mEnemy.Position.X + mirrorVector.X,
                mEnemy.Position.Y + mirrorVector.Y);
            return mirror;
        }

        public bool CanExecute()
        {           
            if (mEnemy.Health <= 30)
            {
                return true;
            }

            return false;
        }
        public void Execute(GameTime g, HashSet<ICharacter> players, List<DistractionObject> rab)
        {
            var target = ClosestTarget(players);
            if (target == null || (Vector2.Distance(target.MidPoint, mEnemy.MidPoint) > mEnemy.Vision)) return;
            mEnemy.Destination = Mirror(players);
            Blocking = true;
        }
    }
}

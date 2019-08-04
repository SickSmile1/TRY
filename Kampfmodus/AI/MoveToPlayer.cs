using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.AI
{
    internal sealed class MoveToPlayer : IKiActions
    {
        /// <summary>
        /// This class takes charge of enemy movements towards characters.
        /// If a playable character is in range, aliens will start chasing him. 
        /// </summary>
        
        private readonly ICharacter mEnemy;

        public MoveToPlayer(ICharacter e)
        {
            mEnemy = e;
            Blocking = false;
        }

        public bool Blocking { get; set; }

        private ICharacter ClosestTarget(HashSet<ICharacter> enemies)
        {
            var closestEnemy = enemies.First();
            // Search for the closest enemy.
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

        public bool CanExecute()
        {
            return mEnemy.Health >= 30;
        }

        public void Execute(GameTime g,
            HashSet<ICharacter> players,
            List<DistractionObject> rab)
        {
            if (players.Any())
            {
                var target = ClosestTarget(players);
                if (Vector2.Distance(target.MidPoint, mEnemy.MidPoint) < mEnemy.Vision)
                {
                    // Supervisor will not chase players.
                    if (mEnemy.Texture == "Supervisor")
                    {
                        return;
                    }

                    mEnemy.Destination = target.Position;
                    Blocking = true;
                }
            }
        }
    }
}
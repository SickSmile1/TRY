using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.AI
{
    internal sealed class Spread : IKiActions
    {
        /// <summary>
        /// Enemies, who are not assigned with a task,
        /// randomly moves within their vision range.
        /// </summary>

        private readonly ICharacter mEnemy;
        private float mSecondPassed;

        public Spread(ICharacter e)
        {
            mEnemy = e;
            Blocking = false;
            mSecondPassed = 0;
        }

        public bool Blocking { get; set; }

        private void SetDestination()
        {
            mEnemy.Destination = mEnemy.Position + 
                                 Utilities.Random(-mEnemy.Vision, mEnemy.Vision);
            mSecondPassed = 0;
        }
        public bool CanExecute()
        {
            return (mEnemy.Destination == null);
        }
        public void Execute(GameTime g, HashSet<ICharacter> players, List<DistractionObject> rab)
        {
            mSecondPassed += g.ElapsedGameTime.Milliseconds / 1000f;
            if (mSecondPassed > 1)
            {
                SetDestination();
                Blocking = true;
            }
        }
    }
}
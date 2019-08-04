using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.AI
{
    sealed class Flocking : IKiActions
    {
        private readonly ICharacter mEnemy;

        public Flocking(ICharacter enemy)
        {
            mEnemy = enemy;
        }

        public bool CanExecute()
        {
            return true;
        }
        public bool Blocking { get; set; }

        private Vector2 ComputeAlignment(IEnumerable<ICharacter> neighbor)
        {
            var resultant = new Vector2(0,0);
            var neighborCount = 0;
            foreach (var nei in neighbor)
            {
                if (mEnemy != nei)
                {
                    resultant += nei.Velocity;
                    neighborCount += 1;
                }
            }

            if (resultant == Vector2.Zero || neighborCount == 0 )
            {
                return resultant;
            }

            resultant.X /= neighborCount;
            resultant.Y /= neighborCount;
            resultant = Vector2.Normalize(resultant);
            return resultant;
        }

        private Vector2 ComputeCohesion(IEnumerable<ICharacter> neighbor)
        {
            var resultant = new Vector2(0, 0);
            var neighborCount = 0;
            foreach (var nei in neighbor)
            {
                if (mEnemy != nei)
                {
                    resultant += nei.Position;
                    neighborCount += 1;
                }
            }

            if (resultant == Vector2.Zero || neighborCount == 0)
            {
                return resultant;
            }

            resultant.X /= neighborCount;
            resultant.Y /= neighborCount;
            resultant -= mEnemy.Position;
            resultant = Vector2.Normalize(resultant);
            return resultant;
        }

        private Vector2 ComputeSeparation(IEnumerable<ICharacter> neighbor)
        {
            var resultant = new Vector2(0, 0);
            var neighborCount = 0;
            foreach (var nei in neighbor)
            {
                if (mEnemy != nei && Vector2.Distance(nei.MidPoint, mEnemy.MidPoint) < 100)
                {
                    resultant.X += nei.Position.X - mEnemy.Position.X;
                    resultant.Y += nei.Position.Y - mEnemy.Position.Y;
                    neighborCount += 1;
                }
            }

            if (resultant == Vector2.Zero || neighborCount == 0)
            {
                return resultant;
            }

            if (neighborCount != 0 && resultant != Vector2.Zero)
            {
                resultant /= neighborCount;
                resultant *= -1;
                resultant = Vector2.Normalize(resultant);
            }

            return resultant;
        }
        public void Execute(GameTime g, HashSet<ICharacter> neighbor, HashSet<ICharacter> players, List<DistractionObject> rab)
        {
            // Computing the three forces; Then computing the resultant vector
            var alignment = ComputeAlignment(neighbor);
            var cohesion = ComputeCohesion(neighbor);
            var separation = ComputeSeparation(neighbor);
            // set weight of forces
            var resultant = cohesion * 2 + separation * 4 + alignment * 3;
            var length = (float) Math.Sqrt(resultant.X * resultant.X + resultant.Y * resultant.Y);
            // special case division by zero
            if (Math.Abs(length) > 0)
            {
                resultant = new Vector2(resultant.X / length, resultant.Y / length);
            }

            mEnemy.Velocity = resultant;
            mEnemy.Position += resultant * 3;
            Blocking = true;
        }
    }
}

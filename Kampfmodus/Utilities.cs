using System;
using Microsoft.Xna.Framework;

namespace TRY.Kampfmodus
{
    static class Utilities
    {
        public static Vector2 Random(int minVision, int maxVision)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            var destX = rnd.Next(minVision, maxVision);
            var destY = rnd.Next(minVision, maxVision);
            return new Vector2(destX, destY);
        }
    }
}

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TRY.Kampfmodus.Pathfinding
{
    class GridNode
    {
        public GridNode(Vector2 position)
        {
            Position = position;
            Neighbours = new Dictionary<GridNode, float>();
        }
        public Vector2 Position { get; set; }
        public Dictionary<GridNode, float> Neighbours { get; }
    }
}

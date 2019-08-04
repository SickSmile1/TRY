using Microsoft.Xna.Framework;

namespace TRY.Kampfmodus.Collision
{
    class Wall: IStaticCollider
    {
        public Wall(Rectangle area)
        {
            ObjectArea = area;
        }
        public Rectangle ObjectArea { get; }
    }
}

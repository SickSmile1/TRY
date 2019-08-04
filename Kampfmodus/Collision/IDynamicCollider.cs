using Microsoft.Xna.Framework;

namespace TRY.Kampfmodus.Collision
{
    interface IDynamicCollider
    {
        Rectangle ObjectArea { get; }
        void CollidesWith(IDynamicCollider collider);
        void CollidesWith(IStaticCollider collider);

        CollisionManager.HasMoved ObjectMoved { get; set; }
    }
}

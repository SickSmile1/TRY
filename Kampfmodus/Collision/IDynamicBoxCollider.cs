using System;
using Microsoft.Xna.Framework;

namespace TRY.Kampfmodus.Collision
{
    interface IDynamicBoxCollider
    {
        Rectangle ObjectArea { get; }
        void CollidesWith(IDynamicBoxCollider boxCollider);
        void CollidesWith(IStaticCollider collider);

        CollisionManager.HasMoved ObjectMoved { get; set; }
    }
}

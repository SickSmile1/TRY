using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tiled;

namespace Game1
{
    // For testing purposes a simple player controller
    public class PlayerController : Component, IUpdatable
    {
        private TiledMapMover _mover;
        private BoxCollider _boxCollider;
        readonly TiledMapMover.CollisionState _collisionState = new TiledMapMover.CollisionState();
        private Vector2 _movement;


        public override void onAddedToEntity()
        {
            _mover = this.getComponent<TiledMapMover>();
            _boxCollider = entity.getComponent<BoxCollider>();
        }

        public void update()
        {
            if (Input.isKeyDown(Keys.Right))
            {
                _movement.X = 3;
            }
            else if (Input.isKeyDown(Keys.Left))
            {
                _movement.X = -3;
            }
            else if (Input.isKeyDown(Keys.Up))
            {
                _movement.Y = -3;
            }
            else if (Input.isKeyDown(Keys.Down))
            {
                _movement.Y = 3;
            }
            else
            {
                _movement.X = 0;
                _movement.Y = 0;
            }
            _mover.move(_movement, _boxCollider, _collisionState);


        }
    }
}

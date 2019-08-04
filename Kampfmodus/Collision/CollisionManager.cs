using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Structure;
using ColliderParams = System.Tuple<Microsoft.Xna.Framework.Vector2, float, Microsoft.Xna.Framework.Point[]>;

namespace TRY.Kampfmodus.Collision
{
    internal sealed class CollisionManager
    {
        public delegate void HasMoved(IDynamicCollider x);

        private readonly Dictionary<IDynamicCollider, ColliderParams> mDynamicColliders;
        private readonly HashSet<IDynamicCollider>[,] mCollisionField;

        private QField<IStaticCollider> mStaticColliderField;

        private readonly int mTileSize;

        public CollisionManager(Map map, QField<IStaticCollider> staticColliderField)
        {
            mStaticColliderField = staticColliderField;
            var collisionObjects = map.TiledMapObjects;
            foreach (var collisionObject in collisionObjects)
            {
                AddCollider(new Wall(new Rectangle(collisionObject.Position.ToPoint(), new Point((int)collisionObject.Size.Width, (int)collisionObject.Size.Height))));
            }
            mDynamicColliders = new Dictionary<IDynamicCollider, ColliderParams>();

            mTileSize = map.TileWidth / 2;

            mCollisionField = new HashSet<IDynamicCollider>[map.Width * 2, map.Height * 2];

            for (var i = 0; i < mCollisionField.GetLength(0); i++)
            {
                for (var j = 0; j < mCollisionField.GetLength(1); j++)
                {
                    mCollisionField[i, j] = new HashSet<IDynamicCollider>();
                }
            }
        }

        public void AddCollider(IDynamicCollider collide)
        {
            mDynamicColliders.Add(
                collide,
                new ColliderParams(
                    collide.ObjectArea.Center.ToVector2(),
                    collide.ObjectArea.Width / 2.0f,
                    GetSquares(collide.ObjectArea)));
            collide.ObjectMoved += OnMove;

            try
            {
                AddToCollisionField(collide, mDynamicColliders[collide].Item3);
            }
            catch
            {
                throw new IndexOutOfRangeException("Spielobjekt außerhalb von Definitionsbereich.");
            }
        }

        public void RemoveCollider(IDynamicCollider collide)
        {
            if (!mDynamicColliders.ContainsKey(collide)) return;
            foreach (var collisionFieldPoint in mDynamicColliders[collide].Item3)
            {
                mCollisionField[collisionFieldPoint.X, collisionFieldPoint.Y].Remove(collide);
            }

            mDynamicColliders.Remove(collide);
            collide.ObjectMoved = null;
        }

        public void AddCollider(IStaticCollider collider)
        {
            mStaticColliderField.AddElementAt(collider, collider.ObjectArea);
        }

        private void AddToCollisionField(IDynamicCollider collider, Point[] squares)
        {
            foreach (var square in squares)
            {
                try
                {
                    mCollisionField[square.X, square.Y].Add(collider);
                }
                catch
                {
                    Console.WriteLine("Problem in der Kollision!");
                    throw new IndexOutOfRangeException("Position des Objekts liegt außerhalb der Grenzen der Map!");
                }
            }
        }

        private void RemoveFromCollisionField(IDynamicCollider collider, Point[] squares)
        {
            foreach (var square in squares)
            {
                mCollisionField[square.X, square.Y].Remove(collider);
            }
        }

        private Point[] GetSquares(Rectangle rect)
        {
            int startx = rect.X / mTileSize;
            int starty = rect.Y / mTileSize;
            int stopx = (rect.X + rect.Width) / mTileSize;
            int stopy = (rect.Y + rect.Height) / mTileSize;
            int xwidth = (stopx - startx + 1);
            int ywidth = (stopy - starty + 1);
            var squares = new Point[xwidth * ywidth];
            //Along the x axis, find all squares.
            for (var x = 0; x < xwidth; x++)
            {
                for (var y = 0; y < ywidth; y++)
                {
                    squares[y * xwidth + x].X = x + startx;
                    squares[y * xwidth + x].Y = y + starty;
                }
            }
            return squares;
        }

        private void OnMove(IDynamicCollider collide)
        {
            var newSquares = GetSquares(collide.ObjectArea);
            var colliderParams = mDynamicColliders[collide];
            var oldSquares = colliderParams.Item3;

            mDynamicColliders[collide] =
                new ColliderParams(collide.ObjectArea.Center.ToVector2(), colliderParams.Item2, newSquares);
            if (!oldSquares.Equals(newSquares))
            {
                RemoveFromCollisionField(collide, oldSquares);
                try
                {
                    AddToCollisionField(collide, newSquares);
                }
                catch
                {
                    foreach (var collisionFieldPoint in mDynamicColliders[collide].Item3)
                    {
                        try
                        {
                            mCollisionField[collisionFieldPoint.X, collisionFieldPoint.Y].Remove(collide);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    mDynamicColliders.Remove(collide);
                    collide.ObjectMoved = null;
                }
            }
        }

        private bool DoTheyCollide(ColliderParams collider1, ColliderParams collider2)
        {
            return (collider1.Item1 - collider2.Item1).Length() < (collider1.Item2 + collider2.Item2);
        }

        private void CheckStaticColliders()
        {
            foreach (var collider in mDynamicColliders.Keys)
            {
                var possibleColliders = mStaticColliderField.GetAllElementsNear(collider.ObjectArea);

                foreach (var staticCollider in possibleColliders)
                {
                    if (staticCollider.ObjectArea.Intersects(collider.ObjectArea))
                    {
                        collider.CollidesWith(staticCollider);
                    }
                }
            }
        }

        private void CheckDynamicColliders()
        {
            //Dictionary with all occured collisions, so no multiple collisions occur
            Dictionary<IDynamicCollider, List<IDynamicCollider>> hasCollided =
                new Dictionary<IDynamicCollider, List<IDynamicCollider>>();
            foreach (var dynamicCollidersKey in mDynamicColliders.Keys)
            {
                hasCollided.Add(dynamicCollidersKey, new List<IDynamicCollider>());
            }

            //Iterate over all squares of the collision Field and find Collisions.
            for (var x = 0; x < mCollisionField.GetLength(0); x++)
            {
                for (var y = 0; y < mCollisionField.GetLength(1); y++)
                {
                    if (mCollisionField[x, y].Count > 1)
                    {
                        var colliderList = new List<IDynamicCollider>(mCollisionField[x, y]);
                        for (int i = 0; i < colliderList.Count; i++)
                        {
                            var collider = colliderList[i];
                            if (!mDynamicColliders.ContainsKey(collider)) continue;
                            var colliderParams = mDynamicColliders[collider];
                            for (int j = i; j < colliderList.Count; j++)
                            {
                                var collider2 = colliderList[j];
                                if (collider == collider2
                                    || hasCollided[collider].Contains(collider2)
                                    || !mDynamicColliders.ContainsKey(collider2)
                                    ) continue;
                                var colliderParams2 = mDynamicColliders[collider2];
                                if (DoTheyCollide(colliderParams, colliderParams2))
                                {
                                    collider.CollidesWith(collider2);
                                    collider2.CollidesWith(collider);
                                    hasCollided[collider].Add(collider2);
                                    hasCollided[collider2].Add(collider);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Update()
        {
            CheckStaticColliders();
            CheckDynamicColliders();
        }
    }
}

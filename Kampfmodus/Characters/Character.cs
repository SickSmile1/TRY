using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Collision;
using TRY.Kampfmodus.Commands;
using TRY.Kampfmodus.Weapons;

namespace TRY.Kampfmodus.Characters
{
    /// <summary>
    /// This class represents all playable characters and aliens
    /// </summary>
    internal sealed class Character : ICharacter
    {
        private ICommand mCurrentCommand;

        // This is the Variable in which the Character Position is stored. 
        // Use the Property "Position" to get its Value
        private Vector2 mPosition;
        private Vector2 mPreviousVector2;

        private Vector2? mDestination;

        // Use the Property "Midpoint" to get its Value
        private Vector2 mMidpoint;

        private int mDirection;

        // Shows the need for Midpoint to be recalculated
        private bool mMidpointRecalculate;

        // Shows if the Character has moved Since the last frame
        private bool mMoved;

        // This stores the Characters Character area as a Rectangle. 
        // Use the Property "CharacterArea" to get its Value
        private Rectangle mCharacterArea;
        private Rectangle mObjectArea;

        private float mPathfindingRecalculateTimer;
        private float mPathfindingRecalculateThreshold = 0.5f;

        private string mTextureIdentifier;
        private bool mPushed;

        private List<Vector2> mWay;

        public Character(string textureIdentifier, Point position, Point sizeInPixels,
                         Pathfinding.Pathfinding pathfinding, float movementSpeed = 0.15f,
                         bool player = true, int health = 100, int playerLevel = 0, int vision = 0)
        {
            Velocity = Vector2.Zero;
            Player = player;
            var enemy = !player;
            PlayerLevel = playerLevel;
            mPushed = false;
            mTextureIdentifier = textureIdentifier;
            IsBeingRevived = false;
            Pathfinding = pathfinding;

            if (!enemy)
            {
                var astronaut = new[] { 6, 4, 6, 6, 4, 6 };
                CharacterAnimation = new Animation(textureIdentifier, new Vector2(sizeInPixels.X, sizeInPixels.Y), new List<int>(astronaut));
            }

            mDirection = 0;

            mPosition = new Vector2(position.X, position.Y);

            Speed = movementSpeed;
            mMidpointRecalculate = false;
            mDestination = null;
            mObjectArea = new Rectangle((int)mPosition.X, (int)mPosition.Y + sizeInPixels.Y - sizeInPixels.X, sizeInPixels.X, sizeInPixels.X);
            mCharacterArea = new Rectangle((int)mPosition.X, (int)mPosition.Y,
                                           sizeInPixels.X,
                                           sizeInPixels.Y);
            // Determine center of the texture.
            mMidpoint = new Vector2(Position.X + (float)CharacterArea.Width / 2,
                                    mPosition.Y + (float)CharacterArea.Height / 2);

            Health = health;
            MaxHealth = health;
            Active = true;
            Vision = vision;

            mMoved = false;
            mPreviousVector2 = MidPoint;
        }

        public int PlayerLevel { get; set; }
        public bool Player { get; set; }
        public bool Active { get; set; }
        public bool IsBeingRevived { get; set; }
        public string Id { get; set; }
        public int Vision { get; }
        public Pathfinding.Pathfinding Pathfinding { get; set; }
        public Animation CharacterAnimation { get; set; }
        public IWeapon Weapon { get; set; }
        public IAbility Ability { get; set; }
        public IAbility SupportAbility { get; set; }
        public CollisionManager.HasMoved ObjectMoved { get; set; }
        private Vector2 Velocity { get; set; }
        public int MaxHealth { get; set; }
        public float DeathTimer { get; set; }
        public int Health { get; set; }
        private float Speed { get; }
        public Rectangle ObjectArea => mObjectArea;

        public Vector2? Destination
        {
            get => mDestination;
            set
            {
                mDestination = value;
                if (value == null) return;
                mWay = Pathfinding.FindWay(MidPoint, value.Value);
                if (mWay.Count == 0)
                {
                    Destination = null;
                }
            }
        }

        public string Texture
        {
            get => mTextureIdentifier;
            set
            {
                mTextureIdentifier = value;
                mTextureIdentifier = Texture;
            }
        }

        /// Character's position. this is at the far-left of the Texture.
        public Vector2 Position
        {
            get => mPosition;
            set
            {
                CalculatePosition(value.X, value.Y);
                mMidpointRecalculate = true;
            }
        }

        /// Area of the Character. It's the Rectangle in which the Texture is drawn.
        public Rectangle CharacterArea
        {
            get => mCharacterArea;
        }

        /// Returns the Midpoint of the Character. Refreshes only after Movement.
        public Vector2 MidPoint
        {
            get
            {
                if (mMidpointRecalculate)
                {
                    mMidpointRecalculate = false;
                    mMidpoint.X = Position.X + (float)mObjectArea.Width / 2;
                    mMidpoint.Y = mPosition.Y + (float)mObjectArea.Height / 2;
                }
                return mMidpoint;
            }
            set
            {
                CalculatePosition(
                    value.X - (float)mObjectArea.Width / 2,
                    value.Y + (float)mObjectArea.Height / 2 - mCharacterArea.Height);
                mMidpoint = value;
                mMidpointRecalculate = false;
            }
        }

        private void CalculatePosition(float x, float y)
        {
            try
            {
                int xint = (int)x;
                int yint = (int)y;
                mCharacterArea.X = xint;
                mCharacterArea.Y = yint;
                mObjectArea.X = xint;
                mObjectArea.Y = yint + mCharacterArea.Height - mCharacterArea.Width;
                mPosition.X = x;
                mPosition.Y = y;
                mMoved = true;
            }
            catch
            {
                Console.WriteLine("Problem in der Positionierung!");
            }
        }

        /// <summary>
        /// Function that moves the Character to Destination.
        /// </summary>
        /// <param name="gameTime"></param>
        private void Move(GameTime gameTime)
        {
            // Only move when there is a Destination
            if (Destination != null)
            {
                var destination = mWay[0];

                // Compute the line the character has to move on
                var direction = destination - MidPoint;
                var oldDirection = mDirection;
                // Get the direction for the animation.
                var currentDirection = Vector2.Normalize(direction);
                var currentGradient = currentDirection.Y / currentDirection.X;
                // 0:up, 1:down, 2:left, 3:right, 4:idle animation.
                if (currentDirection.X > 0 && currentDirection.Y > 0)
                {
                    mDirection = currentGradient > 0.5 ? 1 : 3;
                }
                else if (currentDirection.X > 0 && currentDirection.Y < 0)
                {
                    mDirection = currentGradient < -0.5 ? 0 : 3;
                }
                else if (currentDirection.X < 0 && currentDirection.Y > 0)
                {
                    mDirection = currentGradient < -0.5 ? 1 : 2;
                }
                else if (currentDirection.X < 0 && currentDirection.Y < 0)
                {
                    mDirection = currentGradient > 0.5 ? 0 : 2;
                }
                // Reset animation to avoid flickering.
                if (mDirection != oldDirection)
                {
                    CharacterAnimation?.ResetAnimation();
                }
 
                // If the Character is at the destination, stop.
                if (direction == Vector2.Zero)
                {
                    Velocity = Vector2.Zero;
                    Destination = null;
                    return;
                }

                // Play walking sounds when character starts to move
                if (Player) Game1.sSoundEffectInstance[0].Play();

                // Calculate new position based on speed and time passed.
                var newMidPoint = MidPoint + currentDirection * Speed *
                            gameTime.ElapsedGameTime.Milliseconds;

                // Prevent overshooting by comparing distances between steps.
                if ((MidPoint - destination).Length() <= (newMidPoint - MidPoint).Length())
                {
                    MidPoint = destination;
                    mWay.RemoveAt(0);
                    if (mWay.Count == 0)
                    {
                        Destination = null;
                    }

                    // Stop the footstep once destination has been reached.
                    Game1.sSoundEffectInstance[0].Stop();
                }
                else
                {
                    MidPoint = newMidPoint;
                }
            }
            else
            {
                mDirection = 4;
            }
        }

        /// <summary>
        /// Draws the character in a spritebatch
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="textureManager"></param>
        public void Draw(SpriteBatch sb,TextureManager textureManager)
        {
            if (Ability != null)
            {
                if (Ability.SecondsPassed < Ability.Duration && Ability.Id == "RoundKick")
                {
                    return;
                }
            }
            CharacterAnimation.Draw(sb, Position, textureManager);
            Weapon?.Draw(sb,textureManager,MidPoint);
        }

        /// <summary>
        /// This function updates the state of the character, i.e. follows a given command
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            Ability?.Update(gameTime);

            if (Active && Health > 0)
            {
                mCurrentCommand?.Execute();
                Move(gameTime);


                mPathfindingRecalculateTimer += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                if (mPathfindingRecalculateTimer>mPathfindingRecalculateThreshold)
                {
                    if (Vector2.Distance(mPreviousVector2, MidPoint) < 10)
                    {
                        Destination = mDestination;
                    }
                    mPreviousVector2.X = MidPoint.X;
                    mPreviousVector2.Y = MidPoint.Y;
                    mPathfindingRecalculateTimer = 0;
                }

                if (mPushed)
                {
                    if (mWay.Count > 1 && Pathfinding.IsVisible(MidPoint, mWay[1]))
                    {
                        mWay.RemoveAt(0);
                    }

                    mPushed = false;
                }

                SupportAbility?.Update(gameTime);

                Weapon?.UseWeapon(gameTime, MidPoint);

                if (Player)
                {
                }

                SupportAbility?.UseAbility(MidPoint);

                var fps = mDirection == 0 || mDirection == 1 ? 5 : 13;
                CharacterAnimation.UpdateAnimation(gameTime, fps, mDirection);

            }

            if (Health <= 0)
            {
                Ability?.Terminate();
                if (Player) Game1.sSoundEffectInstance[7].Play();
                if (!IsBeingRevived)
                {
                    DeathTimer += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                }
                CharacterAnimation.UpdateAnimation(gameTime, 6/8f, 5, false);
            }
            else
            {
                DeathTimer = 0;
            }

            if (mMoved)
            {
                ObjectMoved?.Invoke(this);
                mMoved = false;
            }
        }

        /// <summary>
        /// Unregister a given command.
        /// </summary>
        public void AbortCommand()
        {
            mCurrentCommand = null;
        }

        /// <summary>
        /// This function takes up a new command.
        /// </summary>
        /// <param name="x"></param>
        public void UpdateCommand(ICommand x)
        {
            mCurrentCommand = x;
        }

        public void CollidesWith(IDynamicCollider collider)
        {
            if (collider is Projectile)
            {
                return;
            }
            // Try to stop the pushing around
            if (Destination != null)
            {
                if (collider.ObjectArea.Contains(Destination.Value))
                {
                    Destination = null;
                }

                mPushed = true;
            }
            // Stop pushing dead people around
            if (Health > 0)
            {
                var colliderMidPoint = collider.ObjectArea.Center;
                var distance = colliderMidPoint.ToVector2() - MidPoint;
                if (distance.Equals(Vector2.Zero))
                {
                    distance = Vector2.UnitX;
                }
                distance.Normalize();
                MidPoint -= distance;
            }
        }

        public void CollidesWith(IStaticCollider collider)
        {
            if (collider is Door door)
            {
                if (Player)
                {
                    door.Open();
                }
                else
                {
                    if (Weapon != null)
                    {
                        door.Health -= Weapon.Damage;
                    }
                    else
                    {
                        door.Health -= 20;
                    }
                }
            }
            while (mObjectArea.Intersects(collider.ObjectArea))
            {
                var nearestPoint = new Point(0, 0)
                {
                    X = Math.Max(collider.ObjectArea.X,
                    Math.Min((int)MidPoint.X, collider.ObjectArea.X + collider.ObjectArea.Width)),
                    Y = Math.Max(collider.ObjectArea.Y,
                    Math.Min((int)MidPoint.Y, collider.ObjectArea.Y + collider.ObjectArea.Height))
                };
                var distance = nearestPoint.ToVector2() - MidPoint;
                distance.Normalize();
                MidPoint -= distance;
            }
        }
    }
}
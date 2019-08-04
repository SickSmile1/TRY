using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus
{
    /// <summary>
    /// Simple class for a moveable camera
    /// </summary>
    internal sealed class Camera
    {
        private const float CameraSpeed = 0.05f;
        private Rectangle? mLimits;
        public readonly Viewport mViewport;
        private Vector2 mTargetPosition;
        private Matrix mTransform;
        private Matrix mInverted;


        /// <summary>
        /// Sets values for camera movement speed, camera position and map size. Initializes Events.
        /// </summary>
        /// <param name="viewport"> Current view window </param>
        /// <param name="maxHorizontal"> Maximum vertical camera position </param>
        /// <param name="maxVertical"> Maximum horizontal camera position </param>
        public Camera(Viewport viewport, int maxHorizontal, int maxVertical)
        {
            mTransform = Matrix.Identity;
            mInverted = Matrix.Identity;
            mViewport = viewport;
            mTargetPosition = new Vector2(0, 0);
            Position = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
            Zoom = 1f;
            Scroll = 1f;
            Limits = new Rectangle(0, 0, maxHorizontal, maxVertical);

            InputManager.Instance.MoveCameraUp += OnMoveCameraUp;
            InputManager.Instance.MoveCameraDown += OnMoveCameraDown;
            InputManager.Instance.MoveCameraLeft += OnMoveCameraLeft;
            InputManager.Instance.MoveCameraRight += OnMoveCameraRight;
            InputManager.Instance.ZoomCamera += OnZoomCamera;
        }

        private void OnMoveCameraRight(object sender, EventArgs eventArgs)
        {
            MoveCamera(new Vector2(200, 0));
        }
        private void OnMoveCameraLeft(object sender, EventArgs eventArgs)
        {
            MoveCamera(new Vector2(-200, 0));
        }
        private void OnMoveCameraDown(object sender, EventArgs eventArgs)
        {
            MoveCamera(new Vector2(0, 200));
        }
        private void OnMoveCameraUp(object sender, EventArgs eventArgs)
        {
            MoveCamera(new Vector2(0, -200));
        }

        private void OnZoomCamera(object sender, EventArgs eventArgs)
        {
            var e = (InputManager.ScrollEventArgs)eventArgs;
            ZoomCamera(e.ScrollWheelValue);
        }

        public Vector2 TargetPosition
        {
            private get => mTargetPosition;
            set
            {
                mTargetPosition = value;

                // If there's a limit set and the camera is not transformed clamp position to limits
                if (Limits != null)
                {
                    mTargetPosition.X = MathHelper.Clamp(mTargetPosition.X, Limits.Value.X, Limits.Value.X + Limits.Value.Width - mViewport.Width);
                    mTargetPosition.Y = MathHelper.Clamp(mTargetPosition.Y, Limits.Value.Y, Limits.Value.Y + Limits.Value.Height - mViewport.Height);
                }
            }
        }

        private Vector2 Position { get; }
        private float Zoom { get; set; }
        private float Scroll { get; set; }

        public Matrix Transform
        {
            get => mTransform;
            private set
            {
                mTransform = value;
                mInverted = Matrix.Invert(mTransform);
            }
        }

        private Rectangle? Limits
        {
            get => mLimits;
            set
            {
                if (value != null)
                {
                    // Assign limit but make sure it's always bigger than the viewport
                    mLimits = new Rectangle
                    {
                        X = value.Value.X,
                        Y = value.Value.Y,
                        Width = Math.Max(mViewport.Width, value.Value.Width),
                        Height = Math.Max(mViewport.Height, value.Value.Height)
                    };
                }
                else
                {
                    mLimits = null;
                }
            }
        }


        /// <summary>
        /// Calculates the view space with the current position of the camera, its target position and the zoom amount
        /// </summary>
        /// <returns> view matrix </returns>
        private void GetViewMatrix()
        {
            Transform = 
                Matrix.CreateTranslation(new Vector3(-TargetPosition, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1f)) *
                Matrix.CreateTranslation(new Vector3(Position, 0.0f));

        }

        /// <summary>
        /// moves camera. Movable with arrow keys and mouse.
        /// </summary>
        /// <param name="direction"> the direction the camera is supposed to move in </param>
        public void MoveCamera(Vector2 direction)
        {
            TargetPosition = new Vector2(
                (TargetPosition.X + direction.X * CameraSpeed),
                (TargetPosition.Y + direction.Y * CameraSpeed));
            GetViewMatrix();
        }

        /// <summary>
        /// Zoom camera in and out by calculating if current scroll wheel value is greater or less than the saved scroll wheel value Scroll
        /// changes the zoom value accordingly
        /// </summary>
        /// <param name="scrollWheelValue"> current mouse screen scroll wheel value </param>
        private void ZoomCamera(float scrollWheelValue)
        {
            if (scrollWheelValue > Scroll && Zoom < 1f)
            {
                Zoom += 0.05f;
                Scroll = scrollWheelValue;

            }
            else if (scrollWheelValue < Scroll && Zoom > 0.45f)
            {
                Zoom -= 0.05f;
                Scroll = scrollWheelValue;
            }
            GetViewMatrix();
            Scroll = scrollWheelValue;
        }

        /// <summary>
        /// Calculates world coordinates to screen coordinates
        /// </summary>
        /// <param name="coordinate"> Point to be changed to screen coordinates </param>
        /// <returns></returns>
        internal Point WorldToScreen(Point coordinate)
        {
            var (cx, cy) = coordinate;
            var (sx, sy) = Vector2.Transform(new Vector2(cx, cy), mInverted);
            return new Point((int)sx, (int)sy);
        }
    }
}
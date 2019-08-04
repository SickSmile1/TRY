using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TRY.GameStates;
using TRY.SaveGame;

namespace TRY.Kampfmodus
{
    class InputManager
    {
        private InputManager()
        {

        }
        private static readonly Lazy<InputManager> sLazyInstance = new Lazy<InputManager>(() => new InputManager());
        public static InputManager Instance => sLazyInstance.Value;

        public class ClickEventArgs : EventArgs
        {
            public ClickEventArgs(Point x)
            {
                MousePosition = x;
            }
            public Point MousePosition { get; }
        }

        public sealed class ScrollEventArgs : EventArgs
        {
            public ScrollEventArgs(int scrollWheelValue)
            {
                ScrollWheelValue = scrollWheelValue;
            }
            public int ScrollWheelValue { get; }
        }

        private MouseState mMouseState;
        private KeyboardState mKeyboardState;
        private bool mUiClick;


        private BattleMode mBattleMode;
        private BattleModeState mBattleModeState;

        //Event handlers for Left-Clicking
        public event EventHandler LeftClickStart;
        public event EventHandler LeftClickHold;
        public event EventHandler LeftClickRelease;
        //Event handlers for Right-Clicking
        public event EventHandler RightClickStart;
        public event EventHandler RightClickHold;
        public event EventHandler RightClickRelease;

        // Events for Orders
        public event EventHandler ActivateAbility;

        // Events for Camera movement
        public event EventHandler MoveCameraLeft;
        public event EventHandler MoveCameraRight;
        public event EventHandler MoveCameraUp;
        public event EventHandler MoveCameraDown;
        public event EventHandler ZoomCamera;
        public void Update(Camera camera, UserInterface ui, Game1 game,
            GraphicsDevice graphicsDevice, ContentManager content, BattleModeState bms, Selection selection)
        {
            mBattleModeState = bms;
            var newMouseState = Mouse.GetState();
            var newKeyboardState = Keyboard.GetState();
            var mouseWorldPosition = camera.WorldToScreen(newMouseState.Position);

            // Actions for left click start
            if ((newMouseState.LeftButton == ButtonState.Pressed) &&
                (mMouseState.LeftButton == ButtonState.Released))
            {
                mUiClick = ui.SelectInterface(newMouseState.Position);
                if (!mUiClick)
                {
                    LeftClickStart?.Invoke(this, new ClickEventArgs(mouseWorldPosition));
                }
            }

            //Actions for left Click hold and drag
            else if (newMouseState.LeftButton == ButtonState.Pressed)
            {
                //Only throw dragging event once the mouse has moved
                if (!newMouseState.Equals(mMouseState)&&
                    !mUiClick)
                {
                        LeftClickHold?.Invoke(this, new ClickEventArgs(mouseWorldPosition));
                }
            }

            //Actions for left click release
            else if ((newMouseState.LeftButton == ButtonState.Released) &&
                     (mMouseState.LeftButton == ButtonState.Pressed))
            {
                if (!mUiClick)
                {
                    LeftClickRelease?.Invoke(this, new EventArgs());
                }

            }

            // Actions for right click start
            if ((newMouseState.RightButton == ButtonState.Pressed) &&
                (mMouseState.RightButton == ButtonState.Released))
            {
                RightClickStart?.Invoke(this,new ClickEventArgs(mouseWorldPosition));
            }
            else if ((newMouseState.RightButton == ButtonState.Pressed) &&
                     (mMouseState.RightButton == ButtonState.Pressed))
            {
                RightClickHold?.Invoke(this,new ClickEventArgs(mouseWorldPosition));
            }
            else if ((newMouseState.RightButton == ButtonState.Released) &&
                     (mMouseState.RightButton == ButtonState.Pressed))
            {
                RightClickRelease?.Invoke(this, new ClickEventArgs(mouseWorldPosition));
            }

            if (newKeyboardState.IsKeyDown(Keys.Z) && mKeyboardState.IsKeyUp(Keys.Z))
            {
                var selectedCharacters = selection.SelectedCharacters;
                var totalVect = new Vector2(0,0);
                foreach (var selectedCharacter in selectedCharacters)
                {
                    totalVect += selectedCharacter.MidPoint;
                }

                totalVect /= selectedCharacters.Count; 
                totalVect -= new Vector2(camera.mViewport.Width / 2f, camera.mViewport.Height / 2f);
                camera.TargetPosition = totalVect;
            }

            // call pause menu
                if (newKeyboardState.IsKeyDown(Keys.Escape) && mKeyboardState.IsKeyUp(Keys.Escape))
            { 
                // save the game
                mBattleMode = game.mScreenManager.CurrentScreen(i: 1) as BattleMode;
                if (mBattleMode != null)
                {
                    mBattleMode.CurrentEnemyAlive();
                    game.Achieved.Timer += mBattleMode.mTime;
                    mBattleMode.mTime = 0;
                    mBattleMode.StateName = "BattleModeState";
                    SaveStatetoFile.SaveStateFile(game, mBattleMode.BattleModeState);
                }

                if (game.Console.IsVisible) game.Console.ToggleOpenClose();

                game.mState = new PauseMenuState(game, graphicsDevice, content);
                game.mScreenManager.AddScreen(game.mState);
            }

            if (newKeyboardState.IsKeyDown(Keys.Space) && mKeyboardState.IsKeyUp(Keys.Space))
            {
                ActivateAbility?.Invoke(this, new EventArgs());
            }

            if (newKeyboardState.IsKeyUp(Keys.F1) && mKeyboardState.IsKeyDown(Keys.F1))
            {
                game.Console.ToggleOpenClose();
            }

            // Keyboard interaction used for camera movement
            if (newKeyboardState.IsKeyDown(Keys.Up) || newMouseState.Y < 10)
                MoveCameraUp?.Invoke(this,new EventArgs());

            if (newKeyboardState.IsKeyDown(Keys.Down) || newMouseState.Y > camera.mViewport.Height - 10)
                MoveCameraDown?.Invoke(this, new EventArgs());

            if (newKeyboardState.IsKeyDown(Keys.Left) || newMouseState.X < 10)
                MoveCameraLeft?.Invoke(this, new EventArgs());

            if (newKeyboardState.IsKeyDown(Keys.Right) || newMouseState.X > camera.mViewport.Width - 10)
                MoveCameraRight?.Invoke(this, new EventArgs());

            // Keyboard interaction used for selecting characters
            const int offset = 150;
            if (newKeyboardState.IsKeyDown(Keys.D1))
                ui.SelectInterface(new Point(55 , 55));

            if (newKeyboardState.IsKeyDown(Keys.D2))
                ui.SelectInterface(new Point(55 + offset, 55));
            
            if (newKeyboardState.IsKeyDown(Keys.D3))
                ui.SelectInterface(new Point(55 + 2 * offset, 55));

            if (newKeyboardState.IsKeyDown(Keys.D4))
                ui.SelectInterface(new Point(55 + 3 * offset, 55));

            if (newKeyboardState.IsKeyDown(Keys.D5))
                ui.SelectInterface(new Point(55 + 4 * offset, 55));

            if (newKeyboardState.IsKeyDown(Keys.D6))
                ui.SelectInterface(new Point(55 + 5 * offset, 55));

            // Mouse interaction used for camera zoom
            if (newMouseState.ScrollWheelValue != mMouseState.ScrollWheelValue)
                ZoomCamera?.Invoke(this,new ScrollEventArgs(newMouseState.ScrollWheelValue));

            mKeyboardState = newKeyboardState;
            mMouseState = newMouseState;
        }
    }
}

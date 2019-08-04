using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Menu;

namespace TRY.States
{
    class GameStateManager
    {
        enum GameState
        {
            sMainMenu,
            sPauseMenu,
            sGameScreen,
            sLevelScreen
        };

        GameState state = GameState.sMainMenu;
        public virtual void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            switch (state)
            {
                case GameState.sMainMenu:
                    UpdateMainMenu(gameTime);
                    break;
                case GameState.sGameScreen:
                    UpdateGamescreen(gameTime);
                    break;
                case GameState.sLevelScreen:
                    UpdateLevelScreen(gameTime);
                    break;
                case GameState.sPauseMenu:
                    UpdatePauseMenu(gameTime);
                    break;
            }
        }
        void UpdateMainMenu(GameTime gameTime)
        {
            if (pushedStartGameButton)
                state = GameState.sMainMenu;
        }

        void UpdateGamescreen(GameTime gameTime)
        {
            if (StartGame)
                state = GameState.sGameScreen;
        }

        void UpdateLevelScreen(GameTime gameTime)
        {
            if (ChooseLevel)
                state = GameState.sLevelScreen;
        }

        void UpdatePauseMenu(GameTime gameTime)
        {
            if (ButtonPauseMenu)
                state = GameState.sPauseMenu;
        }
    }
}
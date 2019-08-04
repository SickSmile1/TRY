using Nez;

namespace Game1
{
    public class Game1 : Core
    {
        public Game1()
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            scene = new CreateBattleMap();
        }
    }
}


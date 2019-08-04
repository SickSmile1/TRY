namespace TRY.ConsoleInterpreter
{
    class ConsoleInterpreter
    {
        // string test;
        private BattleModeState mBattleMode;
        private ContentManager mContent;
        private Game1 mGame;

        public ConsoleInterpreter(Game1 game, ContentManager content, BattleModeState bms)
        {
            mGame = game;
            mContent = content;
            mBattleMode = bms;
            var interpreter = new ManualInterpreter();
            game.mConsole.Interpreter = interpreter;
            int x;
            int y;
            var test = "";
            interpreter.RegisterCommand("addenemys", args => test = GenerateEnemys(Convert.ToInt32(args[0]), Convert.ToInt32(args[1])));
        }

        private string GenerateEnemys(int x, int y)
        {
            return new Formation(mContent, mBattleMode, x, y);
        }


    }
}

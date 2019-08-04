using QuakeConsole;
using TRY.Kampfmodus;
using TRY.SaveGame;
using System;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;

namespace TRY
{
    static class ConsoleCommands
    {
        public static void RegisterConsoleCommands(ManualInterpreter interpreter, BattleModeState bms, Game1 game, AddEnemy addEnemy)
        {
            interpreter.RegisterCommand("addenemy", args => 
                addEnemy.AddRangedEnemy(new Point(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]))));
            interpreter.RegisterCommand("addform", args => Formation.Create(addEnemy, Convert.ToInt32(args[0]), Convert.ToInt32(args[1])));
            interpreter.RegisterCommand("addtech", args=> Formation.CreateTech(addEnemy));
            interpreter.RegisterCommand("exit", args => game.Console.ToggleOpenClose());
            interpreter.RegisterCommand("savegame", args => SaveStatetoFile.SaveStateFile(game, bms));
        }
    }
}


using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TRY.GameStates;
using TRY.Kampfmodus;

namespace TRY.SaveGame
{
    internal sealed class SaveStatetoFile
    {
        private static BattleModeState sBattleMode;
        private static LevelScreen sLevelScreen;

        /// <summary>
        /// Saves BattleMode or LevelScreen to a txt file in the Game Directory
        /// </summary>
        /// <param name="game"></param>
        /// <param name="battleMode"></param>
        /// <param name="levelScreen"></param>
        public static void SaveStateFile(Game1 game, BattleModeState battleMode = null, LevelScreen levelScreen = null)
        {
            var currentState = game.mState;

            sBattleMode = battleMode;
            sLevelScreen = levelScreen;
            switch (currentState.StateName)
            {
                case "BattleModeState":
                    SaveBattleFile(game);                    
                    break;
                case "LevelState":
                    SaveLevelScreen(game);
                    break;
                default:
                    SaveProgress(game);
                    break;
            }
        }

        private static void SaveLevelScreen(Game1 game)
        {
            SaveProgress(game);
            string s = Directory.GetCurrentDirectory();
            StreamWriter sw = new StreamWriter(@s + "\\level.txt");
            string output = JsonConvert.SerializeObject(sLevelScreen.mLevelAttributes);
            sw.WriteLine(output);
            sw.Close();
        }

        private static void SaveProgress(Game1 game)
        {
            string s = Directory.GetCurrentDirectory();
            StreamWriter sw = new StreamWriter(@s + "\\progress.txt");
            string output = JsonConvert.SerializeObject(game.PlayerProgress);
            sw.WriteLine(output);
            sw.Close();
            
            sw = new StreamWriter(@s + "\\achieved.txt");
            sw.WriteLine(game.Achieved.LevelPlayed);
            sw.WriteLine(game.Achieved.KilledEnemys);
            sw.WriteLine(game.Achieved.Revived);
            sw.WriteLine(game.Achieved.KilledBoss);
            sw.WriteLine(game.Achieved.StartedBattleMode);
            sw.WriteLine(game.Achieved.LostRoundOne);
            sw.WriteLine(game.Achieved.UpgradedWeapon);
            sw.WriteLine(game.Achieved.FirstLoose);
            sw.WriteLine(game.Achieved.KilledBossAlone);
            sw.WriteLine(game.Achieved.EnergyUsed);
            sw.WriteLine(game.Achieved.CharactersAlive);
            sw.WriteLine(game.Achieved.EndBossReach);
            sw.WriteLine(game.Achieved.Timer);
            sw.WriteLine(game.Achieved.PlayerCount);
            sw.WriteLine(game.Achieved.Oxygen);
            sw.Close();
        }

        private static void SaveBattleFile(Game1 game)
        {
            string s = Directory.GetCurrentDirectory();
            SaveProgress(game);
            StreamWriter sw = new StreamWriter(@s + "\\game.txt");


            sw.WriteLine("*CHARACTER*");
            foreach (var c in sBattleMode.GetPlayerCharacters())
            {
                var x = c.Position.X.ToString(CultureInfo.InvariantCulture).Split('.');
                var y = c.Position.Y.ToString(CultureInfo.InvariantCulture).Split('.');
                sw.WriteLine("{0},{1},{2},{3},{4}", x[0], y[0], c.Health, c.Id, c.PlayerLevel);
            }

            sw.WriteLine("*ENEMY*");
            var enemyXy = new Dictionary<Vector2, int>();
            foreach (var c in sBattleMode.GetEnemyCharacters())
            {
                var x = (int)c.MidPoint.X;
                var y = (int)c.MidPoint.Y;
                if (!enemyXy.ContainsKey(c.Position))
                {
                    sw.WriteLine("{0},{1},{2},{3},{4}", x, y, c.Health, c.Texture, c.Id);
                    enemyXy.Add(c.Position, 0);
                }
            }

            sw.WriteLine("*DOORS*");
            foreach (var doors in sBattleMode.Doors)
            {
                sw.WriteLine(doors.Closed);
            }

            sw.WriteLine("*CRYO*");
            foreach (var cryo in sBattleMode.CryoChambers)
            {
                sw.WriteLine(cryo.mPosition.X);
                sw.WriteLine(cryo.mPosition.Y);
            }

            sw.WriteLine("*OxEn*");
            sw.WriteLine(sBattleMode.Oxygen.ToString());
            sw.WriteLine(sBattleMode.Energy.ToString());
            sw.Close();
        }
    }
}

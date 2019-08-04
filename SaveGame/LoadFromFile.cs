using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TRY.GameStates.Structure;

namespace TRY.SaveGame
{
    class LoadFromFile
    {
        private List<string> mCharList;
        private List<string> mEnemyList;
        private List<string> mDoors;
        private List<string> mOxEn;
        private List<Attributes> mLevel;
        private List<string> mCryo;

        internal static void LoadAchieved(Game1 game)
        {
            string s = Directory.GetCurrentDirectory();
            StreamReader sr = new StreamReader(@s + "\\progress.txt");
            game.Achieved.LevelPlayed = Convert.ToInt32(sr.ReadLine());
            game.Achieved.KilledEnemys = Convert.ToInt32(sr.ReadLine());
            game.Achieved.Revived = Convert.ToInt32(sr.ReadLine());
            game.Achieved.KilledBoss = Convert.ToBoolean(sr.ReadLine());
            game.Achieved.StartedBattleMode = Convert.ToBoolean(sr.ReadLine());
            game.Achieved.LostRoundOne = Convert.ToBoolean(sr.ReadLine());
            game.Achieved.UpgradedWeapon = Convert.ToBoolean(sr.ReadLine());
            game.Achieved.FirstLoose = Convert.ToBoolean(sr.ReadLine());
            game.Achieved.KilledBossAlone = Convert.ToBoolean(sr.ReadLine());
            game.Achieved.EnergyUsed = Convert.ToInt32(sr.ReadLine());
            game.Achieved.CharactersAlive = Convert.ToInt32(sr.ReadLine());
            game.Achieved.EndBossReach = Convert.ToBoolean(sr.ReadLine());
            game.Achieved.Timer = Convert.ToDouble(sr.ReadLine());
            game.Achieved.PlayerCount =  Convert.ToInt32(sr.ReadLine());
            game.Achieved.Oxygen = Convert.ToInt32(sr.ReadLine());
        }

        public LoadFromFile(string s)
        {
            if (s == "battle") LoadBattleMode();
            if (s == "level") LoadLevel();
        }

        private void LoadLevel()
        {
            string s = Directory.GetCurrentDirectory();
            mLevel = JsonConvert.DeserializeObject<List<Attributes>>(File.ReadAllText(s+"\\level.txt"));
        }

        public List<Attributes> ReturnProps()
        {
            return mLevel;
        }

        private void LoadBattleMode()
        {
            string s = Directory.GetCurrentDirectory();
            mCharList = new List<string>();
            mEnemyList = new List<string>();
            mDoors = new List<string>();
            mCryo = new List<string>();
            mOxEn = new List<string>();
            var sr = new StreamReader(s+"\\game.txt");
            string line;

            while ((line = sr.ReadLine()) != null && line != "*ENEMY*")
            {
                if (line == "*CHARACTER*") line = sr.ReadLine();
                if (line != null)
                {
                    var character = line.Split(',');
                    mCharList.Add(character[0]);
                    mCharList.Add(character[1]);
                    mCharList.Add(character[2]);
                    mCharList.Add(character[3]);
                    mCharList.Add(character[4]);
                }
            }

            while ((line = sr.ReadLine()) != null && line != "*DOORS*")
            {
                if (line == "*ENEMY*") line = sr.ReadLine();

                if (line != null)
                {
                    var character = line.Split(',');
                    mEnemyList.Add(character[0]);
                    mEnemyList.Add(character[1]);
                    mEnemyList.Add(character[2]);
                    mEnemyList.Add(character[3]);
                    mEnemyList.Add(character[4]);
                }
            }

            while ((line = sr.ReadLine()) != null && line != "*CRYO*")
            {
                if (line == "*DOORS*") line = sr.ReadLine();
                mDoors.Add(line);
            }

            while ((line = sr.ReadLine()) != null && line != "*OxEn*")
            {
                if (line == "*Cryo*") line = sr.ReadLine();
                mCryo.Add(line);
            }

            while ((line = sr.ReadLine()) != null)
            {
                if (line == "*OxEn*") line = sr.ReadLine();
                mOxEn.Add(line);
            }
            sr.Close();
        }
        public List<string> ReturnChar()
        {
            return mCharList;
        }

        public List<string> ReturnCryo() 
        {
            return mCryo; 
        }

        public List<string> ReturnOxEn()
        {
            return mOxEn;
        }

        public List<string> ReturnEnemy()
        {
            return mEnemyList;
        }

        public List<string> ReturnDoors()
        {
            return mDoors;
        }

    }
}

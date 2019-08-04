using System;
using System.Collections.Generic;

namespace TRY.GameStates.Structure
{
    public class Achieved
    {
        internal int LevelPlayed { get; set; }
        internal int KilledEnemys { get; set; }
        internal int Revived { get; set; }
        internal bool KilledBoss { get; set; }
        internal bool StartedBattleMode { get; set; }
        internal bool LostRoundOne { get; set; }
        internal bool UpgradedWeapon { get; set; }
        internal bool FirstLoose { get; set; }
        internal bool KilledBossAlone { get; set; }
        internal int EnergyUsed { get; set; }
        internal int CharactersAlive { get; set; }
        internal bool EndBossReach { get; set; }
        internal double Timer { get; set; }
        internal int PlayerCount { get; set; }
        internal int Oxygen { get; set; }

        public Achieved(int killed = 0,
            int revived = 0,
            bool killedBoss = false,
            bool started = false,
            bool lost = false,
            bool upgrade = false,
            bool first = false,
            int used = 0,
            bool alone = false, 
            int foundPlayer = 0,
            int foundOxygen = 0,
            int levelPlayed = 0)
        {
            Timer = 0;
            KilledEnemys = killed;
            Revived = revived;
            KilledBoss = killedBoss;
            StartedBattleMode = started;
            LostRoundOne = lost;
            UpgradedWeapon = upgrade;
            FirstLoose = first;
            EnergyUsed = used;
            KilledBossAlone = alone;
            Oxygen = foundOxygen;
            PlayerCount = foundPlayer;
            LevelPlayed = levelPlayed;
        }

        public List<int> ReturnAchieved()
        {
            var d = new List<int>();
            d.Add(KilledEnemys >= 1 ? 1 : 0);
            d.Add(KilledBoss ? 1 : 0);
            d.Add(LostRoundOne ? 1 : 0);
            d.Add(KilledEnemys > 49 ? 1 : 0);
            d.Add(UpgradedWeapon ? 1 : 0);
            d.Add(StartedBattleMode ? 1 : 0);
            d.Add(Revived > 3 ? 1 : 0);
            d.Add(FirstLoose ? 1 : 0);
            d.Add(EnergyUsed > 49 ? 1 : 0);
            d.Add(KilledBossAlone ? 1 : 0);           
            return d;
        }

        public int LevelsPlayed()
        {
            var s =   (double)LevelPlayed / 24;
            var d = (s*100);
            return (int)d;
        }


        public double CalculateHighscore(Game1 game)
        {
            var b = 0.5;
            if (EndBossReach) b = (b + 0.5);
            if (KilledBoss) b = b + 2;
            var o = game.PlayerProgress.PlayerOxygen;
            var e = game.PlayerProgress.PlayerEnergy;
            var d = (KilledEnemys*(1+CharactersAlive)*(e+o)*b);
            d /= Timer;
            if (d < 1) d = 1.0d;
            return Math.Round(d, 2);
        }
    }
}

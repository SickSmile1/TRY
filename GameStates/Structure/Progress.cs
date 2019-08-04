using System.Collections.Generic;

namespace TRY.GameStates.Structure
{
    public class Progress
    {
        public int PlayerOxygen { get; set; }
        public int PlayerEnergy { get; set; }

        public int CurrentLevel
        {
            get; 
            set;
        }
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public Dictionary<string, List<int>> PlayerLevel { get; set; }

        /// <summary>
        /// Reads Oxygen/Energy in BattleStateMode, adepts Character Level in LevelScreen, read from file in Game1
        /// Saved with every Save of BattleStateMode or LevelScreen
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="oxygen"></param>
        /// <param name="energy"></param>
        /// <param name="level"></param>
        public Progress(Dictionary<string, List<int>> dict = null, int oxygen = 6, int energy = 2, int level = 0)
        {
            PlayerOxygen = oxygen;
            PlayerEnergy = energy;
            CurrentLevel = level;
            PlayerLevel = dict ?? CreateDict();
        }

        private Dictionary<string, List<int>> CreateDict()
        {
            var d = new Dictionary<string, List<int>>(){
                { "Vut", new List<int>{0, 0} },
                { "Maximus", new List<int>{0, 0} },
                { "Wiense", new List<int>{0, 0} },
                { "Domogas", new List<int>{0, 0} },
                { "Burkha", new List<int>{0, 0} },
                { "Ngol", new List<int>{0, 0} }
            };
            return d;
        }
    }
}

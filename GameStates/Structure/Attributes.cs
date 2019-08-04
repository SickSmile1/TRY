namespace TRY.GameStates.Structure
{
    class Attributes
    {
        public int Oxygen { get; set; }
        public int Energy { get; set; }
        public int Chamber { get; set; }
        public bool Visited { get; set; }
        public bool Player { get; set; }

        public Attributes(int oxygen = 0, int energy = 0, int chamber = 0, bool visited = false, bool containsPlayer = false)
        {
            Oxygen = oxygen;
            Energy = energy;
            Chamber = chamber;
            Visited = visited;
            Player = containsPlayer;
        }
    }
}

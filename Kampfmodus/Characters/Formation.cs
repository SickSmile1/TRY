using Microsoft.Xna.Framework;

namespace TRY.Kampfmodus.Characters
{
    internal static class Formation
    {
        public static void Create(AddEnemy addEnemy, int x, int y)
        {
            addEnemy.AddExplosive(new Point(x - 20, y - 20));
            addEnemy.AddSupervisor(new Point(x + 20, y));
            addEnemy.AddRangedEnemy(new Point(x - 20, y + 20));
            addEnemy.AddRangedEnemy(new Point(x, y));
            addEnemy.AddMeleeEnemy(new Point(x - 20, y));
            addEnemy.AddMeleeEnemy(new Point(x + 20, y + 20));
            addEnemy.AddSupervisor(new Point(x + 20, y - 20));
        }

        public static void CreateTech(AddEnemy addEnemy)
        {
            var row = new[] { 2050, 2150, 2250, 2350, 2450, 2550, 2650, 2750, 2850, 2950 };
            var column = new[] { 200, 350, 500, 650, 800, 950, 1100, 1250, 1400, 1550, 1600 };
            for (int i = 0; i < row.Length; i++)
            {
                for (int k = 0; k < column.Length; k++)
                {
                    int x = row[i];
                    int y = column[k];
                    addEnemy.AddExplosive(new Point(x - 20, y - 20));
                    addEnemy.AddSupervisor(new Point(x + 20, y));
                    addEnemy.AddRangedEnemy(new Point(x - 20, y + 20));
                    addEnemy.AddExplosive(new Point(x, y - 20));
                    addEnemy.AddExplosive(new Point(x, y + 20));
                    addEnemy.AddRangedEnemy(new Point(x, y));
                    addEnemy.AddMeleeEnemy(new Point(x - 20, y));
                    addEnemy.AddMeleeEnemy(new Point(x + 20, y + 20));
                    addEnemy.AddSupervisor(new Point(x + 20, y - 20));
                }
            }
        }
    }
}
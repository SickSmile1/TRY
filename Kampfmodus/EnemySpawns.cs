using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TRY.Kampfmodus
{
    class EnemySpawns
    {
        private readonly AddEnemy mAddEnemy;
        private readonly HashSet<Vector2> mSpawnPositions;
        private readonly Random mRandom;
        private float mElapsedGameTime;
        private readonly float mFrequency;

        public int EnemiesSpawned { get; private set; }
        
        public EnemySpawns(HashSet<Vector2> positions, AddEnemy addEnemy)
        {
            mFrequency = 15f;
            mAddEnemy = addEnemy;
            mElapsedGameTime = 0;
            mRandom = new Random();
            mSpawnPositions = positions;
            EnemiesSpawned = 0;
        }

        private void SpawnEnemy(Point spawnPoint, int damage)
        {
            var whichEnemy = mRandom.Next(6);
            switch (whichEnemy)
            {
                case 0:
                    mAddEnemy.AddExplosive(spawnPoint, damage);
                    break;
                case 1:
                    mAddEnemy.AddMeleeEnemy(spawnPoint, damage);
                    break;
                case 2:
                    mAddEnemy.AddRangedEnemy(spawnPoint, damage);
                    break;
                case 3:
                    mAddEnemy.AddSupervisor(spawnPoint, damage);
                    break;
                case 4:
                    mAddEnemy.AddMeleeEnemy(spawnPoint, damage);
                    break;
                case 5:
                    mAddEnemy.AddRangedEnemy(spawnPoint, damage);
                    break;
            }
        }

        public void Update(GameTime gameTime, float timer, int level, BattleModeState bms, int currentlevel)
        {
            mElapsedGameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f;
            EnemiesSpawned = 0;
            if (mElapsedGameTime < mFrequency) return;
            mElapsedGameTime = 0;
            var spawnRate = (int) (5 + (timer / mFrequency) * 2);
            // Spawn rate cap at 20.
            spawnRate = spawnRate > 15 ? 15 : spawnRate;
            
            // Go through every possible spawn points and spawn random enemies.        
            foreach (var spawnPosition in mSpawnPositions)
            {
                var spawnPoint = new Point((int)spawnPosition.X, (int)spawnPosition.Y);
                if (EnemiesSpawned >= spawnRate) break;                               
                // Decide how many enemies to spawn.
                var spawnAtNode = mRandom.Next(4);
                var i = 0;
                while (i < spawnAtNode)
                {
                    if (EnemiesSpawned >= spawnRate) break;
                    SpawnEnemy(spawnPoint, level);
                    i++;
                }
            }
            if(currentlevel != 24) { bms.ReinforcementKi(); }
            EnemiesSpawned = spawnRate;
        }
    }
}

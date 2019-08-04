using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Abilities
{
    internal class Emp: IAbility
    {
        public Emp(BattleModeState.CharactersInRadius findCharactersInRadius, BattleModeState.FreezeCharacters freeze,
            float duration = 5, int radius = 300, float coolDown = 10)
        {
            Id = "Emp";
            mFindCharactersInRadius = findCharactersInRadius;
            mFreeze = freeze;
            Damage = 0;
            Radius = radius;
            Duration = duration;
            CoolDown = coolDown;
            SecondsPassed = coolDown;
        }

        private readonly BattleModeState.CharactersInRadius mFindCharactersInRadius;
        private readonly BattleModeState.FreezeCharacters mFreeze;
        public string Id { get; }
        public int Damage { get; set; }
        public float Duration { get; set; }
        public bool Active { get; set; }
        public float SecondsPassed { get; private set; }
        public float CoolDown { get; set; }
        public float Radius { get; set; }
        private List<ICharacter> mFrozenEnemies;

        public void UseAbility(Vector2 position)
        {
            if ((SecondsPassed < CoolDown)) return;
            mFrozenEnemies = mFindCharactersInRadius(position, Radius, false);
            if (mFrozenEnemies == null || mFrozenEnemies.Count == 0) return;
            if (mFrozenEnemies.Count == 1 && mFrozenEnemies[0].Texture == "Boss") return;

            mFreeze(mFrozenEnemies, false);
            SecondsPassed = 0;
        }

        public void Update(GameTime gameTime)
        {
            SecondsPassed += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (!(SecondsPassed > Duration)) return;
            mFreeze(mFrozenEnemies, true);
        }

        public void Draw(SpriteBatch sb, TextureManager tex)
        {
        }

        public void Terminate()
        {
            mFreeze(mFrozenEnemies, true);
        }
    }
}

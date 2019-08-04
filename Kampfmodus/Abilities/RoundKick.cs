using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Abilities
{
    internal class RoundKick: IAbility
    {
        public string Id { get; }
        public int Damage { get; set; }
        public float Duration { get; set; }
        public float CoolDown { get; set; }
        public float Radius { get; set; }
        public bool Active { get; set; }
        private static Animation sRoundKickAnimation;
        private readonly BattleModeState.CharactersInRadius mFindCharactersInRadius;
        public float SecondsPassed { get; private set; }
        private static Vector2 sPosition;
        private static Point sSize;

        public RoundKick(BattleModeState.CharactersInRadius findCharactersInRadius,
            int radius = 300, float coolDown = 2, int damage = 30)
        {
            Id = "RoundKick";
            mFindCharactersInRadius = findCharactersInRadius;
            Radius = radius;
            CoolDown = coolDown;
            SecondsPassed = coolDown;
            Damage = damage;
            Active = false;
            Duration = 2;
            sSize = new Point(20, 43);
            var animation = new[] { 4 };
            sRoundKickAnimation = new Animation("RoundKick", new Vector2(sSize.X , sSize.Y ), new List<int>(animation));
        }
        public void UseAbility(Vector2 position)
        {
            if ((SecondsPassed < CoolDown)) return;
            Active = true;
            var nearEnemies = mFindCharactersInRadius(position, Radius, false);
            foreach (var enemy in nearEnemies)
            {
                enemy.Health -= Damage;
                // kick back
                enemy.Position += Vector2.Normalize(position - enemy.Position) * 10;
            }
            sPosition = new Vector2(position.X-2, position.Y-5);
            SecondsPassed = 0;
            sRoundKickAnimation.ResetAnimation();
        }

        public void Update(GameTime gameTime)
        {
            SecondsPassed += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (!Active) return;
            sRoundKickAnimation.UpdateAnimation(gameTime, 30f / 3, 0, false);
            if (sRoundKickAnimation.mCurrentFrame != 3) return;
            Active = false;
            Duration = SecondsPassed;

        }

        public void Terminate()
        {
        }

        public void Draw(SpriteBatch sb, TextureManager textureManager)
        {
            sRoundKickAnimation.Draw(sb, new Vector2(sPosition.X - ((float)sSize.X / 2), sPosition.Y - ((float)sSize.X / 2)), textureManager);
        }
    }
}
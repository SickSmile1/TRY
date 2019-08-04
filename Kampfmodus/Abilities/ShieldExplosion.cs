using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;
using TRY.Kampfmodus.Weapons;

namespace TRY.Kampfmodus.Abilities
{
    internal class ShieldExplosion: IAbility
    {
        public ShieldExplosion(BattleModeState.CharactersInRadius findCharactersInRadius, int coolDown = 5)
        {
            Id = "ShieldExplosion";
            mFindCharactersInRadius = findCharactersInRadius;
            Active = false;
            Damage = 40;
            SecondsPassed = coolDown;
            Duration = coolDown;
            Radius = 300;

            var animation = new[] { 9 };
            sShieldExpAnimation = new Animation("Explosion", sSize, new List<int>(animation));
        }
        private readonly BattleModeState.CharactersInRadius mFindCharactersInRadius;
        private static Animation sShieldExpAnimation;
        private static readonly Vector2 sSize = new Vector2(150, 150);
        public string Id { get; }
        public int Damage { get; set; }
        public float Duration { get; set; }
        public float CoolDown { get; set; }
        public float Radius { get; set; }
        public bool Active { get; set; }
        public float SecondsPassed { get; private set; }
        private static Vector2 sPosition;

        public void UseAbility(Vector2 position)
        {
            if (!Shield.mShieldActive) return;
            sPosition = position;
            var enemies = mFindCharactersInRadius(position, Radius, false);
            if (enemies == null) return;
            foreach (var enemy in enemies)
            {
                enemy.Health -= Damage;
            }
            sShieldExpAnimation.ResetAnimation();
            Game1.sSoundEffectInstance[2].Play();
            Shield.mShieldActive = false;
            Shield.mCurrentShieldDamage = Shield.MaxShieldDamage2;
            Active = true;
            SecondsPassed = 0;
        }

        public void Draw(SpriteBatch sb, TextureManager tex)
        {
            sShieldExpAnimation.Draw(sb, new Vector2(sPosition.X - (sSize.X / 2),
                y: sPosition.Y - (sSize.Y / 2)), tex);
        }


        public void Update(GameTime gameTime)
        {
            SecondsPassed += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (!Active) return;
            sShieldExpAnimation.UpdateAnimation(gameTime, 22 / 5f, 0, false);
        }

        public void Terminate()
        {
        }
    }
}
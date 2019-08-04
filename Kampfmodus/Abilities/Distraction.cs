using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Abilities
{
    internal class Distraction : IAbility
    {
        public Distraction(string textureDescriptor, BattleModeState.PlaceRabbit placeRabbit, float duration = 5, float cooldown = 8)
        {
            Id = "Distraction";
            CoolDown = cooldown;
            Duration = duration;
            mTextureDescriptor = textureDescriptor;
            mPlaceRabbit = placeRabbit;
            SecondsPassed = 0;
            Size = new Point(20, 40);
            SecondsPassed = cooldown;

            var list = new[] { 1, 19 };
            HudAnimation = new Animation("Sanitar1Abi", new Vector2(60, 90), new List<int>(list));
        }

        private readonly BattleModeState.PlaceRabbit mPlaceRabbit;
        public float CoolDown { get; set; }
        public float SecondsPassed { get; private set; }
        public float Duration { get; set; }
        private Point Size { get; }
        public string Id { get; }
        public int Damage{get => 0;set { }}
        public bool Active { get; set; }
        private Animation HudAnimation { get; }
        public float Radius { get => 0; set { } }
        private int mAnim;

        private readonly string mTextureDescriptor;

        public void Update(GameTime gameTime)
        {
            HudAnimation.UpdateAnimation(gameTime, 19 / Duration, mAnim, false);

            SecondsPassed += gameTime.ElapsedGameTime.Milliseconds/1000.0f;
        }

        public void Draw(SpriteBatch sb, TextureManager tex)
        {
        }

        public void Terminate()
        {
        }

        public void UseAbility(Vector2 position)
        {
            mAnim = 0;
            HudAnimation.ResetAnimation();
            if (!(SecondsPassed >= CoolDown)) return;
            mPlaceRabbit(new DistractionObject(mTextureDescriptor, new Vector2(position.X-10, 
                y: position.Y-20), Size, Duration));
            SecondsPassed = 0;
            mAnim = 1;
        }
    }
}

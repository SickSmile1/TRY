using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus.Abilities
{
    class ReviveCharacter : IAbility
    {
        private ICharacter mDeadCharacter;
        private float ReviveTime { get; set; }
        public string Id { get; }
        public int Damage { get; set; }
        public float Duration { get; set; }
        public bool Active { get; set; }
        public float SecondsPassed { get; private set; }
        public float CoolDown { get; set; }
        public float Radius { get; set; }

        private readonly BattleModeState.FindCharacter mFindDeadCharacter;

        public ReviveCharacter(BattleModeState.FindCharacter findDeadCharacter,
                               int radius = 100, int cooldown = 10, int duration = 5, int damage=70)
        {
            Id = "ReviveCharacter";
            mFindDeadCharacter = findDeadCharacter;
            Damage = damage;
            Radius = radius;
            Duration = duration;
            CoolDown = cooldown;
            SecondsPassed = cooldown;
            mDeadCharacter = null;
            ReviveTime = 0;
        }

        public void UseAbility(Vector2 position)
        {
            if (SecondsPassed < CoolDown) return;  // When ability is on cooldown, return.
            if (mDeadCharacter == null)
            {
                var character = mFindDeadCharacter(position, true);
                if (character == null || (character.MidPoint - position).Length() > Radius) return;
                mDeadCharacter = character;
                mDeadCharacter.IsBeingRevived = true;
            }
            else
            {
                if ((mDeadCharacter.MidPoint - position).Length() > Radius)
                {
                    SecondsPassed = 0;
                    mDeadCharacter.IsBeingRevived = false;
                    mDeadCharacter = null;
                    ReviveTime = 0;
                }
                else if (ReviveTime >= Duration)
                {
                    SecondsPassed = 0;
                    ReviveTime = 0;
                    mDeadCharacter.IsBeingRevived = false;
                    mDeadCharacter.Health = Damage;
                    mDeadCharacter = null;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            SecondsPassed += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (mDeadCharacter != null) ReviveTime += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
        }

        public void Draw(SpriteBatch sb, TextureManager tex)
        {
        }

        public void Terminate()
        {
        }
    }
}

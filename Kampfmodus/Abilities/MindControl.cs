using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;


namespace TRY.Kampfmodus.Abilities
{
    internal class MindControl : IAbility
    {
        public MindControl(BattleModeState.FindCharacter findNearestCharacter,
            BattleModeState.TogglePlayer togglePlayer, 
            float durationInSeconds = 5, float range = 700)
        {
            Id = "MindControl";
            mFindNearestCharacter = findNearestCharacter;
            mTogglePlayer = togglePlayer;
            Duration = durationInSeconds;
            mControlDuration = 0;
            Radius = range;
            CoolDown = 5;
            SecondsPassed = CoolDown;

        }

        private readonly BattleModeState.FindCharacter mFindNearestCharacter;
        private readonly BattleModeState.TogglePlayer mTogglePlayer;
        public string Id { get; }
        public int Damage { get; set; }
        public float CoolDown { get; set; }
        public float SecondsPassed { get; private set; }
        public float Duration { get; set; }
        public float Radius { get; set; }
        public bool Active { get; set; }

        private ICharacter mEnemy;
        private float mControlDuration;

        public void Update(GameTime gameTime)
        {
            SecondsPassed += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (mEnemy == null) return;
            if (mEnemy.Health <= 0)
            {
                mTogglePlayer(mEnemy);
                mEnemy = null;
            }

            mControlDuration += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (!(mControlDuration > Duration)) return;
            mTogglePlayer(mEnemy);
            mEnemy = null;
        }

        public void Draw(SpriteBatch sb, TextureManager tex)
        {
        }

        public void Terminate()
        {
            if (mEnemy == null) return;
            mTogglePlayer(mEnemy);
            mEnemy = null;
        }

        public void UseAbility(Vector2 position)
        {
            if (!(SecondsPassed >= CoolDown)) return;
            var nearest = mFindNearestCharacter.Invoke(position, false);
            if (nearest == null || !(Vector2.Distance(nearest.MidPoint, position) <= Radius)) return;
            if (mEnemy != null)
            {
                mTogglePlayer(mEnemy);
            }

            else if (nearest.Texture != "Boss")
            {
                mTogglePlayer(nearest);
                mEnemy = nearest;
                SecondsPassed = 0;
                mControlDuration = 0;
            }
        }
    }
}
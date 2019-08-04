using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus.Abilities
{
    internal class Mines : IAbility
    {
        public Mines(string textureDescriptor, BattleModeState.PlaceMine mine, BattleModeState.CharactersInRadius charactersInRadius, bool player, int damage = 50, float cooldown = 8, float radius = 200)
        {
            Id = "Mines";
            CoolDown = cooldown;
            Damage = damage;
            Radius = radius;
            mTextureDescriptor = textureDescriptor;
            mPlaceMine = mine;
            SecondsPassed = 0;
            Size = new Point(20, 20);
            SecondsPassed = cooldown;
            mCharactersInRadius = charactersInRadius;
            mPlayer = player;
        }

        private readonly BattleModeState.PlaceMine mPlaceMine;
        private readonly BattleModeState.CharactersInRadius mCharactersInRadius;
        private readonly bool mPlayer;
        public float CoolDown { get; set; }
        public float SecondsPassed { get; private set; }
        public float Duration { get => 0;set { }}
        public bool Active { get; set; }
        private Point Size { get; }
        public string Id { get; }
        public int Damage { get; set; }
        public float Radius { get; set; }

        private readonly string mTextureDescriptor;
        public void Update(GameTime gameTime)
        {
            SecondsPassed += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
        }

        public void Draw(SpriteBatch sb, TextureManager tex)
        {

        }

        public void Terminate()
        {
        }

        public void UseAbility(Vector2 position)
        {
            if ((SecondsPassed < CoolDown)) return;
            mPlaceMine(new MineObjects(mTextureDescriptor, mCharactersInRadius, mPlayer, position, Size, Damage, Radius));
            SecondsPassed = 0;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Characters;

namespace TRY.Kampfmodus
{
    sealed class HealthBar
    {
        private readonly Texture2D mHealthBar;
        private readonly float mHealthBarWidth;
        private readonly float mHealthBarHeight;

        public HealthBar(Texture2D healthBar)
        {
            mHealthBar = healthBar;
            mHealthBarWidth = mHealthBar.Width;
            mHealthBarHeight = mHealthBar.Height / 3f;
        }

        public void DrawCharacterHp(SpriteBatch sb, Vector2 offset, int health, int maxHealth)
        {
            float healthFactor = (float) health / maxHealth;
            // Each time character's has been reduced, reduce the width of the hp bar;
            var scale = 50f / mHealthBarWidth * 2;
            float healthBarWidth = mHealthBarWidth * scale * healthFactor; 
            sb.Draw(mHealthBar,
                new Rectangle((int) offset.X, (int) offset.Y * 3 + 10, (int)healthBarWidth, (int)mHealthBarHeight / 3),
                Color.White);
        }

        public void DrawEnemyHp(SpriteBatch sb, ICharacter npc)
        {
            var healthFactor = npc.Health / (float) npc.MaxHealth;
            var healthBarWidth = mHealthBarWidth / 3f * healthFactor;
            sb.Draw(mHealthBar, new Rectangle((int)npc.Position.X, (int)npc.Position.Y - 20,
                (int)healthBarWidth, (int)mHealthBarHeight / 4), Color.White);
        }
        public void DrawDoorHp(SpriteBatch sb, Door npc)
        {
            var healthFactor = (float)npc.Health / 100;
            var healthBarWidth = mHealthBarWidth / 3f * healthFactor;
            sb.Draw(mHealthBar, new Rectangle(npc.DoorArea.X, npc.DoorArea.Y - 20,
                (int)healthBarWidth, (int)mHealthBarHeight / 4), Color.White);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Collision;

namespace TRY.Kampfmodus
{
    class Door: IStaticCollider
    {
        public bool Closed { get; private set; }
        public Rectangle DoorArea { get; }
        private Texture2D Texture { get; }
        private float SecondsOpen { get; set; }
        private float SecondsSinceDamage { get; set; }

        private int mHealth;
        public int Health
        {
            get => mHealth;
            set
            {
                if (SecondsSinceDamage > 5)
                {
                    mHealth = value;
                    SecondsSinceDamage = 0;
                }
            }
        }

        public Door(Texture2D texture, Vector2 position, bool closed)
        {
            Texture = texture;
            Closed = closed;
            DoorArea = new Rectangle(position.ToPoint(), new Point(32, texture.Height));
            ObjectArea = DoorArea;
            if (Closed)
            {
                Close();
            }
            else
            {
                Open();
            }

            mHealth = 100;
            Health = 100;
            SecondsOpen = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Closed)
            {
                spriteBatch.Draw(Texture, DoorArea, Color.White);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Health > 0)
            {
                SecondsSinceDamage += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                if (!Closed)
                {
                    SecondsOpen += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                }

                if (SecondsOpen > 5.0f)
                {
                    Close();
                    SecondsOpen = 0;
                }
            }
            if (Health <= 0 && Closed)
            {
                Open();
            }
        }

        public void Open()
        {
            ObjectArea = new Rectangle(-1, -1, 0, 0);
            Closed = false;
        }

        private void Close()
        {
            if (Health > 0)
            {
                ObjectArea = DoorArea;
                Closed = true;
            }
        }

        public void Toggle()
        {
            if (Closed) Open();
            else if (!Closed) Close();
        }

        public Rectangle ObjectArea { get; private set; }
    }
}

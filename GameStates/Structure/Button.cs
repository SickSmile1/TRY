using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TRY.GameStates.Structure
{
    class Button
    {
        public Rectangle mButtonRectangle;
        private readonly Vector2 mTextPosition;
        public string mText;
        private Color mColor;
        public string Texture { get; set; }

        private MouseState mMouseState;
        public event EventHandler Click;

        public Button(int x, int y, string text = "", string texture = null)
        {
            mButtonRectangle = new Rectangle(x, y, 150, 40);
            mTextPosition = new Vector2(x + (10), y + (10));
            mText = text;
            mColor = Color.White;
            Texture = texture;
        }
        

        public Rectangle ReturnRectangle()
        {
            return mButtonRectangle;
        }

        public void SetColor(Button button)
        {
            button.mColor = button.mColor == Color.White ? Color.Gray : Color.White;
        }


        public void DrawLevels(SpriteBatch spriteBatch, Textures textures, Attributes attributes, bool level = false)
        {
            if (!attributes.Visited)
            {
                spriteBatch.Draw(textures.GetTexture("Button"), mButtonRectangle, mColor);
                if (attributes.Player)
                    spriteBatch.Draw(textures.GetTexture("Button"), mButtonRectangle, Color.DarkCyan);
                if (attributes.Oxygen >= 1)
                {
                    var location = new Point();
                    location.X = mButtonRectangle.X + 25;
                    location.Y = mButtonRectangle.Y + 5;
                    spriteBatch.Draw(textures.GetTexture("Oxygen"), new Rectangle(location, new Point(30, 30)), mColor);
                }

                if (attributes.Energy >= 1)
                {
                    var location = new Point();
                    location.X = mButtonRectangle.X + 5;
                    location.Y = mButtonRectangle.Y + 45;
                    spriteBatch.Draw(textures.GetTexture("Energy"), new Rectangle(location, new Point(30, 30)), mColor);
                }

                if (attributes.Chamber >= 1)
                {
                    var location = new Point();
                    location.X = mButtonRectangle.X + 45;
                    location.Y = mButtonRectangle.Y + 5;
                    spriteBatch.Draw(textures.GetTexture("Char"), new Rectangle(location, new Point(30, 30)), mColor);
                }
            }
            else
            {
                spriteBatch.Draw(textures.GetTexture("Button"), mButtonRectangle, Color.DarkOliveGreen);
            }
            if (attributes.Player )
            {
                Point location = new Point();
                location.X = mButtonRectangle.X+25;
                location.Y = mButtonRectangle.Y+5;
                spriteBatch.Draw(textures.GetTexture("Alien"),new Rectangle(location,new Point(50, 80)), mColor);
            }

        }

        public void Draw(SpriteBatch spriteBatch, Textures textures, bool lvlup = false, bool icon = false, bool hud = false)
        {
            if (lvlup)
            {
                spriteBatch.Draw(textures.GetTexture("Plus"), mButtonRectangle, Color.White);
                return;
            }

            if (icon)
            {
                spriteBatch.Draw(textures.GetTexture(/*"Alien"*/Texture+"Hud"), mButtonRectangle, mColor);
                return;
            }

            if (hud)
            {
                spriteBatch.Draw(textures.GetTexture(Texture), mButtonRectangle, Color.White );
                return;
            }
        
            spriteBatch.Draw(textures.GetTexture("Button"), mButtonRectangle, mColor);
            spriteBatch.DrawString(textures.GetFont("Font"), mText, mTextPosition, Color.Black);
        
        }

        public void Update()
        {
            
            if (mButtonRectangle.Contains(mMouseState.Position))
            {
                var newMouse = Mouse.GetState();
                mColor = Color.Gray;
                if (mMouseState.LeftButton == ButtonState.Released &&
                    newMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
                mColor = Color.Gray;
            }
            else
            {
                mColor = Color.White;
            }
            mMouseState = Mouse.GetState();
        }
    }
}
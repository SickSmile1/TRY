using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TRY.Menu
{
    class Button
    {
        // Fonts for Button, Background and Text
        private readonly SpriteFont mFont;
        private readonly Texture2D mBackground;
        private readonly Texture2D mButton;
        private readonly Rectangle mButtonRectangle;
        // Text position and variabe
        private readonly Vector2 mTextPosition;
        private readonly string mText;
        private Color mColor;

        public Button(ContentManager content, int x, int y, string text)
        {
            mFont = content.Load<SpriteFont>("Menu/File");
            mBackground = content.Load<Texture2D>("Menu/MenuBackground");
            mButton = content.Load<Texture2D>("Menu/Button");
            mButtonRectangle = new Rectangle(x, y, 150, 40);
            mTextPosition = new Vector2(x+10, y+10);
            mText = text;
            mColor = Color.White;
        }

        public Rectangle returnRectangle()
        {
            return mButtonRectangle;
        }

        public void SetColor(Button button)
        {
            if (button.mColor == Color.White) button.mColor = Color.Gray;
            else button.mColor = Color.White;
        } 


        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //if (Fontdrawn)
            //{
            //    spriteBatch.Draw(mBackground, destinationRectangle: new Rectangle(0, 0, 1920, 1080));
            //    Fontdrawn = false;
            //}
            spriteBatch.Draw(mButton, mButtonRectangle, mColor);
            spriteBatch.DrawString(mFont, mText, mTextPosition, Color.Black);
        }
    }
}

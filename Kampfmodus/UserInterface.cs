using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus
{
    class UserInterface
    {
        private readonly BattleModeState mBms;
        private readonly Selection mSelection;
        private readonly float mPortraitBorderX;
        private readonly float mPortraitBorderY;
        private readonly int mPortraitWidth;
        private readonly HealthBar mHealthBar;
        private readonly SpriteFont mFont;

        public UserInterface(BattleModeState bms, Selection select, Texture2D healthBar, SpriteFont font)
        {
            mBms = bms;
            mSelection = select;
            mHealthBar = new HealthBar(healthBar);
            mPortraitBorderX = 50f;
            mPortraitBorderY = 50f;
            mPortraitWidth = 100;
            mFont = font;
        }

        public bool SelectInterface(Point clickPoint)
        {
            var nChar = 0;
            foreach (var ch in mBms.GetPlayerCharacters())
            {
                var rect = new Rectangle((int)mPortraitBorderX + (int)(mPortraitWidth + mPortraitBorderX) * nChar,
                                         (int)mPortraitBorderY, mPortraitWidth, mPortraitWidth);
                if (rect.Contains(clickPoint))
                {
                    mSelection.UnselectAll();
                    mSelection.SelectCharacter(ch);
                    Game1.sSoundEffectInstance[9].Play();
                    return true;
                }

                nChar++;
            }
            return false;
        }


        public void Draw(GraphicsDevice graphics, SpriteBatch sb, TextureManager textureManager)
        {
            sb.DrawString(mFont, "Oxygen: " + mBms.Oxygen,
                          new Vector2(graphics.Viewport.Width - 100, 20), Color.AliceBlue);
            sb.DrawString(mFont, "Energy: " + mBms.Energy,
                          new Vector2(graphics.Viewport.Width - 200, 20), Color.AliceBlue);
            var borderX = mPortraitBorderX;
            var borderY = mPortraitBorderY;
            var width = mPortraitWidth;
            var offset = new Vector2(borderX, borderY);
 
            foreach (var ch in mBms.GetPlayerCharacters())
            {
                

                sb.Draw(textureManager.GetTexture(ch.Texture+"Hud"), destinationRectangle: new Rectangle((int)offset.X, (int)offset.Y, width, width),
                            Color.White);
                if (ch.Health > 0)
                {
                    sb.Draw(mSelection.IsSelected(ch)
                            ? textureManager.GetTexture(ch.Texture + "HudSel")
                            : textureManager.GetTexture(ch.Texture + "Hud"),
                        new Rectangle((int)offset.X,(int)offset.Y, mPortraitWidth,mPortraitWidth),
                        Color.White);
                    mHealthBar.DrawCharacterHp(sb, offset, ch.Health, ch.MaxHealth);
                }
                else
                {
                    sb.Draw(textureManager.GetTexture(ch.Texture + "Dead"),
                        offset,
                        Color.White);
                }

                if (ch.Ability != null)
                {
                    //Global things
                    var abilityTexture = textureManager.GetTexture(ch.Texture + "Ability");
                    int numOfAbilityFrames = 20;


                    var frameWidth = abilityTexture.Width / numOfAbilityFrames;
                    var frameHeight = abilityTexture.Height;
                    var percentageOfCooldown = ch.Ability.SecondsPassed / ch.Ability.CoolDown;
                    percentageOfCooldown = percentageOfCooldown > 1 ? 1 : percentageOfCooldown;
                    var whichImage = (int)Math.Floor(percentageOfCooldown * (numOfAbilityFrames-1));
                    var abilitySourceRectangle = new Rectangle(whichImage * frameWidth, 0, frameWidth, frameHeight);
                    sb.Draw(
                        abilityTexture,
                        offset,
                        abilitySourceRectangle, 
                        Color.White);
                }

                offset.X += width + borderX;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace TRY.GameStates
{
    public class Textures
    {
        [JsonIgnore]
        private Dictionary<String, Texture2D> mTextureDictionary;
        [JsonIgnore]
        private Dictionary<String, SpriteFont> mFontDictionary;
        [JsonIgnore]
        private Texture2D mStandardTexture2D;

        public Textures(ContentManager content)
        {
            mStandardTexture2D = content.Load<Texture2D>("Ability/Rabbit");
            mTextureDictionary = new Dictionary<string, Texture2D>
            {
                {"Background", content.Load<Texture2D>("Menu/MenuBackground")},
                {"BackgroundStart", content.Load<Texture2D>("Menu/MenuBackgroundStart")},
                {"Button", content.Load<Texture2D>("Menu/Button")},
                {"Oxygen", content.Load<Texture2D>("Oxygen")},
                {"Energy", content.Load<Texture2D>("Utility/energy1")},
                {"Char", content.Load<Texture2D>("Utility/wooden-coffin")},
                {"Alien", content.Load<Texture2D>("Aliens/space-invaders")},
                {"Credits", content.Load<Texture2D>("Menu/credits") },
                {"Plus", content.Load<Texture2D>("Menu/plus") },
                {"GameOver", content.Load<Texture2D>("Menu/finish") },
                {"Win", content.Load<Texture2D>("Menu/win1") },
                {"VutHud", content.Load<Texture2D>("hud/WhiteAst") },
                {"MaximusHud", content.Load<Texture2D>("hud/GreenAst") },
                {"WienseHud", content.Load<Texture2D>("hud/Robot") },
                {"NgolHud", content.Load<Texture2D>("hud/Robot2") },
                {"BurkhaHud", content.Load<Texture2D>("hud/Sanitar") },
                {"DomogasHud", content.Load<Texture2D>("hud/Sanitar1") },
                {"VutHudSel", content.Load<Texture2D>("hud/WhiteAstSel") },
                {"MaximusHudSel", content.Load<Texture2D>("hud/GreenAstSel") },
                {"WienseHudSel", content.Load<Texture2D>("hud/RobotSel") },
                {"NgolHudSel", content.Load<Texture2D>("hud/Robot2Sel") },
                {"BurkhaHudSel", content.Load<Texture2D>("hud/SanitarSel") },
                {"DomogasHudSel", content.Load<Texture2D>("hud/Sanitar1Sel") },
                {"Rocket", content.Load<Texture2D>("Menu/rocket") }
            };


            mFontDictionary = new Dictionary<string, SpriteFont> {{"Font", content.Load<SpriteFont>("Menu/File")}};
        }

        public Texture2D GetTexture(string textureName)
        {
            if (mTextureDictionary.ContainsKey(textureName))
            {
                return mTextureDictionary[textureName];
            }
            else
            {
                return mStandardTexture2D;
            }
        }

        public SpriteFont GetFont(string fontName)
        {
            if (mFontDictionary.ContainsKey(fontName))
            {
                return mFontDictionary[fontName];
            }
            return mFontDictionary.Values.ElementAtOrDefault(0);
        }
    }
}

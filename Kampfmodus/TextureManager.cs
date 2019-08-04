using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TRY.Kampfmodus
{
    class TextureManager
    {
        private readonly Dictionary<string, Texture2D> mTextureDictionary;
        private readonly Dictionary<string, SpriteFont> mFontDictionary;
        private readonly Texture2D mStandardTexture2D;

        public TextureManager(ContentManager content, GraphicsDevice graphicsDevice)
        {
            mStandardTexture2D = content.Load<Texture2D>("Ability/Rabbit");

            var portraitWidth = 80;
            var portraitHeight = 80;

            var portraitSelected = new Texture2D(graphicsDevice, portraitWidth, portraitHeight);
            var color = new Color[portraitWidth * portraitHeight];
            for (var i = 0; i < color.Length; i++)
            {
                color[i] = Color.LightGreen;
            }
            portraitSelected.SetData(color);

            var portraitUnselected = new Texture2D(graphicsDevice, portraitWidth, portraitHeight);
            var uColor = new Color[portraitWidth * portraitHeight];
            for (var i = 0; i < uColor.Length; i++)
            {
                uColor[i] = Color.Chocolate;
            }
            portraitUnselected.SetData(uColor);

            var lineTexture = new Texture2D(graphicsDevice, 1, 1);
            var lineData = new Color[1];
            lineData[0] = Color.White;
            lineTexture.SetData(lineData);

            var deadAstronaut = content.Load<Texture2D>("hud/Astronaut_tot");
            var deadRobot = content.Load<Texture2D>("hud/Robot_tot");
            var deadSupport = content.Load<Texture2D>("hud/Sanitar_tot");


            mTextureDictionary = new Dictionary<string, Texture2D>
            {
                {"Vut", content.Load<Texture2D>("Characters/Fernkampf1/Astronaut")},
                {"VutDead", deadAstronaut},
                {"VutHud", content.Load<Texture2D>("hud/WhiteAst") },
                {"VutHudSel", content.Load<Texture2D>("hud/WhiteAstSel") },
                {"VutAbility", content.Load<Texture2D>($"Ability/hud/MineH") },

                {"Maximus", content.Load<Texture2D>("Characters/Fernkampf1/Astronaut2")},
                {"MaximusDead", deadAstronaut},
                {"MaximusHud", content.Load<Texture2D>("hud/GreenAst") },
                {"MaximusHudSel", content.Load<Texture2D>("hud/GreenAstSel") },
                {"MaximusAbility", content.Load<Texture2D>($"Ability/hud/RabbitH") },

                {"Ngol", content.Load<Texture2D>("Characters/Nahkampf/Robot")},
                {"NgolDead", deadRobot},
                {"NgolHud", content.Load<Texture2D>("hud/Robot") },
                {"NgolHudSel", content.Load<Texture2D>("hud/RobotSel") },
                {"NgolAbility", content.Load<Texture2D>($"Ability/hud/Rund") },

                {"Wiense", content.Load<Texture2D>("Characters/Nahkampf/Robot2")},
                {"WienseDead", deadRobot},
                {"WienseHud", content.Load<Texture2D>("hud/Robot2") },
                {"WienseHudSel", content.Load<Texture2D>("hud/Robot2Sel") },
                {"WienseAbility", content.Load<Texture2D>($"Ability/hud/Emp") },

                {"Domogas", content.Load<Texture2D>("Characters/Sanitare/Sanitar")},
                {"DomogasDead", deadSupport},
                {"DomogasHud", content.Load<Texture2D>("hud/Sanitar") },
                {"DomogasHudSel", content.Load<Texture2D>("hud/SanitarSel") },
                {"DomogasAbility", content.Load<Texture2D>($"Ability/hud/Gedanken") },

                {"Burkha", content.Load<Texture2D>("Characters/Sanitare/Sanitar1")},
                {"BurkhaDead", deadSupport},
                {"BurkhaHud", content.Load<Texture2D>("hud/Sanitar1") },
                {"BurkhaHudSel", content.Load<Texture2D>("hud/Sanitar1Sel") },
                {"BurkhaAbility", content.Load<Texture2D>($"Ability/hud/SchutzHud") },

                {"Oxygen", content.Load<Texture2D>("Oxygen")},
                {"Energy", content.Load<Texture2D>("Energy")},
                {"Cryo", content.Load<Texture2D>("Utility/wooden-coffin")},
                {"Alien", content.Load<Texture2D>("Aliens/space-invaders")},
                {"1234", content.Load<Texture2D>("Aliens/alien")},

                {"Boss", content.Load<Texture2D>("Aliens/Boss")},

                {"Explosiv", content.Load<Texture2D>("Aliens/Alien1")},
                {"ExplosivHud", content.Load<Texture2D>("hud/Alien1Hud")},
                {"ExplosivHudSel", content.Load<Texture2D>("hud/Alien1Hud")},
                {"ExplosivDead", content.Load<Texture2D>("hud/Alien1Dead")},

                {"MeleeEnemy", content.Load<Texture2D>("Aliens/Alien2")},
                {"MeleeEnemyHud", content.Load<Texture2D>("hud/Alien2Hud")},
                {"MeleeEnemyHudSel", content.Load<Texture2D>("hud/Alien2Hud")},
                {"MeleeEnemyDead", content.Load<Texture2D>("hud/Alien1Dead")},

                {"RangedEnemy", content.Load<Texture2D>("Aliens/Alien3")},
                {"RangedEnemyHud", content.Load<Texture2D>("hud/Alien3Hud")},
                {"RangedEnemyHudSel", content.Load<Texture2D>("hud/Alien3Hud")},
                {"RangedEnemyDead", content.Load<Texture2D>("hud/Alien3Dead")},

                {"Supervisor", content.Load<Texture2D>("Aliens/Alien4")},
                {"SupervisorHud", content.Load<Texture2D>("hud/Alien4Hud")},
                {"SupervisorHudSel", content.Load<Texture2D>("hud/Alien4Hud")},
                {"SupervisorDead", content.Load<Texture2D>("hud/Alien3Dead")},

                {"Projectile", content.Load<Texture2D>("Projectiles/Projectile")},
                {"Laser", content.Load<Texture2D>("Projectiles/Laser")},
                {"Circle", content.Load<Texture2D>("Projectiles/Circle")},
                {"Shield", content.Load<Texture2D>("Projectiles/Shield")},
                {"CryoChamber", content.Load<Texture2D>("CryoChamber/CryoSprite")},
                {"AbilityNotActive", content.Load<Texture2D>("Ability/aktivieren")},
                {"AbilityActive", content.Load<Texture2D>("Ability/aktiviert")},
                {"HealthBar", content.Load<Texture2D>("Utility/HealthBar")},
                {"PortraitActive", portraitSelected},
                {"PortraitInactive", portraitUnselected },
                {"LeftDoorTextureClosed", content.Load<Texture2D>("Map/LeftDoor")},
                {"RightDoorTextureClosed", content.Load<Texture2D>("Map/RightDoor")},
                {"LeftDoorTextureOpened", content.Load<Texture2D>("Map/LeftDoorOpened")},
                {"RightDoorTextureOpened", content.Load<Texture2D>("Map/RightDoorOpened")},
                {"HealthBarShield", content.Load<Texture2D>("Utility/HealthBarShield")},
                {"Explosion", content.Load<Texture2D>("Ability/explosion")},
                {"Rabbit", content.Load<Texture2D>("Ability/Rabbit") },

                {"DoorVertical", content.Load<Texture2D>("Map/Door1")},
                {"DoorHorizontal", content.Load<Texture2D>("Map/Door")},

                {"Mine", content.Load<Texture2D>("Ability/Mine") },
                {"RoundKick", content.Load<Texture2D>("Ability/RoundKick") },
                {"Explosion2", content.Load<Texture2D>("vfx/Explosion2") },
                {"WhiteLine", lineTexture }
            };
            mFontDictionary = new Dictionary<string, SpriteFont>
            {
                { "Font", content.Load<SpriteFont>("Menu/File") }
            };
        }

        public Texture2D GetTexture(string textureName)
        {
            return mTextureDictionary.ContainsKey(textureName) ?
                   mTextureDictionary[textureName] : mStandardTexture2D;
        }

        public SpriteFont GetFont(string fontName)
        {
            return mFontDictionary.ContainsKey(fontName) ? mFontDictionary[fontName] :
                                                           mFontDictionary.Values.ElementAtOrDefault(0);
        }
    }
}
